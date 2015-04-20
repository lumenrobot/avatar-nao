# avatar-nao
Avatar node implementation for NAO Robot by Aldebaran, planned successor of https://github.com/lumenitb/NaoServer

## jnaoqi

On Windows, you **must** [use the 32-bit JDK on Windows 64-bit](https://github.com/aldebaran/jnaoqi/issues/1). 

## Hendy's note: JAR files

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
