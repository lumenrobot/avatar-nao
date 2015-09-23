# avatar-nao
Avatar node implementation for NAO Robot by Aldebaran, planned successor of https://github.com/lumenitb/NaoServer

## jnaoqi

Due to **permanent** requirement of 32-bit JDK for jnaoqi, we can't use OpenCV (64-bit) and for convenience
will just use JDK standard ImageIO.

### Windows 64-bit - Requires 32-bit Oracle JDK

On Windows 64-bit, you **must** [use the 32-bit JDK](https://github.com/aldebaran/jnaoqi/issues/1).
This is for both jnaoqi 1.14.x and 2.x.

We currently use jnaoqi v1.14.5, please copy `lib_win32/jnaoqi.dll` to `C:\ProgramData\Oracle\Java\javapath`

If you get this error:

    Caused by: java.lang.UnsatisfiedLinkError: C:\ProgramData\Oracle\Java\javapath\jnaoqi.dll: Can't load IA 32-bit .dll on a AMD 64-bit platform
    
make sure you run `AvatarNaoApp` using 32-bit JDK.
 
### Linux 64-bit - Can use 64-bit Oracle JDK

## Hendy's note: JAR and DLL files for jnaoqi v1.x

The jnaoqi JARs and DLLs was available in [Aldebaran Community Archives](https://community.aldebaran-robotics.com/resources/archives/) (not anymore)
or offline at Hendy's passport: `I:\project_passport\lumen\nao\java`

To upload the jnaoqi JAR + source JAR to Soluvas Thirdparty Maven repository:

```bash
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=lib/jnaoqi-1.14.5.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=1.14.5
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-1.14.5-sources.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=1.14.5 -Dclassifier=sources
```

## Hendy's note: JAR files for jnaoqi v2.x

jnaoqi JARs are sourced from https://github.com/aldebaran/jnaoqi
(or, offline at `I:\project_passport\lumen\nao\java`),
first you zip up the javadocs directory into `jnaoqi-2.1.0.19-javadoc.jar`,
then deployed all files to Soluvas Thirdparty Maven repository using:

(the different platform JARs are separated using classifier so we can just use a single javadoc file)

```bash
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-android-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=android
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-atom-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=atom
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-linux32-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=linux32
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-linux64-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=linux64
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-mac64-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=mac64
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-master/jars/jnaoqi-windows32-2.1.0.19.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=windows32
mvn deploy:deploy-file -DrepositoryId=soluvas-public-thirdparty -Durl=http://nexus.bippo.co.id/nexus/content/repositories/soluvas-public-thirdparty/ -Dfile=jnaoqi-2.1.0.19-javadoc.jar -Dpackaging=jar -DgroupId=com.aldebaran -DartifactId=jnaoqi -Dversion=2.1.0.19 -Dclassifier=javadoc
```
