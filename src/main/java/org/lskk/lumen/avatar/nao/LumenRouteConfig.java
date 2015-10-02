package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.fasterxml.jackson.databind.JsonNode;
import com.rabbitmq.client.ConnectionFactory;
import id.ac.itb.lumen.core.*;
import javafx.scene.paint.Color;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.HexDump;
import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.lskk.lumen.core.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Profile;
import org.springframework.core.env.Environment;

import javax.imageio.ImageIO;
import javax.inject.Inject;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferByte;
import java.io.ByteArrayOutputStream;

/**
 * Created by ceefour on 1/19/15.
 */
@Configuration
@Profile("avatarNaoApp")
public class LumenRouteConfig {

    private static final Logger log = LoggerFactory.getLogger(LumenRouteConfig.class);

//    @Inject
//    protected AgentRepository agentRepo
//    @Inject
//    protected ToJson toJson

    @Inject
    private Environment env;
    @Inject
    private ToJson toJson;
    @Inject
    private ALMotionProxy motion;
    @Inject
    private ALTextToSpeechProxy tts;
    @Inject
    private ALAudioDeviceProxy audioDevice;
    @Inject
    private ALRobotPostureProxy robotPosture;
    @Inject
    private ALVideoDeviceProxy videoDevice;
    @Inject
    private ALLedsProxy ledsProxy;

    @Bean
    public ConnectionFactory amqpConnFactory() {
        final ConnectionFactory connFactory = new ConnectionFactory();
        connFactory.setHost(env.getProperty("amqp.host", "localhost"));
        connFactory.setUsername(env.getProperty("amqp.username", "guest"));
        connFactory.setPassword(env.getProperty("amqp.password", "guest"));
        log.info("AMQP configuration: host={} username={}", connFactory.getHost(), connFactory.getUsername());
        return connFactory;
    }

