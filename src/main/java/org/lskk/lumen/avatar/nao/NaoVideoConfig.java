package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALVideoDeviceProxy;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import javax.inject.Inject;
import java.io.IOException;

/**
 * Created by ceefour on 03/10/2015.
 */
@Configuration
public class NaoVideoConfig {

    private static final Logger log = LoggerFactory.getLogger(NaoVideoConfig.class);

    @Inject
    private NaoConfig naoConfig;

    @Bean
    public ALVideoDeviceProxy naoVideoDevice() throws IOException {
        try {
            log.info("Initializing VideoDevice at {}:{}...", naoConfig.getNaoHost(), naoConfig.getNaoPort());
            return new ALVideoDeviceProxy(naoConfig.getNaoHost(), naoConfig.getNaoPort());
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO VideoDevice at " + naoConfig.getNaoHost() + ":" + naoConfig.getNaoPort(), e);
        }
    }

    @PostConstruct
    public void init() throws IOException {
        //log.info("Start cleanly: Unsubscribe all instances of VideoDevice '{}' ...", GVM_ID);
        //naoVideoDevice().unsubscribeAllInstances(GVM_ID); // start with clean state

        // make sure we're connected to VideoDevice
        naoVideoDevice();

        //default is to get image with this specification
        //resolution : 1    ;320*240
        //colorSpace : 13   ;BufferedImage.TYPE_3BYTE_BGR
        //frameRate  : 15   ;15 frame per second
        final int COLORSPACE_BGR = 13;
        log.info("Subscribing to VideoDevice '{}' ...", NaoConfig.GVM_ID);
        try {
            naoVideoDevice().subscribe(NaoConfig.GVM_ID, 1, COLORSPACE_BGR, NaoConfig.CAMERA_FPS);
        } catch (Exception e) {
            log.info("Unsubscribe VideoDevice '{}' ...", NaoConfig.GVM_ID);
            naoVideoDevice().unsubscribe(NaoConfig.GVM_ID);
            log.info("Re-subscribing to VideoDevice '{}' ...", NaoConfig.GVM_ID);
            naoVideoDevice().subscribe(NaoConfig.GVM_ID, 1, COLORSPACE_BGR, NaoConfig.CAMERA_FPS);
        }
    }

    @PreDestroy
    public void destroy() throws IOException {
        try {
            log.info("Releasing image VideoDevice '{}' ...", NaoConfig.GVM_ID);
            naoVideoDevice().releaseImage(NaoConfig.GVM_ID);
        } finally {
            log.info("Unsubscribe VideoDevice '{}' ...", NaoConfig.GVM_ID);
            naoVideoDevice().unsubscribe(NaoConfig.GVM_ID);
            log.info("Stop frame grabber...");
            naoVideoDevice().stopFrameGrabber(0);
        }
    }

}
