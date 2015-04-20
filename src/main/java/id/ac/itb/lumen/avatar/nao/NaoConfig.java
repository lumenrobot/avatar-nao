package id.ac.itb.lumen.avatar.nao;

import com.aldebaran.qimessaging.Session;
import com.aldebaran.qimessaging.helpers.al.ALMotion;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;

import javax.inject.Inject;
import java.io.IOException;
import java.util.concurrent.TimeUnit;

/**
 * Created by ceefour on 20/04/2015.
 */
@Configuration
public class NaoConfig {

    private static Logger log = LoggerFactory.getLogger(NaoConfig.class);
    public static final int sessionTimeoutMs = 1000;

    @Inject
    private Environment env;

    @Bean(destroyMethod="close")
    public Session naoSession() throws IOException {
        final String naoUri = "tcp://" + env.getRequiredProperty("nao.host") + ":" +
                env.getProperty("nao.port", Integer.class, 9559);
        log.info("Connecting to NAO at {} ...", naoUri);
        final Session session = new Session();
        try {
            session.connect(naoUri).get(sessionTimeoutMs, TimeUnit.MILLISECONDS);
        } catch (Exception e) {
            throw new IOException("Cannot connect to NAO robot at " + naoUri + " in " + sessionTimeoutMs + "ms", e);
        }
        return session;
    }

    public ALMotion naoMotion() throws IOException {
        log.info("Initializing Motion proxy");
        return new ALMotion(naoSession());
    }

}
