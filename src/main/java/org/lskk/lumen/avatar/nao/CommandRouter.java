package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.fasterxml.jackson.databind.JsonNode;
import org.apache.camel.builder.RouteBuilder;
import org.lskk.lumen.core.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by ceefour on 10/2/15.
 */
@Component
public class CommandRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(CommandRouter.class);

    @Inject
    private ALAudioDeviceProxy audioDevice;
    @Inject
    private ALMotionProxy motion;
    @Inject
    private ALRobotPostureProxy robotPosture;
    @Inject
    private ToJson toJson;
    @Inject
    private ALTextToSpeechProxy tts;

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
}
