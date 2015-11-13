package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALVideoDeviceProxy;
import com.aldebaran.proxy.Variant;
import org.apache.camel.ProducerTemplate;
import org.apache.camel.builder.LoggingErrorHandlerBuilder;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.HexDump;
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

    @Override
    public void configure() throws Exception {
        onException(Exception.class).bean(asError).bean(toJson).handled(true);
        errorHandler(new LoggingErrorHandlerBuilder(log));
        final int period = 1000 / NaoVideoConfig.CAMERA_FPS;
        from("timer:camera?period=" + period)
                .process(exchange -> {
                    final byte[] topImg;
                    final int topId;
                    final byte[] bottomImg;
                    final int bottomId;

                    log.trace("Getting image remote '{}' '{}' ...", NaoVideoConfig.GVM_TOP_ID, NaoVideoConfig.GVM_BOTTOM_ID);
                    // grab bottom camera first so we don't have to "reset" active camera after this
                    videoDevice.setActiveCamera(1); // workaround!
                    final Variant bottomImageRemoteVariant = videoDevice.getImageRemote(NaoVideoConfig.GVM_TOP_ID); // workaround!
                    try {
                        //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                        bottomImg = bottomImageRemoteVariant.getElement(6).toBinary();
                        bottomId = bottomImageRemoteVariant.getElement(7).toInt();
                    } finally {
//                        videoDevice.releaseImage(NaoVideoConfig.GVM_BOTTOM_ID);
                        videoDevice.releaseImage(NaoVideoConfig.GVM_TOP_ID);
                    }
                    videoDevice.setActiveCamera(0);
                    final Variant topImageRemoteVariant = videoDevice.getImageRemote(NaoVideoConfig.GVM_TOP_ID);
                    try {
                        //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                        topImg = topImageRemoteVariant.getElement(6).toBinary();
                        topId = topImageRemoteVariant.getElement(7).toInt();
                    } finally {
//                        videoDevice.releaseImage(NaoVideoConfig.GVM_BOTTOM_ID);
                        videoDevice.releaseImage(NaoVideoConfig.GVM_TOP_ID);
                    }
                    log.trace("Image {}/{}={} bytes, {}/{}={} bytes", NaoVideoConfig.GVM_TOP_ID, topId, topImg.length,
                            NaoVideoConfig.GVM_BOTTOM_ID, bottomId, bottomImg.length);

                    // process TOP Image
                    if (log.isTraceEnabled()) {
                        final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                        HexDump.dump(topImg, 0, bos, 0);
                        log.trace("{}", bos);
                    }
                    final YUV422Image yuv422Image = new YUV422Image(topImg,
                            naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight());
//                        final BufferedImage bufImg = new BufferedImage(
//                                naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight(),
//                                BufferedImage.TYPE_3BYTE_BGR);
                    final BufferedImage bufImg = new BufferedImage(
                            naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight(),
                            BufferedImage.TYPE_INT_RGB);
                    yuv422Image.initImageRgbInt(bufImg);
//                        final byte[] bufImgData = ((DataBufferByte) bufImg.getRaster().getDataBuffer()).getData();
                    //System.arraycopy(imageRemote, 0, bufImgData, 0, imageRemote.length);

                    final ImageObject topImageObject = new ImageObject();
                    try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                        ImageIO.write(bufImg, "jpg", baos);
                        final byte[] bytes = baos.toByteArray();
                        topImageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                        topImageObject.setContentType("image/jpeg");
                        topImageObject.setContentSize((long) bytes.length);
                        topImageObject.setContentUrl("data:image/jpeg;base64," +
                                Base64.encodeBase64String(bytes));
                    }
//                        exchange.getIn().setBody(topImageObject);
                    producer.sendBody(
                            "rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main",
                            toJson.apply(topImageObject));

                    // process BOTTOM Image
                    if (log.isTraceEnabled()) {
                        final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                        HexDump.dump(bottomImg, 0, bos, 0);
                        log.trace("{}", bos);
                    }
                    final YUV422Image yuv422BottomImage = new YUV422Image(bottomImg,
                            naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight());
//                        final BufferedImage bufImg = new BufferedImage(
//                                naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight(),
//                                BufferedImage.TYPE_3BYTE_BGR);
                    final BufferedImage bufBottomImg = new BufferedImage(
                            naoVideoConfig.getResolution().getWidth(), naoVideoConfig.getResolution().getHeight(),
                            BufferedImage.TYPE_INT_RGB);
                    yuv422BottomImage.initImageRgbInt(bufBottomImg);
//                        final byte[] bufImgData = ((DataBufferByte) bufImg.getRaster().getDataBuffer()).getData();
                    //System.arraycopy(imageRemote, 0, bufImgData, 0, imageRemote.length);

                    final ImageObject bottomImageObject = new ImageObject();
                    try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                        ImageIO.write(bufBottomImg, "jpg", baos);
                        final byte[] bytes = baos.toByteArray();
                        bottomImageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                        bottomImageObject.setContentType("image/jpeg");
                        bottomImageObject.setContentSize((long) bytes.length);
                        bottomImageObject.setContentUrl("data:image/jpeg;base64," +
                                Base64.encodeBase64String(bytes));
                    }
                    //exchange.getIn().setBody(bottomImageObject);
                    producer.sendBody(
                            "rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.bottom",
                            toJson.apply(bottomImageObject));
                });
//                .bean(toJson)
//                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main");
        //.to("log:OUT.avatar.nao1.camera.main?showHeaders=true&showAll=true&multiline=true");
    }
}
