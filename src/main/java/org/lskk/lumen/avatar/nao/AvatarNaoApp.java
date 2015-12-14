package org.lskk.lumen.avatar.nao;

import org.lskk.lumen.core.LumenCoreConfig;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.context.annotation.Import;
import org.springframework.context.annotation.Profile;

@SpringBootApplication
@Profile("avatarNaoApp")
@Import(LumenCoreConfig.class)
public class AvatarNaoApp implements CommandLineRunner {

    private static Logger log = LoggerFactory.getLogger(AvatarNaoApp.class);

    static {
        log.info("java.library.path = {}", System.getProperty("java.library.path"));
    }

    public static void main(String[] args) {
        new SpringApplicationBuilder(AvatarNaoApp.class)
                .profiles("avatarNaoApp", "nao")
                .run(args);
    }

//    @Inject
//    private LumenCamelConfiguration lumenCamelConfiguration;

    @Override
    public void run(String... args) throws Exception {
    }
}