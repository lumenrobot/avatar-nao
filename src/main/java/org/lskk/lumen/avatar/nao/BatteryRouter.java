package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALBatteryProxy;
import com.aldebaran.proxy.ALVideoDeviceProxy;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.bytedeco.javacpp.Loader;
import org.joda.time.DateTime;
import org.lskk.lumen.core.BatteryState;
import org.lskk.lumen.core.util.AsError;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by student on 11/20/2015.
 */
@Component
public class BatteryRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(BatteryRouter.class);

    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;
    @Inject
    private ALBatteryProxy batteryProxy;
    @Inject
    private ProducerTemplate producer;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final String avatarId = "nao1";
        final int period = 1000;
        log.info("Battery capture timer with period = {}ms", period);
        from("timer:battery?period=" + period)
                .process(exchange -> {
                    final BatteryState batteryState = new BatteryState();
                    batteryState.setPercentage((double) batteryProxy.getBatteryCharge());
                    batteryState.setDateCreated(new DateTime());
                    exchange.getIn().setBody(batteryState);
                })
                .bean(toJson)
                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar." + avatarId + ".data.battery");
                //.to("log:battery?showAll=true&multiline=true");
    }
}