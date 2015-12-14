package org.lskk.lumen.avatar.nao;

import com.google.common.collect.ImmutableSet;
import com.google.common.util.concurrent.ListeningExecutorService;
import com.google.common.util.concurrent.MoreExecutors;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.env.Environment;

import javax.annotation.PostConstruct;
import javax.annotation.PreDestroy;
import javax.inject.Inject;
import java.io.IOException;
import java.util.Map;
import java.util.Optional;
import java.util.Set;
import java.util.concurrent.ConcurrentSkipListMap;
import java.util.concurrent.Executors;

/**
 * Created by ceefour on 20/04/2015.
 */
@Configuration
public class NaoConfig {

    private static Logger log = LoggerFactory.getLogger(NaoConfig.class);
    public static final int sessionTimeoutMs = 5000;
    public static final Set<String> NAO_AVATAR_IDS = ImmutableSet.of(
            "nao1", "nao2", "nao3", "nao4", "nao5", "nao6", "nao7", "nao8", "nao9");

    private final Map<String, NaoController> controllers = new ConcurrentSkipListMap<>();

    @Inject
    private Environment env;

    @Bean(destroyMethod = "shutdown")
    @ForNao
    public ListeningExecutorService naoExecutor() {
        return MoreExecutors.listeningDecorator(Executors.newSingleThreadExecutor());
    }

    @PostConstruct
    public void init() {
        log.info("Checking {} avatar configurations: {}", NAO_AVATAR_IDS.size(), NAO_AVATAR_IDS);
        NAO_AVATAR_IDS.stream().forEach(avatarId -> {
            final String host = env.getProperty(avatarId + ".host");
            final int port = env.getProperty(avatarId + ".port", Integer.class, 9559);
            if (host != null) {
                try {
                    final NaoController naoController = new NaoController(avatarId, host, port, env, naoExecutor());
                    naoController.init();
                    controllers.put(avatarId, naoController);
                } catch (Exception e) {
                    log.error("Cannot initialize avatar " + avatarId, e);
                }
            }
        });
        log.info("Initialized {} NAO avatars: {}", controllers.size(), controllers.keySet());
    }

    @PreDestroy
    public void destroy() {
        log.info("Shutting down {} NAO avatars: {}", controllers.size(), controllers.keySet());
        controllers.values().stream().forEach(controller -> {
            try {
                controller.destroy();
            } catch (IOException e) {
                log.error("Cannot shutdown avatar " + controller.getAvatarId(), e);
            }
        });
        controllers.clear();
    }

    public Set<String> getControllerAvatarIds() {
        return controllers.keySet();
    }

    public NaoController get(String avatarId) {
        return Optional.ofNullable(controllers.get(avatarId)).orElseThrow(
                () -> new NaoException("Cannot get avatar " + avatarId));
    }
}