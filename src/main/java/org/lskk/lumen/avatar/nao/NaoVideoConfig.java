package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALVideoDeviceProxy;
import com.aldebaran.proxy.Variant;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import javax.inject.Inject;
import java.io.IOException;

/**
 * Created by ceefour on 03/10/2015.
 */
@Configuration
public class NaoVideoConfig {

    public static final String GVM_TOP_ID = "top";
    public static final String GVM_BOTTOM_ID = "bottom";
//    public static final int CAMERA_FPS = 1;
    private static final Logger log = LoggerFactory.getLogger(NaoVideoConfig.class);

    @Inject
    private NaoConfig naoConfig;
    @Inject
    private Environment env;
    private ImageResolution resolution;
    private int cameraFps;

    public int getCameraFps() {
        return this.cameraFps;
    }

    @Bean
    public ALVideoDeviceProxy naoVideoDevice() throws IOException {
        try {
            log.info("Initializing VideoDevice at {}:{}...", naoConfig.getNaoHost(), naoConfig.getNaoPort());
            return new ALVideoDeviceProxy(naoConfig.getNaoHost(), naoConfig.getNaoPort());
        } catch (Exception e) {
            throw new IOException("Cannot connect NAO VideoDevice at " + naoConfig.getNaoHost() + ":" + naoConfig.getNaoPort(), e);
        }
    }

    public ImageResolution getResolution() {
        return resolution;
    }

    @PostConstruct
    public void init() throws IOException {
        cameraFps = env.getRequiredProperty("nao.video.fps", Integer.class);

        //log.info("Start cleanly: Unsubscribe all instances of VideoDevice '{}' ...", GVM_TOP_ID);
        //naoVideoDevice().unsubscribeAllInstances(GVM_TOP_ID); // start with clean state

        // make sure we're connected to VideoDevice
        naoVideoDevice();

        //default is to get image with this specification
        //resolution : 1    ;320*240
        //colorSpace : 13   ;BufferedImage.TYPE_3BYTE_BGR
        //colorSpace : 9    ;YUV422
        //frameRate  : 15   ;15 frame per second
        final int COLORSPACE_BGR = 13;
        final int COLORSPACE_YUV422 = 9;
        resolution = env.getRequiredProperty("nao.video.resolution", ImageResolution.class);

        /**
         * Hendy's note: NAO V3.3 camera management is "buggy" (?), i.e.
         * if the active camera is 0, setFrameGrabber(1) will error: Can't launch camera driver
         * and subscribeCamera() is same as subscribe(), because it will just use the same camera index as active camera
         * So our workaround is: subscribeCamera(GVM_TOP_ID) but
         * for each getImageRemote we call setActiveCamera(0) and setActiveCamera(1)
         */

        log.info("Subscribing to {} VideoDevice '{}' '{}' ...", resolution, GVM_TOP_ID, GVM_BOTTOM_ID);
        try {
            final String gvmTopId = naoVideoDevice().subscribeCamera(GVM_TOP_ID, 0,
                    resolution.getNaoParameterId(), COLORSPACE_YUV422, cameraFps);
//            final String gvmBottomId = naoVideoDevice().subscribeCamera(GVM_BOTTOM_ID, 1,
//                    resolution.getNaoParameterId(), COLORSPACE_YUV422, CAMERA_FPS);
            log.info("Subscribed to {} VideoDevice '{}' '{}'", resolution,
                    GVM_TOP_ID, GVM_BOTTOM_ID);
        } catch (Exception e) {
            log.info("Unsubscribe VideoDevice '{}' '{}' ...", GVM_TOP_ID, GVM_BOTTOM_ID);
//            naoVideoDevice().unsubscribe(GVM_BOTTOM_ID);
            naoVideoDevice().unsubscribe(GVM_TOP_ID);
            log.info("Re-subscribing to {} VideoDevice '{}' '{}' ...", resolution, GVM_TOP_ID, GVM_BOTTOM_ID);
            final String gvmTopId = naoVideoDevice().subscribeCamera(GVM_TOP_ID, 0,
                    resolution.getNaoParameterId(), COLORSPACE_YUV422, cameraFps);
//            final String gvmBottomId = naoVideoDevice().subscribeCamera(GVM_BOTTOM_ID, 1,
//                    resolution.getNaoParameterId(), COLORSPACE_YUV422, CAMERA_FPS);
            log.info("Subscribed to {} VideoDevice '{}' '{}'", resolution,
                    GVM_TOP_ID, GVM_BOTTOM_ID);
        }
    }

    @PreDestroy
    public void destroy() throws IOException {
        try {
            log.info("Releasing images VideoDevice '{}' '{}' ...", GVM_TOP_ID, GVM_BOTTOM_ID);
//            naoVideoDevice().releaseImage(GVM_BOTTOM_ID);
            naoVideoDevice().releaseImage(GVM_TOP_ID);
        } finally {
            log.info("Unsubscribe VideoDevice '{}' '{}' ...", GVM_TOP_ID, GVM_BOTTOM_ID);
//            naoVideoDevice().unsubscribe(GVM_BOTTOM_ID);
            naoVideoDevice().unsubscribe(GVM_TOP_ID);
        }
    }

}
