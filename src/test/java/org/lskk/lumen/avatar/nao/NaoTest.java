package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALMotionProxy;
import com.aldebaran.proxy.ALRobotPostureProxy;
import org.junit.Test;
import org.junit.runner.RunWith;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.autoconfigure.jackson.JacksonAutoConfiguration;
import org.springframework.context.annotation.Configuration;
import org.springframework.context.annotation.Import;
import org.springframework.context.annotation.PropertySource;
import org.springframework.test.context.ContextConfiguration;
import org.springframework.test.context.junit4.SpringJUnit4ClassRunner;

import javax.inject.Inject;

/**
 * Created by aina on 18/11/2015.
 */
@RunWith(SpringJUnit4ClassRunner.class)
@ContextConfiguration(classes = NaoTest.Config.class)
public class NaoTest {

    private static final Logger log = LoggerFactory.getLogger(NaoTest.class);

    @Configuration
    @Import({JacksonAutoConfiguration.class, NaoConfig.class})
    @PropertySource(value = {"classpath:application.properties", "file:config/application.properties"},
            ignoreResourceNotFound = true)
    //@ComponentScan("org.lskk.lumen.reasoner.quran")
//    @EnableConfigurationProperties
    public static class Config {

    }

    @Inject
    private ALMotionProxy motion;
    @Inject
    private ALRobotPostureProxy robotPosture;

    @Test
    public void rest() {
        log.info("Resting...");
        motion.rest();
        log.info("Rested");
    }

    @Test
    public void stand() throws InterruptedException {
        log.info("Standing...");
        motion.wakeUp();
        robotPosture.goToPosture("Stand", 0.7f);
        log.info("Stood. (waiting)");
        Thread.sleep(2000);
        log.info("Resting...");
        motion.rest();
    }
}
