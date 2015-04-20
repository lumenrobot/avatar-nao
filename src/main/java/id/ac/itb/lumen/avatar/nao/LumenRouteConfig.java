package id.ac.itb.lumen.avatar.nao;

import com.aldebaran.qimessaging.helpers.al.ALMotion;
import com.fasterxml.jackson.databind.JsonNode;
import com.github.ooxi.jdatauri.DataUri;
import com.rabbitmq.client.ConnectionFactory;
import id.ac.itb.lumen.core.*;
import org.apache.camel.Exchange;
import org.apache.camel.Processor;
import org.apache.camel.builder.RouteBuilder;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;

import javax.inject.Inject;
import java.nio.charset.StandardCharsets;
import java.util.Optional;

/**
 * Created by ceefour on 1/19/15.
 */
@Configuration
public class LumenRouteConfig {

    private static final Logger log = LoggerFactory.getLogger(LumenRouteConfig.class);

//    @Inject
//    protected AgentRepository agentRepo
//    @Inject
//    protected ToJson toJson

    @Inject
    private Environment env;
    @Inject
    private ToJson toJson;
    @Inject
    private ALMotion motion;

    @Bean
    public ConnectionFactory amqpConnFactory() {
        final ConnectionFactory connFactory = new ConnectionFactory();
        connFactory.setHost(env.getProperty("amqp.host", "localhost"));
        connFactory.setUsername(env.getProperty("amqp.username", "guest"));
        connFactory.setPassword(env.getProperty("amqp.password", "guest"));
        log.info("AMQP configuration: host={} username={}", connFactory.getHost(), connFactory.getUsername());
        return connFactory;
    }

    @Bean
    public RouteBuilder cameraProcessorRouteBuilder() {
        log.info("Initializing camera processor RouteBuilder");
        return new RouteBuilder() {
            @Override
            public void configure() throws Exception {
                from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.NAO.command")
                    .to("log:IN.avatar.NAO.data.image?showHeaders=true&showAll=true&multiline=true")
                    .process(new Processor() {
                        @Override
                        public void process(Exchange exchange) throws Exception {
                            final LumenThing thing;
                            // "legacy" messages support
                            final JsonNode jsonNode = toJson.getMapper().readTree((byte[]) exchange.getIn().getBody());
                            if ("motion".equals(jsonNode.path("type").textValue()) && "wakeUp".equals(jsonNode.path("method").textValue())) {
                                thing = new WakeUp();
                            } else if ("motion".equals(jsonNode.path("type").textValue()) && "rest".equals(jsonNode.path("method").textValue())) {
                                thing = new Rest();
                            } else {
                                thing = toJson.getMapper().readValue(
                                        (byte[]) exchange.getIn().getBody(), LumenThing.class);
                            }
                            log.info("Got avatar command: {}", thing);
                            if (thing instanceof WakeUp) {
                                log.info("Waking up...");
                                motion.wakeUp();
                                log.info("Woke up");
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
                            }
                        }
                    });
            }
        };
    }

}
