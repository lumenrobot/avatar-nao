package org.lskk.lumen.avatar.nao;

/**
 * Created by ceefour on 03/10/2015.
 */
public class NaoException extends RuntimeException {

    public NaoException() {
    }

    public NaoException(String message) {
        super(message);
    }

    public NaoException(String message, Throwable cause) {
        super(message, cause);
    }

    public NaoException(Throwable cause) {
        super(cause);
    }

    public NaoException(Throwable cause, String format, Object... args) {
        super(String.format(format, args), cause);
    }

    public NaoException(String message, Throwable cause, boolean enableSuppression, boolean writableStackTrace) {
        super(message, cause, enableSuppression, writableStackTrace);
    }
}
