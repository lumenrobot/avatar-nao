package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.github.ooxi.jdatauri.DataUri;
import com.google.common.base.Preconditions;
import org.apache.camel.Body;
import org.apache.camel.Header;
import org.apache.camel.Produce;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.io.IOUtils;
import org.apache.commons.net.ftp.FTP;
import org.apache.commons.net.ftp.FTPClient;
import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.lskk.lumen.core.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.core.env.Environment;
import org.springframework.stereotype.Component;

import javax.inject.Inject;
import javax.sound.sampled.AudioFormat;
import javax.sound.sampled.AudioInputStream;
import javax.sound.sampled.AudioSystem;
import javax.sound.sampled.UnsupportedAudioFileException;
import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URI;
import java.nio.charset.StandardCharsets;
import java.util.Optional;

@Component
public class AudioRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(AudioRouter.class);

    @Inject
    private Environment env;
    @Inject
    private NaoConfig naoConfig;
    @Inject
    private ALAudioDeviceProxy audioDevice;
    @Inject
    private ALAudioPlayerProxy audioPlayer;
    @Inject
    private ALAudioRecorderProxy audioRecorder;
    @Inject
    private ToJson toJson;
    @Inject
    private ProducerTemplate producerTemplate;

//    public interface DataRecording {
//        void sendAudio(@Body AudioObject audioObject, @Header("avatarId") String avatarId);
//    }
//
//    @Produce(uri = "seda:data-recording")
//    private DataRecording dataRecording;

    public static class AudioData {
        private final AudioFormat format;
        private final byte[] data;
        private final AudioFormat sourceFormat;

        public AudioData(AudioFormat format, byte[] data) {
            this.format = format;
            this.data = data;
            this.sourceFormat = null;
        }

        public AudioData(AudioFormat format, byte[] data, AudioFormat sourceFormat) {
            this.format = format;
            this.data = data;
            this.sourceFormat = sourceFormat;
        }

        public AudioFormat getFormat() {
            return format;
        }

        public byte[] getData() {
            return data;
        }

        public AudioFormat getSourceFormat() {
            return sourceFormat;
        }
    }

    public static AudioData convertAudioData(byte[] sourceBytes, AudioFormat audioFormat) throws IOException, UnsupportedAudioFileException {
        if (sourceBytes == null || sourceBytes.length == 0 || audioFormat == null) {
            throw new IllegalArgumentException("Illegal Argument passed to this method");
        }

        try (final ByteArrayInputStream bais = new ByteArrayInputStream(sourceBytes);
             final AudioInputStream sourceAIS = AudioSystem.getAudioInputStream(bais)) {
            final AudioFormat sourceFormat = sourceAIS.getFormat();
            final AudioFormat convertFormat = new AudioFormat(
                    AudioFormat.Encoding.PCM_SIGNED, sourceFormat.getSampleRate(), 16,
                    sourceFormat.getChannels(), sourceFormat.getChannels() * 2, sourceFormat.getSampleRate(), false);
            try (final AudioInputStream convert1AIS = AudioSystem.getAudioInputStream(convertFormat, sourceAIS);
                 final AudioInputStream convert2AIS = AudioSystem.getAudioInputStream(audioFormat, convert1AIS);
                 final ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                byte[] buffer = new byte[8192];
                while (true) {
                    int readCount = convert2AIS.read(buffer, 0, buffer.length);
                    if (readCount == -1) {
                        break;
                    }
                    baos.write(buffer, 0, readCount);
                }
                return new AudioData(audioFormat, baos.toByteArray(), sourceFormat);
            }
        }
    }

    public static AudioData getAudioDataPcmSigned(byte[] sourceBytes) throws IOException, UnsupportedAudioFileException {
        if (sourceBytes == null || sourceBytes.length == 0) {
            throw new IllegalArgumentException("Illegal Argument passed to this method");
        }

        try (final ByteArrayInputStream bais = new ByteArrayInputStream(sourceBytes);
             final AudioInputStream sourceAIS = AudioSystem.getAudioInputStream(bais)) {
            final AudioFormat sourceFormat = sourceAIS.getFormat();
            final AudioFormat convertFormat = new AudioFormat(
                    AudioFormat.Encoding.PCM_SIGNED, sourceFormat.getSampleRate(), 16,
                    sourceFormat.getChannels(), sourceFormat.getChannels() * 2, sourceFormat.getSampleRate(), false);
            try (final AudioInputStream convert1AIS = AudioSystem.getAudioInputStream(convertFormat, sourceAIS);
                 final ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                byte[] buffer = new byte[8192];
                while (true) {
                    int readCount = convert1AIS.read(buffer, 0, buffer.length);
                    if (readCount == -1) {
                        break;
                    }
                    baos.write(buffer, 0, readCount);
                }
                return new AudioData(convertFormat, baos.toByteArray(), sourceFormat);
            }
        }
    }

    @Override
    public void configure() throws Exception {
        final String naoPassword = env.getRequiredProperty("nao.password");
        // NAO only supports limited sample rates, so we limit to 22050 Hz (default)
        final int sampleRate = 22050;
        final int channelCount = 2;
        final String naoUser = "nao";
        final AudioFormat naoFormat = new AudioFormat(
                AudioFormat.Encoding.PCM_SIGNED, sampleRate, 16,
                channelCount, channelCount * 2, sampleRate, false);
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.audio.out&concurrentConsumers=4")
                .to("log:IN.avatar.nao1.audio.out?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(
                            exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got avatar command: {}", thing);
                    if (thing instanceof AudioObject) {
                        final AudioObject playAudio = (AudioObject) thing;
                        log.info("Play audio {}", playAudio.getContentUrl());
                        final URI contentUrl = URI.create(playAudio.getContentUrl());
                        if ("file".equals(contentUrl.getScheme())) {
                            log.info("Playing locally: {}", contentUrl.getPath());
                            audioPlayer.playFile(contentUrl.getPath());
                            log.info("Played locally: {}", contentUrl.getPath());
                        } else if ("data".equals(contentUrl.getScheme())) {
                            final DataUri dataUri = DataUri.parse(playAudio.getContentUrl(), StandardCharsets.UTF_8);

//                            final String destFile = "/home/nao/tmp.mp3";
//                            log.info("Uploading {} bytes to ftp://{}@{}{} ...", dataUri.getData().length, naoUser, naoConfig.getNaoHost(), destFile);
//                            final FTPClient ftpClient = new FTPClient();
//                            ftpClient.connect(naoConfig.getNaoHost());
//                            try {
//                                Preconditions.checkArgument(ftpClient.login(naoUser, naoPassword),
//                                        "Cannot connect to FTP {} using user '{}'", naoConfig.getNaoHost(), naoUser);
//                                ftpClient.setFileType(FTP.BINARY_FILE_TYPE);
//                                try (ByteArrayInputStream bais = new ByteArrayInputStream(dataUri.getData())) {
//                                    Preconditions.checkArgument(ftpClient.storeFile(FilenameUtils.getName(destFile), bais),
//                                            "Cannot upload {} bytes to ftp://{}@{}{}", dataUri.getData().length,
//                                            naoUser, naoConfig.getNaoHost(), destFile);
//                                }
//                            } finally {
//                                ftpClient.disconnect();
//                            }
//                            log.info("Playing uploaded file locally: {}", destFile);
//                            audioPlayer.playFile(destFile);
//                            log.info("Played uploaded file locally: {}", destFile);

                            // using remotebuffer
                            final AudioData naoSound = convertAudioData(dataUri.getData(), naoFormat);
                            final Variant fStereoAudioData = new Variant(naoSound.getData());
                            final int frameCount = naoSound.getData().length / naoSound.getFormat().getFrameSize();
                            log.info("Playing converted file from {} bytes to {} bytes PCM_SIGNED in {} frames from {}",
                                    dataUri.getData().length, naoSound.getData().length,
                                    frameCount, naoSound.getSourceFormat());
//                            audioDevice.setParameter("outputSampleRate", (int) naoSound.getFormat().getSampleRate());
                            audioDevice.sendRemoteBufferToOutput(frameCount, fStereoAudioData);
                            log.info("Played converted file from {} bytes to {} bytes PCM_SIGNED in {} frames from {}",
                                    dataUri.getData().length, naoSound.getData().length,
                                    frameCount, naoSound.getSourceFormat());
                        } else {
                            throw new NaoException("Unknown audio URL: " + contentUrl);
                        }
                    } else if (thing instanceof StopAudio) {
                        log.info("Stopping audio!");
                        audioPlayer.stopAll();
                    } else if (thing instanceof RecordAudio) {
                        final RecordAudio recordAudio = (RecordAudio) thing;
                        final String naoHomeFolder = "/home/nao";
                        final String remoteFile = "recordings/microphones/recording.wav";
                        log.info("Executing audioDevice.record() to {} ...", remoteFile);
                        audioRecorder.stopMicrophonesRecording();
                        audioRecorder.startMicrophonesRecording(naoHomeFolder + "/" + remoteFile, "wav", 16000,
                                new Variant(new int[]{0, 0, 1, 0}));
                        Thread.sleep(Math.round(Optional.ofNullable(recordAudio.getDuration()).orElse(5.0) * 1000));
                        audioRecorder.stopMicrophonesRecording();

                        log.debug("Downloading {} ...", remoteFile);
                        final byte[] rawData;
                        final FTPClient ftpClient = new FTPClient();
                        ftpClient.connect(naoConfig.getNaoHost());
                        try {
                            Preconditions.checkArgument(ftpClient.login(naoUser, naoPassword),
                                    "Cannot connect to FTP {} using user '{}'", naoConfig.getNaoHost(), naoUser);
                            ftpClient.setFileType(FTP.BINARY_FILE_TYPE);
                            try (InputStream ftpis = ftpClient.retrieveFileStream(remoteFile)) {
                                rawData = IOUtils.toByteArray(ftpis);
                                log.debug("Retrieved {} bytes of recorded file", rawData.length);
                            }
                        } finally {
                            ftpClient.disconnect();
                        }

                        final AudioObject audioObject = new AudioObject();
                        audioObject.setName(FilenameUtils.getName(remoteFile));
                        audioObject.setContentSize((long) rawData.length);
                        audioObject.setContentType("audio/x-wav");
                        audioObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                        audioObject.setUploadDate(audioObject.getDateCreated());
                        audioObject.setDatePublished(audioObject.getDateCreated());
                        audioObject.setDateModified(audioObject.getDateCreated());
                        audioObject.setContentUrl("data:" + audioObject.getContentType() + ";base64," +
                                Base64.encodeBase64String(rawData));

                        final String dataRecordingUri = "rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.audio.in";
                        producerTemplate.sendBody(dataRecordingUri, toJson.mapper.writeValueAsBytes(audioObject));
                        log.info("execution audioDevice.record() finished with {} bytes", rawData.length);
                    }
                });
    }
}
