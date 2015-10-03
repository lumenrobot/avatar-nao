package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.google.common.util.concurrent.ListeningExecutorService;
import org.apache.camel.builder.RouteBuilder;
import org.lskk.lumen.core.ActingPerformance;
import org.lskk.lumen.core.LumenThing;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

@Component
public class ActorRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(ActorRouter.class);

    @Inject
    private ALLedsProxy ledsProxy;
    @Inject
    private ALMotionProxy motion;
    @Inject
    private ALRobotPostureProxy posture;
    @Inject
    private ALAudioPlayerProxy audioPlayer;
    @Inject
    private ToJson toJson;
    @Inject
    @ForNao
    private ListeningExecutorService naoExecutor;

    @Override
    public void configure() throws Exception {
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.actor")
                .to("log:IN.avatar.nao1.actor?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got actor command: {}", thing);
                    if (thing instanceof ActingPerformance) {
                        final ActingPerformance actingPerformance = (ActingPerformance) thing;
                        switch (actingPerformance.getScript()) {
                            case GOOD_BYE:
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "RHand"}),
                                        new Variant(new float[]{-0.9f, -0.44f, 0.53f, 0.3f, -0.2f, 1.0f}), 0.5f);
                                Thread.sleep(500);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{-0.9f, -0.44f, 0.53f, 1.2f, -0.2f}), 0.2f);
                                Thread.sleep(1000);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{-0.9f, -0.44f, 0.53f, 0.3f, -0.2f}), 0.2f);
                                Thread.sleep(1000);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{-0.9f, -0.44f, 0.53f, 1.2f, -0.2f}), 0.2f);
                                Thread.sleep(1000);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw", "RHand"}),
                                        new Variant(new float[]{-0.9f, -0.44f, 0.53f, 0.3f, -0.2f, 1.0f}), 0.2f);
                                Thread.sleep(1000);
                                break;

                            case PHOTO_POSE:
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{1.12f, 0.31f, 0.34f, 1.25f, -0.1f}), 0.2f);
                                motion.setAngles(
                                        new Variant(new String[]{
                                                "LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"
                                        }),
                                        new Variant(new float[]{
                                                -0.9f, 0.71f, -0.79f, -0.88f, 0.42f
                                        }), 0.2f);
                                motion.openHand("LHand");
                                break;

                            case DANCE_GANGNAM:
                                log.info("start music");
                                posture.goToPosture("StandInit", 0.5f);
                                naoExecutor.submit(() -> audioPlayer.playFile("/home/nao/gangnam.mp3"));
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{1.28f, 0.18f, 0.71f, 0.71f, -0.7f}), 0.2f);
                                motion.setAngles(
                                        new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"}),
                                        new Variant(new float[]{-0.72f, 0.04f, -0.67f, -1.06f, 0.97f}), 0.2f);
                                for (int i = 0; i < 6; i++) {
                                    motion.setAngles(
                                            new Variant(new String[]{"HeadPitch", "RShoulderPitch", "LShoulderPitch"}),
                                            new Variant(new float[]{0.0f, 1.28f, -0.72f}), 0.3f);
                                    Thread.sleep(500);
                                    motion.setAngles(
                                            new Variant(new String[]{"HeadPitch", "RShoulderPitch", "LShoulderPitch"}),
                                            new Variant(new float[]{0.51f, 1.10f, -0.60f}), 0.3f);
                                    Thread.sleep(500);
                                }
                                motion.setAngles(
                                        new Variant(new String[]{"HeadPitch", "RShoulderPitch", "LShoulderPitch"}),
                                        new Variant(new float[]{0.0f, 1.28f, -0.72f}), 0.3f);
                                Thread.sleep(500);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{-1.28f, -0.22f, -0.13f, 1.1f, 0.2f}), 0.2f);
                                motion.setAngles(
                                        new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"}),
                                        new Variant(new float[]{1.28f, -0.31f, -0.46f, -0.59f, 0.62f}), 0.2f);

                                for (int i = 0; i < 7; i++) {
                                    motion.setAngles(
                                            new Variant(new String[]{"HeadYaw", "RShoulderRoll", "LShoulderRoll"}),
                                            new Variant(new float[]{0.0f, -0.22f, -0.31f}), 0.2f);
                                    Thread.sleep(500);
                                    motion.setAngles(
                                            new Variant(new String[]{"HeadYaw", "RShoulderRoll", "LShoulderRoll"}),
                                            new Variant(new float[]{0.51f, -0.73f, 0.78f}), 0.3f);
                                    Thread.sleep(500);
                                }
                                posture.goToPosture("Stand", 0.5f);
                                break;

                            case SING_MANUK:
                                log.debug("Start singing {}", actingPerformance.getScript());
                                naoExecutor.submit(() -> audioPlayer.playFile("/home/nao/manuk.mp3"));
                                Thread.sleep(15000);
                                posture.goToPosture("StandInit", 0.5f);
                                motion.setAngles(
                                        new Variant(new String[]{"RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                        new Variant(new float[]{0.19f, 0.05f, 0.54f, 1.54f, 0.16f}), 0.2f);
                                for (int i = 0; i < 9; i++) {
                                    motion.setAngles(
                                            new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"}),
                                            new Variant(new float[]{-0.93f, 0.01f, -0.73f, -0.47f, 0.26f}), 0.1f);
                                    Thread.sleep(1500);
                                    motion.setAngles(
                                            new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw"}),
                                            new Variant(new float[]{-0.89f, 0.36f, -0.74f, -0.05f, 0.26f}), 0.1f);
                                    Thread.sleep(1500);
                                }
                                posture.goToPosture("Stand", 0.5f);
                                break;

                            case SING_UPTOWN:
                                posture.goToPosture("StandInit", 0.5f);
                                naoExecutor.submit(() -> audioPlayer.playFile("/home/nao/uptown.mp3"));
                                for (int i = 0; i <= 10; i++) {
                                    motion.setAngles(
                                            new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                            new Variant(new float[]{0.87f, -0.03f, -1.4f, -1.23f, 0.05f, 0.63f, -0.07f, 1.08f, 0.88f, 0.37f}), 0.3f);
                                    Thread.sleep(500);
                                    motion.setAngles(
                                            new Variant(new String[]{"LShoulderPitch", "LShoulderRoll", "LElbowYaw", "LElbowRoll", "LWristYaw", "RShoulderPitch", "RShoulderRoll", "RElbowYaw", "RElbowRoll", "RWristYaw"}),
                                            new Variant(new float[]{0.89f, -0.26f, -0.99f, -1.24f, 0.06f, 0.63f, 0.26f, 0.95f, 0.87f, 0.37f}), 0.4f);
                                    Thread.sleep(500);
                                }

                            default:
                                throw new UnsupportedOperationException("Unknown acting script: " + actingPerformance.getScript());
                        }
                    }
                });
    }
}
