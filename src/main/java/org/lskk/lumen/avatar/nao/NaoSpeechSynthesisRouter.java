package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import org.apache.camel.LoggingLevel;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.apache.camel.model.language.HeaderExpression;
import org.apache.commons.lang3.StringUtils;
import org.lskk.lumen.core.*;
import org.lskk.lumen.core.util.AsError;
import org.lskk.lumen.core.util.ToJson;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.inject.Inject;
import java.util.Locale;
import java.util.Optional;

/**
 * Created by ceefour on 10/2/15.
 */
@Component
public class NaoSpeechSynthesisRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(NaoSpeechSynthesisRouter.class);

    @Inject
    private NaoConfig naoConfig;
    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        // lumen.speech.expression
        // TODO: we should delay e.g. 500ms to see if speech-expression handles it (and notifies with actionStatus=ACTIVE_ACTION_STATUS),
        // otherwise NAO TTS will handle it
        from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&queue=" + LumenChannel.SPEECH_SYNTHESIS.key() + "&routingKey=" + LumenChannel.SPEECH_SYNTHESIS.key())
                .to("log:IN.lumen.speech.synthesis?showHeaders=true&showAll=true&multiline=true")
                .process(exchange -> {
                    final LumenThing thing = toJson.getMapper().readValue(
                            exchange.getIn().getBody(byte[].class), LumenThing.class);
                    log.info("Got speech.synthesis command: {}", thing);
                    if (thing instanceof CommunicateAction) {
                        final CommunicateAction communicateAction = (CommunicateAction) thing;
                        if (naoConfig.getControllerAvatarIds().contains(communicateAction.getAvatarId())) {
                            final Locale lang = Optional.ofNullable(communicateAction.getInLanguage()).orElse(Locale.US);
                            if ("en".equals(lang.getLanguage())) {
                                log.info("Speaking {} for {}: {}", lang.toLanguageTag(), communicateAction.getAvatarId(), communicateAction.getObject());
                                naoConfig.get(communicateAction.getAvatarId()).getTts().say(communicateAction.getObject());
                                log.debug("Spoken {} for {}: {}", lang.toLanguageTag(), communicateAction.getAvatarId(), communicateAction.getObject());
                            } else {
                                log.info("Language '{}' not supported by {}, skipping: {}",
                                        lang.toLanguageTag(), communicateAction.getAvatarId(), communicateAction.getObject());
                            }
                        }
                        exchange.getIn().setBody(new Status());
                    } else {
                        exchange.getOut().setBody(null);
                    }
                })
                .bean(toJson);
//                .to("log:OUT.lumen.speech.synthesis");
    }
}
