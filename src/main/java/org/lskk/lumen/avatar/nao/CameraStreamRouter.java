package org.lskk.lumen.avatar.nao;

import com.aldebaran.proxy.ALVideoDeviceProxy;
import com.aldebaran.proxy.Variant;
import org.apache.camel.builder.RouteBuilder;
import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.HexDump;
import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.lskk.lumen.core.ImageObject;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Component;

import javax.imageio.ImageIO;
import javax.inject.Inject;
import java.awt.image.BufferedImage;
import java.awt.image.DataBufferByte;
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
    private ALVideoDeviceProxy videoDevice;
    @Inject
    private NaoVideoConfig naoVideoConfig;

    @Override
    public void configure() throws Exception {
        final int period = 1000 / NaoVideoConfig.CAMERA_FPS;
        from("timer:camera?period=" + period)
                .process(exchange -> {
                    try {
                        log.trace("Getting image remote '{}' ...", NaoVideoConfig.GVM_ID);
                        final Variant imageRemoteVariant = videoDevice.getImageRemote(NaoVideoConfig.GVM_ID);
                        //log.info("Image remote variant: {} size", imageRemoteVariant.getSize());
                        final byte[] imageRemote = imageRemoteVariant.getElement(6).toBinary();
                        log.trace("Image '{}': {} bytes", NaoVideoConfig.GVM_ID, imageRemote.length);
                        if (log.isTraceEnabled()) {
                            final ByteArrayOutputStream bos = new ByteArrayOutputStream();
                            HexDump.dump(imageRemote, 0, bos, 0);
                            log.trace("{}", bos);
                        }

                        final YUV422Image yuv422Image = new YUV422Image(imageRemote,
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

                        final ImageObject imageObject = new ImageObject();
                        try (ByteArrayOutputStream baos = new ByteArrayOutputStream()) {
                            ImageIO.write(bufImg, "jpg", baos);
                            final byte[] bytes = baos.toByteArray();
                            imageObject.setDateCreated(new DateTime(DateTimeZone.forID("Asia/Jakarta")));
                            imageObject.setContentType("image/jpeg");
                            imageObject.setContentSize((long) bytes.length);
                            imageObject.setContentUrl("data:image/jpeg;base64," +
                                    Base64.encodeBase64String(bytes));
                        }
                        exchange.getIn().setBody(imageObject);
                    } finally {
                        videoDevice.releaseImage(NaoVideoConfig.GVM_ID);
                    }
                })
                .bean(toJson)
                .to("rabbitmq://localhost/amq.topic?connectionFactory=#amqpConnFactory&exchangeType=topic&autoDelete=false&routingKey=avatar.nao1.camera.main");
        //.to("log:OUT.avatar.nao1.camera.main?showHeaders=true&showAll=true&multiline=true");
    }
}
