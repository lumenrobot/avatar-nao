package id.ac.itb.lumen.avatar.nao;

import com.aldebaran.qimessaging.Application;
import com.aldebaran.qimessaging.CallError;
import com.aldebaran.qimessaging.Future;
import com.aldebaran.qimessaging.Session;
import com.aldebaran.qimessaging.helpers.ALInterface;
import com.aldebaran.qimessaging.helpers.ALModule;
import com.aldebaran.qimessaging.helpers.al.ALMotion;
import com.aldebaran.qimessaging.helpers.al.ALTextToSpeech;
import com.google.common.base.Throwables;
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
    public static final int sessionTimeoutMs = 5000;

    @Inject
    private Environment env;

    @Bean(destroyMethod = "stop")
    public Application naoqiApp() {
        final Application naoqiApp = new Application();
        // naoqiApp.run() blocks! should be in separate thread?
        return naoqiApp;
    }

    @Bean(destroyMethod="close")
    public Session naoSession() throws IOException {
        naoqiApp();

        ALModule.alInterface = new ALInterface() {
            @Override
            public void onALModuleReady() {
                log.info("Module is ready");
            }

            @Override
            public void onALModuleException(Exception e) {
                Throwables.propagate(e);
            }
        };

        final String naoUri = "tcp://" + env.getRequiredProperty("nao.host") + ":" +
                env.getProperty("nao.port", Integer.class, 9559);
        log.info("Connecting to NAO at {} ...", naoUri);
        final Session session = new Session();
        try {
            final Future<Void> future = session.connect(naoUri);
            synchronized (future) {
                future.wait(sessionTimeoutMs);//, TimeUnit.MILLISECONDS);
            }
            log.info("Connected to NAO at {}", naoUri);
            return session;
        } catch (Exception e) {
            throw new IOException("Cannot connect to NAO robot at " + naoUri + " in " + sessionTimeoutMs + "ms", e);
        }
    }

    @Bean
    public ALMotion naoMotion() throws IOException {
        log.info("Initializing Motion proxy...");
        return new ALMotion(naoSession());
    }

    @Bean
    public ALTextToSpeech naoTts() throws IOException, InterruptedException, CallError {
        log.info("Initializing TTS...");
        final ALTextToSpeech tts = new ALTextToSpeech(naoSession());
        tts.say("I am connected to Lumen Avatar Nao");
        return tts;
    }

}
