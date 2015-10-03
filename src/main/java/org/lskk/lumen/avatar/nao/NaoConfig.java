package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.google.common.util.concurrent.ListeningExecutorService;
import com.google.common.util.concurrent.MoreExecutors;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;

import javax.inject.Inject;
import java.io.IOException;
import java.util.concurrent.Executors;

/**
 * Created by ceefour on 20/04/2015.
 */
@Configuration
public class NaoConfig {

    private static Logger log = LoggerFactory.getLogger(NaoConfig.class);
    public static final int sessionTimeoutMs = 5000;
    public static final String GVM_ID = "res1";
    public static final int CAMERA_FPS = 1;

    @Inject
    private Environment env;

/* jnaoqi v2.x
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
*/

    public String getNaoHost() {
        return env.getRequiredProperty("nao.host");
    }

    public int getNaoPort() {
        return env.getProperty("nao.port", Integer.class, 9559);
    }

    @Bean(destroyMethod = "shutdown")
    public ListeningExecutorService naoExecutor() {
        return MoreExecutors.listeningDecorator(Executors.newSingleThreadExecutor());
    }

    @Bean
    public ALMotionProxy naoMotion() throws IOException {
        try {
            log.info("Initializing Motion proxy at {}:{}...", getNaoHost(), getNaoPort());
            final ALMotionProxy motionProxy = new ALMotionProxy(getNaoHost(), getNaoPort());
            final boolean restAtInit = env.getProperty("nao.motion.rest-at-init", Boolean.class, true);
            if (restAtInit) {
                log.info("Resting NAO...");
                motionProxy.rest();
            }
            return motionProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO Motion at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALTextToSpeechProxy naoTts() throws IOException {
        try {
            log.info("Initializing TTS at {}:{}...", getNaoHost(), getNaoPort());
            final ALTextToSpeechProxy tts = new ALTextToSpeechProxy(getNaoHost(), getNaoPort());
            tts.setVolume(1f);
            naoExecutor().submit(() -> tts.say("Us-suh-lum-moo-ah-lay-koom wa-roh-ma-tool-laah-ee wa-ba-roh-kah-tooh"));
            return tts;
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO TextToSpeech at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALAudioDeviceProxy naoAudioDevice() throws IOException {
        try {
            log.info("Initializing AudioDevice at {}:{}...", getNaoHost(), getNaoPort());
            final ALAudioDeviceProxy audioDeviceProxy = new ALAudioDeviceProxy(getNaoHost(), getNaoPort());
            final Integer volume = env.getProperty("nao.audio.volume", Integer.class, 80);
            if (volume != null) {
                log.info("Set audio output volume to {}...", volume);
                audioDeviceProxy.setOutputVolume(volume);
            }
            return audioDeviceProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO AudioDevice at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALAudioPlayerProxy naoAudioPlayer() throws IOException {
        try {
            log.info("Initializing AudioPlayer at {}:{}...", getNaoHost(), getNaoPort());
            final ALAudioPlayerProxy audioPlayerProxy = new ALAudioPlayerProxy(getNaoHost(), getNaoPort());
            return audioPlayerProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO AudioPlayer at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALAudioRecorderProxy naoAudioRecorder() throws IOException {
        try {
            log.info("Initializing AudioRecorder at {}:{}...", getNaoHost(), getNaoPort());
            final ALAudioRecorderProxy audioRecorderProxy = new ALAudioRecorderProxy(getNaoHost(), getNaoPort());
            return audioRecorderProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO AudioRecorder at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALRobotPostureProxy naoRobotPosture() throws IOException {
        try {
            log.info("Initializing RobotPosture at {}:{}...", getNaoHost(), getNaoPort());
            return new ALRobotPostureProxy(getNaoHost(), getNaoPort());
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO RobotPosture at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

    @Bean
    public ALLedsProxy ledsProxy() throws IOException {
        try {
            log.info("Initializing LEDs at {}:{}...", getNaoHost(), getNaoPort());
            return new ALLedsProxy(getNaoHost(), getNaoPort());
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO LEDs at " + getNaoHost() + ":" + getNaoPort(), e);
        }
    }

}
