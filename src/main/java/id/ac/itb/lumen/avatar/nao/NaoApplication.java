package id.ac.itb.lumen.avatar.nao;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.boot.builder.SpringApplicationBuilder;
import org.springframework.context.annotation.Profile;

@SpringBootApplication
@Profile("nao")
public class NaoApplication implements CommandLineRunner {

    private static Logger log = LoggerFactory.getLogger(NaoApplication.class);

    public static void main(String[] args) {
        new SpringApplicationBuilder(NaoApplication.class)
                .profiles("nao")
                .run(args);
    }

//    @Inject
//    private LumenCamelConfiguration lumenCamelConfiguration;

    @Override
    public void run(String... args) throws Exception {
        log.info("java.library.path={}", System.getProperty("java.library.path"));
    }
}