    @Bean
    public RouteBuilder cameraStreamBuilder() {
        log.info("Initializing camera stream RouteBuilder");
        return new RouteBuilder() {
            @Override
            public void configure() throws Exception {
                final int period = 1000 / NaoConfig.CAMERA_FPS;
                from("timer:camera?period=" + period)
                        .process(exchange -> {
                            try {
                                log.trace("Getting image remote '{}' ...", NaoConfig.GVM_ID);
                                final Variant imageRemoteVariant = videoDevice.getImageRemote(NaoConfig.GVM_ID);
                                //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                                final byte[] imageRemote = imageRemoteVariant.getElement(6).toBinary();
                                log.trace("Image '{}': {} bytes", NaoConfig.GVM_ID, imageRemote.length);
                                if (log.isTraceEnabled()) {
                                    final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                                    HexDump.dump(imageRemote, 0, bos, 0);
                                    log.trace("{}", bos);
                                }
                                final BufferedImage bufImg = new BufferedImage(320, 240, BufferedImage.TYPE_3BYTE_BGR);
                                final byte[] bufImgData = ((DataBufferByte) bufImg.getRaster().getDataBuffer()).getData();
                                System.arraycopy(imageRemote, 0, bufImgData, 0, imageRemote.length);

                                final ImageObject imageObject = new ImageObject();
                                try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                                    ImageIO.write(bufImg, "jpg", baos);
                                    final byte[] bytes = baos.toByteArray();
                                    imageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                                    imageObject.setContentType("image/jpeg");
                                    imageObject.setContentSize((long) bytes.length);
                                    imageObject.setContentUrl("data:image/jpeg;base64," +
                                            Base64.encodeBase64String(bytes));
                                }
                                exchange.getIn().setBody(imageObject);
                            } finally {
                                videoDevice.releaseImage(NaoConfig.GVM_ID);
                            }
                        })
                        .bean(toJson)
                        .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main");
                        //.to("log:OUT.avatar.nao1.camera.main?showHeaders=true&showAll=true&multiline=true");
            }
        };
    }

    @Bean
    public RouteBuilder commandProcessorRouteBuilder() {
        log.info("Initializing command processor RouteBuilder");
        return new RouteBuilder() {
            @Override
            public void configure() throws Exception {
                from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.NAO.command")
                    .to("log:IN.avatar.NAO.command?showHeaders=true&showAll=true&multiline=true")
                    .process(exchange -> {
                        final LumenThing thing;
                        // "legacy" messages support
                        final JsonNode jsonNode = toJson.getMapper().readTree(exchange.getIn().getBody(byte[].class));
                        if ("motion".equals(jsonNode.path("type").asText()) && "wakeUp".equals(jsonNode.path("method").asText())) {
                            thing = new WakeUp();
                        } else if ("motion".equals(jsonNode.path("type").asText()) && "rest".equals(jsonNode.path("method").asText())) {
                            thing = new Rest();
                        } else if ("texttospeech".equals(jsonNode.path("type").asText()) && "say".equals(jsonNode.path("method").asText())) {
                            thing = new Speech(jsonNode.path("parameter").path("text").asText());
                        } else {
                            thing = toJson.getMapper().readValue(
                                    (byte[]) exchange.getIn().getBody(), LumenThing.class);
                        }
                        log.info("Got avatar command: {}", thing);
                        if (thing instanceof AudioVolume) {
                            log.info("Set volume {}", thing);
                            final int volumePct = (int) Math.round(((AudioVolume) thing).getVolume() * 100);
                            audioDevice.setOutputVolume(volumePct);
                            tts.say("My volume is now " + volumePct + "%");
                        } else if (thing instanceof Speech) {
                            log.info("Speaking: {}", ((Speech) thing).getMarkup());
                            tts.say(((Speech) thing).getMarkup());
                            log.info("Spoken");
                        } else if (thing instanceof WakeUp) {
                            log.info("Waking up...");
                            motion.wakeUp();
                            log.info("Woke up");
                        } else if (thing instanceof PostureChange) {
                            log.info("Changing posture {}", thing);
                            robotPosture.goToPosture(((PostureChange) thing).getPostureId(),
                                    (float) (double) ((PostureChange) thing).getSpeed());
                            log.info("Posture changed.");
                        } else if (thing instanceof Rest) {
                            log.info("Resting...");
                            motion.rest();
                            log.info("Rested");
                        } else if (thing instanceof MoveTo) {
                            final MoveTo moveTo = (MoveTo) thing;
                            final float naoMoveToX = (float) (-moveTo.getBackDistance());
                            final float naoMoveToY = (float) -moveTo.getRightDistance();
                            final float naoMoveToTheta = (float) Math.toRadians(moveTo.getTurnCcwDeg());
                            log.info("Moving {} as NAO: x={} y={} theta={} ...",
                                    moveTo, naoMoveToX, naoMoveToY, naoMoveToTheta);
                            motion.moveTo(naoMoveToX, naoMoveToY, naoMoveToTheta);
                            log.info("Moved");
                        } else if (thing instanceof JointInterpolateAngle) {
                            final JointInterpolateAngle jointInterpolateAngle = (JointInterpolateAngle) thing;
                            final float naoAngle = (float) Math.toRadians(jointInterpolateAngle.getTargetCcwDeg());
                            log.info("Interpolate {} as NAO: angle={} ...", jointInterpolateAngle, naoAngle);
                            motion.angleInterpolation(new Variant(jointInterpolateAngle.getJointId().name()),
                                    new Variant(naoAngle), new Variant((float) (double) jointInterpolateAngle.getDuration()), true);
                            log.info("Interpolated.");
                        }
                    });
            }
        };
    }

    @Bean
    public RouteBuilder ledsRouteBuilder() {
        log.info("Initializing LEDs RouteBuilder");
        return new RouteBuilder() {
            @Override
            public void configure() throws Exception {
                from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.leds")
                    .to("log:IN.avatar.nao1.leds?showHeaders=true&showAll=true&multiline=true")
                    .process(exchange -> {
                        final LumenThing thing;
                        // "legacy" messages support
                        final JsonNode jsonNode = toJson.getMapper().readTree(exchange.getIn().getBody(byte[].class));
                        thing = toJson.getMapper().readValue(exchange.getIn().getBody(byte[].class), LumenThing.class);
                        log.info("Got LED command: {}", thing);
                        if (thing instanceof LedOperation) {
                            final LedOperation ledOp = (LedOperation) thing;
                            final String ledName = ledOp.getNames().stream().findFirst().orElse("FaceLeds");
                            if (ledOp.getKind() == LedOperationKind.OFF) {
                                ledsProxy.off(ledName);
                            } else if (ledOp.getKind() == LedOperationKind.ON) {
                                ledsProxy.on(ledName);
                            } else if (ledOp.getKind() == LedOperationKind.FADE) {
                                //final Color rgb = Color.web(ledOp.getColor());
                                ledsProxy.fade(ledName, ledOp.getIntensity().floatValue(), ledOp.getDuration().floatValue());
                            } else if (ledOp.getKind() == LedOperationKind.FADE_RGB) {
                                final Color rgb = Color.web(ledOp.getColor());
                                final int rgbi = ((int)(rgb.getRed() * 0xff) << 16) | ((int)(rgb.getGreen() * 0xff) << 8) | (int)(rgb.getBlue() * 0xff);
                                ledsProxy.fadeRGB(ledName, rgbi, ledOp.getDuration().floatValue());
                            } else if (ledOp.getKind() == LedOperationKind.RANDOM_EYES) {
                                ledsProxy.randomEyes(ledOp.getDuration().floatValue());
                            } else if (ledOp.getKind() == LedOperationKind.RASTA) {
                                ledsProxy.rasta(ledOp.getDuration().floatValue());
                            }
                        }
                    });
            }
        };
    }

}
