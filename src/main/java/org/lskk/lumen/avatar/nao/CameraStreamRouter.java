package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALVideoDeviceProxy;
import com.aldebaran.proxy.Variant;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.HexDump;
import org.bytedeco.javacpp.BytePointer;
import org.bytedeco.javacpp.opencv_core;
import org.bytedeco.javacpp.opencv_highgui;
import org.bytedeco.javacpp.opencv_imgproc;
import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.lskk.lumen.core.ImageObject;
import org.lskk.lumen.core.util.AsError;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.imageio.ImageIO;
import javax.inject.Inject;
import java.awt.image.BufferedImage;
import java.io.ByteArrayOutputStream;

/**
 * Created by ceefour on 10/2/15.
 */
@Component
public class CameraStreamRouter extends RouteBuilder {

    private static final Logger log = LoggerFactory.getLogger(CameraStreamRouter.class);

    @Inject
    private ToJson toJson;
    @Inject
    private AsError asError;
    @Inject
    private ALVideoDeviceProxy videoDevice;
    @Inject
    private NaoVideoConfig naoVideoConfig;
    @Inject
    private ProducerTemplate producer;

    protected byte[] yuv422ToJpg(byte[] topImg) {
        // http://study.marearts.com/2014/12/yuyv-to-rgb-and-rgb-to-yuyv-using.html
        final byte[] topBytes;
        final opencv_core.Mat yuv422Mat = new opencv_core.Mat(naoVideoConfig.getResolution().getHeight(), naoVideoConfig.getResolution().getWidth(),
                opencv_core.CV_8UC2);
        final opencv_core.Mat bgrMat;
        try {
            yuv422Mat.ptr().put(topImg);
            bgrMat = new opencv_core.Mat(naoVideoConfig.getResolution().getHeight(), naoVideoConfig.getResolution().getWidth(),
                    opencv_core.CV_8UC3);
            try {
                opencv_imgproc.cvtColor(yuv422Mat, bgrMat, opencv_imgproc.CV_YUV2BGR_YUYV);
                final BytePointer bufPtr = new BytePointer();
                try {
                    opencv_highgui.imencode(".jpg", bgrMat, bufPtr);
                    log.trace("JPEG Image: {} bytes", bufPtr.capacity());
                    topBytes = new byte[bufPtr.capacity()];
                    bufPtr.get(topBytes);
                    return topBytes;
                } finally {
                    bufPtr.deallocate();
                }
            } finally {
                bgrMat.release();
            }
        } finally {
            yuv422Mat.release();
        }
    }

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final int period = 1000 / naoVideoConfig.getCameraFps();
        from("timer:camera?period=" + period)
                .process(exchange -> {
                    final byte[] topYuv422;
                    final int topId;
                    final byte[] bottomYuv422;
                    final int bottomId;

                    log.trace("Getting image remote '{}' '{}' ...", NaoVideoConfig.GVM_TOP_ID, NaoVideoConfig.GVM_BOTTOM_ID);
                    // grab bottom camera first so we don't have to "reset" active camera after this
                    videoDevice.setActiveCamera(1); // workaround!
                    final Variant bottomImageRemoteVariant = videoDevice.getImageRemote(NaoVideoConfig.GVM_TOP_ID); // workaround!
                    try {
                        //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                        bottomYuv422 = bottomImageRemoteVariant.getElement(6).toBinary();
                        bottomId = bottomImageRemoteVariant.getElement(7).toInt();
                    } finally {
//                        videoDevice.releaseImage(NaoVideoConfig.GVM_BOTTOM_ID);
                        videoDevice.releaseImage(NaoVideoConfig.GVM_TOP_ID);
                    }
                    videoDevice.setActiveCamera(0);
                    final Variant topImageRemoteVariant = videoDevice.getImageRemote(NaoVideoConfig.GVM_TOP_ID);
                    try {
                        //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                        topYuv422 = topImageRemoteVariant.getElement(6).toBinary();
                        topId = topImageRemoteVariant.getElement(7).toInt();
                    } finally {
//                        videoDevice.releaseImage(NaoVideoConfig.GVM_BOTTOM_ID);
                        videoDevice.releaseImage(NaoVideoConfig.GVM_TOP_ID);
                    }
                    log.debug("Image {}/{}={} bytes, {}/{}={} bytes", NaoVideoConfig.GVM_TOP_ID, topId, topYuv422.length,
                            NaoVideoConfig.GVM_BOTTOM_ID, bottomId, bottomYuv422.length);

                    // process TOP Image
                    if (log.isTraceEnabled()) {
                        final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                        HexDump.dump(topYuv422, 0, bos, 0);
                        log.trace("{}", bos);
                    }
                    final byte[] topJpg = yuv422ToJpg(topYuv422);

                    final ImageObject topImageObject = new ImageObject();
                    topImageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                    topImageObject.setContentType("image/jpeg");
                    topImageObject.setContentSize((long) topJpg.length);
                    topImageObject.setContentUrl("data:image/jpeg;base64," +
                            Base64.encodeBase64String(topJpg));

//                        exchange.getIn().setBody(topImageObject);
                    producer.sendBody(
                            "rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main",
                            toJson.apply(topImageObject));

                    // process BOTTOM Image
                    if (log.isTraceEnabled()) {
                        final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                        HexDump.dump(bottomYuv422, 0, bos, 0);
                        log.trace("{}", bos);
                    }
                    final byte[] bottomJpg = yuv422ToJpg(bottomYuv422);

                    final ImageObject bottomImageObject = new ImageObject();
                    bottomImageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                    bottomImageObject.setContentType("image/jpeg");
                    bottomImageObject.setContentSize((long) bottomJpg.length);
                    bottomImageObject.setContentUrl("data:image/jpeg;base64," +
                            Base64.encodeBase64String(bottomJpg));
                    //exchange.getIn().setBody(bottomImageObject);
                    producer.sendBody(
                            "rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.bottom",
                            toJson.apply(bottomImageObject));

                    log.debug("Sent JPG {}={} bytes {}={} bytes", NaoVideoConfig.GVM_TOP_ID, topJpg.length,
                            NaoVideoConfig.GVM_BOTTOM_ID, bottomJpg.length);

                });
//                .bean(toJson)
//                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main");
        //.to("log:OUT.avatar.nao1.camera.main?showHeaders=true&showAll=true&multiline=true");
    }
}
