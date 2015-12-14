package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.*;
import com.google.common.util.concurrent.ListeningExecutorService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.core.env.Environment;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import java.io.IOException;

/**
 * Created by ceefour on 14/12/2015.
 */
public class NaoController {

    public static final String GVM_TOP_ID = "top";
    public static final String GVM_BOTTOM_ID = "bottom";
//    public static final int CAMERA_FPS = 1;

    private final Logger log;
    private final ListeningExecutorService naoExecutor;
    private Environment env;
    private final String avatarId;
    private final String host;
    private final int port;
    private ImageResolution resolution;
    private int cameraFps;

    private ALMotionProxy motion;
    private ALTextToSpeechProxy tts;
    private ALAudioDeviceProxy audioDevice;
    private ALAudioPlayerProxy audioPlayer;
    private ALAudioRecorderProxy audioRecorder;
    private ALBatteryProxy battery;
    private ALSonarProxy sonar;
    private ALVideoDeviceProxy videoDevice;
    private ALRobotPostureProxy robotPosture;
    private ALRobotPoseProxy robotPose;
    private ALLedsProxy leds;

    public NaoController(String avatarId, String host, int port,
                         Environment env, ListeningExecutorService naoExecutor) {
        this.log = LoggerFactory.getLogger(NaoConfig.class.getName() + "." + avatarId);
        this.env = env;
        this.naoExecutor = naoExecutor;
        this.avatarId = avatarId;
        this.host = host;
        this.port = port;
    }

    public String getAvatarId() {
        return avatarId;
    }

    public String getHost() {
        return host;
    }

    public int getPort() {
        return port;
    }

    public int getCameraFps() {
        return this.cameraFps;
    }

    public ImageResolution getResolution() {
        return resolution;
    }

    @PostConstruct
    public void init() throws IOException {
        this.motion = naoMotion();
        this.tts = naoTts();
        this.audioDevice = naoAudioDevice();
        this.audioPlayer = naoAudioPlayer();
        this.audioRecorder = naoAudioRecorder();
        this.battery = batteryProxy();
        this.sonar = sonarProxy();
        this.robotPosture = naoRobotPosture();
        this.robotPose = robotPoseProxy();
        this.leds = ledsProxy();
        initVideo();
    }

    @PreDestroy
    public void destroy() throws IOException {
        destroyVideo();
    }

    protected void initVideo() throws IOException {
        cameraFps = env.getRequiredProperty("nao.video.fps", Integer.class);

        //log.info("Start cleanly: Unsubscribe all instances of VideoDevice '{}' ...", GVM_TOP_ID);
        //naoVideoDevice().unsubscribeAllInstances(GVM_TOP_ID); // start with clean state

        // make sure we're connected to VideoDevice
        this.videoDevice = naoVideoDevice();

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

        log.info("Subscribing to {} {} VideoDevice '{}' '{}' ...", avatarId, resolution, GVM_TOP_ID, GVM_BOTTOM_ID);
        try {
            final String gvmTopId = videoDevice.subscribeCamera(GVM_TOP_ID, 0,
                    resolution.getNaoParameterId(), COLORSPACE_YUV422, cameraFps);
//            final String gvmBottomId = videoDevice.subscribeCamera(GVM_BOTTOM_ID, 1,
//                    resolution.getNaoParameterId(), COLORSPACE_YUV422, CAMERA_FPS);
            log.info("Subscribed to {} {} VideoDevice '{}' '{}'", avatarId, resolution,
                    GVM_TOP_ID, GVM_BOTTOM_ID);
        } catch (Exception e) {
            log.info("Unsubscribe {} VideoDevice '{}' '{}' ...", avatarId, GVM_TOP_ID, GVM_BOTTOM_ID);
//            videoDevice.unsubscribe(GVM_BOTTOM_ID);
            videoDevice.unsubscribe(GVM_TOP_ID);
            log.info("Re-subscribing to {} {} VideoDevice '{}' '{}' ...", avatarId, resolution, GVM_TOP_ID, GVM_BOTTOM_ID);
            final String gvmTopId = videoDevice.subscribeCamera(GVM_TOP_ID, 0,
                    resolution.getNaoParameterId(), COLORSPACE_YUV422, cameraFps);
//            final String gvmBottomId = videoDevice.subscribeCamera(GVM_BOTTOM_ID, 1,
//                    resolution.getNaoParameterId(), COLORSPACE_YUV422, CAMERA_FPS);
            log.info("Subscribed to {} {} VideoDevice '{}' '{}'", avatarId, resolution,
                    GVM_TOP_ID, GVM_BOTTOM_ID);
        }
    }

    public void destroyVideo() throws IOException {
        try {
            log.info("Releasing images {} VideoDevice '{}' '{}' ...", avatarId, GVM_TOP_ID, GVM_BOTTOM_ID);
//            videoDevice.releaseImage(GVM_BOTTOM_ID);
            videoDevice.releaseImage(GVM_TOP_ID);
        } finally {
            log.info("Unsubscribe {} VideoDevice '{}' '{}' ...", avatarId, GVM_TOP_ID, GVM_BOTTOM_ID);
//            videoDevice.unsubscribe(GVM_BOTTOM_ID);
            videoDevice.unsubscribe(GVM_TOP_ID);
        }
    }

