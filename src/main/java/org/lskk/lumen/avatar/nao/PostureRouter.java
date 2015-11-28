package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALRobotPoseProxy;
import com.aldebaran.proxy.ALRobotPostureProxy;
import com.aldebaran.proxy.ALSonarProxy;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.joda.time.DateTime;
import org.lskk.lumen.core.BatteryState;
import org.lskk.lumen.core.SonarState;
import org.lskk.lumen.core.util.AsError;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by student on 11/20/2015.
 */
@Component
public class PostureRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(PostureRouter.class);

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
        log.info("Sonar capture timer with period = {}ms", period);
        from("timer:sonar?period=" + period)
                .process(exchange -> {
                    final ALRobotPoseProxy robotPoseState  = new ALRobotPoseProxy();
                    robotPoseState.version((string) robotPoseProxy.getPoseNames());

                    exchange.getIn().setBody(robotPoseState);
                })
                .bean(toJson)
                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar." + avatarId + ".data.sonar")
                .to("log:sonar?showAll=true&multiline=true");
    }
}