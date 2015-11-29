package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALRobotPoseProxy;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.lskk.lumen.core.RobotPoseState;
import org.lskk.lumen.core.util.AsError;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by student on 11/20/2015.
 */
@Component
public class PoseRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(PoseRouter.class);

    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;
    @Inject
    private ALRobotPoseProxy robotPoseProxy;
    @Inject
    private ProducerTemplate producer;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final String avatarId = "nao1";
        final int period = 1000;
        log.info("robotPose capture timer with period = {}ms", period);
        from("timer:robotPose?period=" + period)
                .process(exchange -> {
                    final RobotPoseState robotPoseState = new RobotPoseState();
                    for (int i = 0; i < robotPoseProxy.getPoseNames().getSize(); i++) {
                        robotPoseState.getPoseNames().add(robotPoseProxy.getPoseNames().getElement(i).toString());
                    }
                    robotPoseState.setActualPoseName(robotPoseProxy.getActualPoseAndTime().getElement(0).toString());
                    robotPoseState.setActualPoseTime(robotPoseProxy.getActualPoseAndTime().getElement(1).toFloat());
                    exchange.getIn().setBody(robotPoseState);
                })
                .bean(toJson)
                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&skipQueueDeclare=true&routingKey=avatar." + avatarId + ".data.robotpose")
                .to("log:robotPose?showAll=true&multiline=true");
    }
}