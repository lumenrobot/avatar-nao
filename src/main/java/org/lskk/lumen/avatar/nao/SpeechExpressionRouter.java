package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.model.language.HeaderExpression;
import org.apache.commons.lang3.StringUtils;
import org.lskk.lumen.core.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;

/**
 * Created by ceefour on 10/2/15.
 */
@Component
public class SpeechExpressionRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(SpeechExpressionRouter.class);

    @Inject
    private ToJson toJson;
    @Inject
    private ALTextToSpeechProxy tts;

    @Override
    public void configure() throws Exception {
        // lumen.speech.expression
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=lumen.speech.expression")
                .to("log:IN.lumen.speech.expression?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(
                            exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got speech.expression command: {}", thing);
                    if (thing instanceof Speech) {
                        final Speech speech = (Speech) thing;
                        if (StringUtils.startsWith(speech.getAvatarId(), "nao")) {
                            log.info("Speaking for {}: {}", speech.getAvatarId(), speech.getMarkup());
                            tts.say(speech.getMarkup());
                            log.debug("Spoken for {}: {}", speech.getAvatarId(), speech.getMarkup());
                        }
                    }

                    // reply
                    exchange.getOut().setBody("{}");
                    final String replyTo = exchange.getIn().getHeader("rabbitmq.REPLY_TO", String.class);
                    if (replyTo != null) {
                        log.debug("Sending reply to {} ...", replyTo);
                        exchange.getOut().setHeader("rabbitmq.ROUTING_KEY", replyTo);
                        exchange.getOut().setHeader("rabbitmq.EXCHANGE_NAME", "");
                        exchange.getOut().setHeader("recipients",
                                "rabbitmq://localhost/dummy?connectionFactory=#amqpConnFactory&autoDelete=false,log:OUT.lumen.speech.expression");
                    } else {
                        exchange.getOut().setHeader("recipients", "log:OUT.lumen.speech.expression");
                    }
                })
                .recipientList(new HeaderExpression("recipients"));
    }
}
