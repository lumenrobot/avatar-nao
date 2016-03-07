package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALLedsProxy;
import com.aldebaran.proxy.ALMotionProxy;
import com.aldebaran.proxy.Variant;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.joda.time.DateTime;
import org.lskk.lumen.core.JointState;
import org.lskk.lumen.core.util.AsError;
import org.lskk.lumen.core.MotionState;
import org.lskk.lumen.core.util.ToJson;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;
import java.util.List;

/**
 * Created by student on 12/6/2015.
 */
@Component
public class MotionRouter extends RouteBuilder {
    private static final Logger log = LoggerFactory.getLogger(BatteryRouter.class);

    @Inject
    private NaoConfig naoConfig;
    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;
    @Inject
    private ProducerTemplate producer;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final int period = 1000;
        log.info("Motion capture timer with period = {}ms", period);
        for (final String avatarId : naoConfig.getControllerAvatarIds()) {
            final NaoController nao = naoConfig.get(avatarId);
            final ALMotionProxy motionProxy = nao.getMotion();
            final String[] angleNames = motionProxy.getJointNames("Body");
            from("timer:motion?period=" + period)
                    .process(exchange -> {
                        final MotionState motionState = new MotionState();
                        final float[] angles = motionProxy.getAngles(new Variant("Body"), true);
                        for (int i = 0; i < angles.length; i++) {
                            final JointState jointState = new JointState();
                            jointState.setName(angleNames[i]);
                            jointState.setAngle((double) angles[i]);
                            motionState.getAngles().add(jointState);
                        }

                        motionState.setDateCreated(new DateTime());
                        exchange.getIn().setBody(motionState);
                    })
                    .bean(toJson)
                    .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&skipQueueDeclare=true&autoDelete=false&routingKey=avatar." + avatarId + ".data.motion")
                    .to("log:" + MotionRouter.class.getName() + "." + avatarId + "?level=TRACE&showAll=true&multiline=true");
        }
    }

}
