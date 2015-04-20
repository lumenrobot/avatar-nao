package id.ac.itb.lumen.avatar.nao;

import com.github.ooxi.jdatauri.DataUri;
import com.rabbitmq.client.ConnectionFactory;
import id.ac.itb.lumen.core.ImageObject;
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
                from("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.NAO.data.image")
                    .to("log:IN.avatar.NAO.data.image?showHeaders=true&showAll=true&multiline=true")
                    .process(new Processor() {
                        @Override
                        public void process(Exchange exchange) throws Exception {
//                            final ImageObject imageObject = toJson.getMapper().readValue(
//                                    (byte[]) exchange.getIn().getBody(), ImageObject.class);
//                            log.info("Object yang kita dapatkan: {}", imageObject);
//                            final DataUri dataUri = DataUri.parse(imageObject.getContentUrl(), StandardCharsets.UTF_8);
//                            final Mat ocvImg = Highgui.imdecode(new MatOfByte(dataUri.getData()), Highgui.IMREAD_UNCHANGED);
//                            log.info("OpenCV Mat: rows={} cols={}", ocvImg.rows(), ocvImg.cols());
                        }
                    });
            }
        };
    }

}
