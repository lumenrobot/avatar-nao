package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.fasterxml.jackson.databind.JsonNode;
import com.google.common.base.Preconditions;
import org.apache.camel.Expression;
import org.apache.camel.LoggingLevel;
import org.apache.camel.Predicate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.model.language.HeaderExpression;
import org.apache.commons.lang3.StringUtils;
import org.lskk.lumen.core.*;
import org.lskk.lumen.core.util.AsError;
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
    private ALTextToSpeechProxy tts;
    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        // avatar.*.command
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.command")
                .to("log:IN.avatar.nao*.command?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(
                                exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got avatar.nao* command: {}", thing);
                    if (thing instanceof AudioVolume) {
                        log.info("Set volume {}", thing);
                        final int volumePct = (int) Math.round(((AudioVolume) thing).getVolume() * 100);
                        audioDevice.setOutputVolume(volumePct);
                        tts.say("My volume is now " + volumePct + "%");
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

                    // reply
                    exchange.getIn().setBody(new Status());
                })
                .bean(toJson);
//                .to("log:OUT.avatar.nao1.command");
    }

}
