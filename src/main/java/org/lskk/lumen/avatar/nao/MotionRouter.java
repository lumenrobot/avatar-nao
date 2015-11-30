package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALMotionProxy;
import com.sun.scenario.effect.impl.state.MotionBlurState;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.joda.time.DateTime;
import org.lskk.lumen.core.SonarState;
import org.lskk.lumen.core.util.AsError;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by student on 11/30/2015.
 */
@Component
public class MotionRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(SonarRouter.class);

    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;
    @Inject
    private ALMotionProxy motionProxy;
    @Inject
    private ProducerTemplate producer;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final String avatarId = "nao1";
        final int period = 1000;
        log.info("Motion capture timer with period = {}ms", period);
        from("timer:sonar?period=" + period)
                .process(exchange -> {
                    final MotionBlurState motionBlurState = new MotionBlurState();
                    motionBlurState.setAngle() motionProxy.getAngles());
                    motionBlurState.setRadius((float)); motionProxy.());
                    //  sonarState.setDateCreated(new DateTime());
                    motionBlurState.setAngle(float());
                    exchange.getIn().setBody(motionBlurState);
                })
                .bean(toJson)
                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&skipQueueDeclare=true&autoDelete=false&routingKey=avatar." + avatarId + ".data.motion");
         }
}