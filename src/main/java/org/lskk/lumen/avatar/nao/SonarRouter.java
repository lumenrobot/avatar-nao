package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALBatteryProxy;
import com.aldebaran.proxy.ALRobotPoseProxy;
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
public class SonarRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(SonarRouter.class);

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
        log.info("Sonar capture timer with period = {}ms", period);
        for (final String avatarId : naoConfig.getControllerAvatarIds()) {
            final NaoController nao = naoConfig.get(avatarId);
            final ALSonarProxy sonarProxy = nao.getSonar();
            from("timer:sonar?period=" + period)
                    .process(exchange -> {
                        final SonarState sonarState = new SonarState();
                        // sonarState. setPercentage((double) batteryProxy.getBatteryCharge());
                        sonarState.setLeftSensor((double) sonarProxy.getCurrentPrecision());
                        sonarState.setRightSensor((double) sonarProxy.getCurrentPrecision());
                        //  sonarState.setDateCreated(new DateTime());
                        sonarState.setDateCreated(new DateTime());
                        exchange.getIn().setBody(sonarState);
                    })
                    .bean(toJson)
                    .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&skipQueueDeclare=true&autoDelete=false&routingKey=avatar." + avatarId + ".data.sonar")
                    .to("log:" + SonarRouter.class.getName() + "." + avatarId + "?level=TRACE&showAll=true&multiline=true");
        }
    }
}