    protected ALMotionProxy naoMotion() throws IOException {
        try {
            log.info("Initializing {} Motion proxy at {}:{}...", avatarId, host, port);
            final ALMotionProxy motionProxy = new ALMotionProxy(host, port);
            log.info("{} Motion {}: {}", avatarId, motionProxy.version(), motionProxy.getSummary());
            final boolean restAtInit = env.getProperty("nao.motion.rest-at-init", Boolean.class, true);
            if (restAtInit) {
                log.info("Resting NAO {}...", avatarId);
                motionProxy.killAll();
                motionProxy.rest();
            }
            return motionProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " Motion at " + host + ":" + port, e);
        }
    }

    protected ALTextToSpeechProxy naoTts() throws IOException {
        try {
            log.info("Initializing {} TTS at {}:{}...", avatarId, host, port);
            final ALTextToSpeechProxy tts = new ALTextToSpeechProxy(host, port);
            tts.setVolume(1f);
            naoExecutor.submit(() -> tts.say("Us-suh-lum-moo-ah-lay-koom wa-roh-ma-tool-laah-ee wa-ba-roh-kah-tooh"));
            return tts;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " TextToSpeech at " + host + ":" + port, e);
        }
    }

    protected ALAudioDeviceProxy naoAudioDevice() throws IOException {
        try {
            log.info("Initializing {} AudioDevice at {}:{}...", avatarId, host, port);
            final ALAudioDeviceProxy audioDeviceProxy = new ALAudioDeviceProxy(host, port);
            final Integer volume = env.getProperty("nao.audio.volume", Integer.class, 80);
            if (volume != null) {
                log.info("Set {} audio output volume to {}...", avatarId, volume);
                audioDeviceProxy.setOutputVolume(volume);
            }
            return audioDeviceProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " AudioDevice at " + host + ":" + port, e);
        }
    }

    protected ALAudioPlayerProxy naoAudioPlayer() throws IOException {
        try {
            log.info("Initializing {} AudioPlayer at {}:{}...", avatarId, host, port);
            final ALAudioPlayerProxy audioPlayerProxy = new ALAudioPlayerProxy(host, port);
            return audioPlayerProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " AudioPlayer at " + host + ":" + port, e);
        }
    }

    protected ALAudioRecorderProxy naoAudioRecorder() throws IOException {
        try {
            log.info("Initializing {} AudioRecorder at {}:{}...", avatarId, host, port);
            final ALAudioRecorderProxy audioRecorderProxy = new ALAudioRecorderProxy(host, port);
            return audioRecorderProxy;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " AudioRecorder at " + host + ":" + port, e);
        }
    }

    protected ALRobotPostureProxy naoRobotPosture() throws IOException {
        try {
            log.info("Initializing {} RobotPosture at {}:{}...", avatarId, host, port);
            final ALRobotPostureProxy robotPosture = new ALRobotPostureProxy(host, port);
            log.info("{} RobotPosture {}: {}", avatarId, robotPosture.version(), robotPosture.getPostureList());
            return robotPosture;
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " RobotPosture at " + host + ":" + port, e);
        }
    }

    protected ALLedsProxy ledsProxy() throws IOException {
        try {
            log.info("Initializing {} LEDs at {}:{}...", avatarId, host, port);
            return new ALLedsProxy(host, port);
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " LEDs at " + host + ":" + port, e);
        }
    }

    protected ALBatteryProxy batteryProxy() throws IOException {
        try {
            log.info("Initializing {} Batteries at {}:{}...", avatarId, host, port);
            return new ALBatteryProxy(host, port);
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " Batteries at " + host + ":" + port, e);
        }
    }

    protected ALSonarProxy sonarProxy() throws IOException {
        try {
            log.info("Initializing {} Sonar at {}:{}...", avatarId, host, port);
            return new ALSonarProxy(host, port);
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " Sonar at " + host + ":" + port, e);
        }
    }

    protected ALRobotPoseProxy robotPoseProxy() throws IOException {
        try {
            log.info("Initializing {} Robot Pose at {}:{}...", avatarId, host, port);
            return new ALRobotPoseProxy(host, port);
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " Pose at " + host + ":" + port, e);
        }
    }

    protected ALVideoDeviceProxy naoVideoDevice() throws IOException {
        try {
            log.info("Initializing VideoDevice at {}:{}...", host, port);
            return new ALVideoDeviceProxy(host, port);
        } catch (Exception e) {
            throw new IOException("Cannot connect " + avatarId + " VideoDevice at " + host + ":" + port, e);
        }
    }

    public ALMotionProxy getMotion() {
        return motion;
    }

    public ALTextToSpeechProxy getTts() {
        return tts;
    }

    public ALAudioDeviceProxy getAudioDevice() {
        return audioDevice;
    }

    public ALAudioPlayerProxy getAudioPlayer() {
        return audioPlayer;
    }

    public ALAudioRecorderProxy getAudioRecorder() {
        return audioRecorder;
    }

    public ALBatteryProxy getBattery() {
        return battery;
    }

    public ALSonarProxy getSonar() {
        return sonar;
    }

    public ALVideoDeviceProxy getVideoDevice() {
        return videoDevice;
    }

    public ALRobotPostureProxy getRobotPosture() {
        return robotPosture;
    }

    public ALRobotPoseProxy getRobotPose() {
        return robotPose;
    }

    public ALLedsProxy getLeds() {
        return leds;
    }
}
