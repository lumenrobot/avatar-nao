package org.lskk.lumen.avatar.nao;

/**
 * Created by ceefour on 05/10/2015.
 * @see <a href="http://doc.aldebaran.com/1-14/naoqi/vision/alvideodevice.html">ALVideoDevice</a>
 */
public enum ImageResolution {
    /**
     * 160x120. 30 fps YUV422 over WiFi g.
     */
    QQVGA(160, 120, 0),
    /**
     * 320x240. 11 fps YUV422 over WiFi g.
     */
    QVGA(320, 240, 1),
    /**
     * 640x480. 2.5 fps YUV422 over WiFi g.
     */
    VGA(640, 480, 2),
    /**
     * 1280x960. 0.5 fps YUV422 over WiFi g.
     */
    FOURVGA(1280, 960, 4);

    int width;
    int height;
    int naoParameterId;

    ImageResolution(int width, int height, int naoParameterId) {
        this.width = width;
        this.height = height;
        this.naoParameterId = naoParameterId;
    }

    public int getWidth() {
        return width;
    }

    public int getHeight() {
        return height;
    }

    public int getNaoParameterId() {
        return naoParameterId;
    }

    @Override
    public String toString() {
        return "ImageResolution{" +
                "width=" + width +
                ", height=" + height +
                ", naoParameterId=" + naoParameterId +
                '}';
    }
}
