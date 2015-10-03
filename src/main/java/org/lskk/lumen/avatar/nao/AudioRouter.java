package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.fasterxml.jackson.databind.JsonNode;
import com.github.ooxi.jdatauri.DataUri;
import com.google.common.base.Preconditions;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.io.FileUtils;
import org.apache.commons.io.FilenameUtils;
import org.apache.commons.net.ftp.FTP;
import org.apache.commons.net.ftp.FTPClient;
import org.lskk.lumen.core.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.core.env.Environment;
import org.springframework.stereotype.Component;

import javax.inject.Inject;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.net.URI;
import java.nio.charset.StandardCharsets;

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
    private ToJson toJson;

    @Override
    public void configure() throws Exception {
        final String naoPassword = env.getRequiredProperty("nao.password");
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.audio.out")
                .to("log:IN.avatar.nao1.audio.out?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(
                            exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got avatar command: {}", thing);
                    if (thing instanceof PlayAudio) {
                        final PlayAudio playAudio = (PlayAudio) thing;
                        log.info("Play audio {}", playAudio.getContentUrl());
                        final URI contentUrl = URI.create(playAudio.getContentUrl());
                        if ("file".equals(contentUrl.getScheme())) {
                            log.info("Playing locally: {}", contentUrl.getPath());
                            audioPlayer.playFile(contentUrl.getPath());
                        } else if ("data".equals(contentUrl.getScheme())) {
                            final DataUri dataUri = DataUri.parse(playAudio.getContentUrl(), StandardCharsets.UTF_8);
//                            final File audioFile = File.createTempFile("nao_", ".mp3");
                            try {
//                                log.info("Writing {} bytes to {} ...", dataUri.getData().length, audioFile);
//                                FileUtils.writeByteArrayToFile(audioFile, dataUri.getData());
                                final String destFile = "/home/nao/tmp.mp3";
                                final String naoUser = "nao";
                                log.info("Uploading {} bytes to ftp://{}@{}{} ...", dataUri.getData().length, naoUser, naoConfig.getNaoHost(), destFile);
                                final FTPClient ftpClient = new FTPClient();
                                ftpClient.connect(naoConfig.getNaoHost());
                                try {
                                    Preconditions.checkArgument(ftpClient.login(naoUser, naoPassword),
                                            "Cannot connect to FTP {} using user '{}'", naoConfig.getNaoHost(), naoUser);
                                    ftpClient.setFileType(FTP.BINARY_FILE_TYPE);
                                    try (ByteArrayInputStream bais = new ByteArrayInputStream(dataUri.getData())) {
                                        Preconditions.checkArgument(ftpClient.storeFile(FilenameUtils.getName(destFile), bais),
                                                "Cannot upload {} bytes to ftp://{}@{}{}", dataUri.getData().length,
                                                naoUser, naoConfig.getNaoHost(), destFile);
                                    }
                                } finally {
                                    ftpClient.disconnect();
                                }
                                log.info("Playing uploaded file locally: {}", destFile);
                                audioPlayer.playFile(destFile);
                            } finally {
//                                audioFile.delete();
                            }
                        } else {
                            throw new NaoException("Unknown audio URL: " + contentUrl);
                        }
                    } else if (thing instanceof StopAudio) {
                        log.info("Stopping audio!");
                        audioPlayer.stopAll();
                    }
                });
    }
}
