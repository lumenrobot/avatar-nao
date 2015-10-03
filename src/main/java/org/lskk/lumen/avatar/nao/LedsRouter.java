package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALLedsProxy;
import com.fasterxml.jackson.databind.JsonNode;
import javafx.scene.paint.Color;
import org.apache.camel.builder.RouteBuilder;
import org.lskk.lumen.core.LedOperation;
import org.lskk.lumen.core.LedOperationKind;
import org.lskk.lumen.core.LumenThing;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by ceefour on 10/2/15.
 */
@Component
public class LedsRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(LedsRouter.class);

    @Inject
    private ALLedsProxy ledsProxy;
    @Inject
    private ToJson toJson;

    @Override
    public void configure() throws Exception {
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.leds&concurrentConsumers=4")
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
                            final int rgbi = ((int) (rgb.getRed() * 0xff) << 16) | ((int) (rgb.getGreen() * 0xff) << 8) | (int) (rgb.getBlue() * 0xff);
                            ledsProxy.fadeRGB(ledName, rgbi, ledOp.getDuration().floatValue());
                        } else if (ledOp.getKind() == LedOperationKind.RANDOM_EYES) {
                            ledsProxy.randomEyes(ledOp.getDuration().floatValue());
                        } else if (ledOp.getKind() == LedOperationKind.RASTA) {
                            ledsProxy.rasta(ledOp.getDuration().floatValue());
                        }
                    }
                });
    }
}
