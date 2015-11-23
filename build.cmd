@rem we don't build the WAR because deploying a compressed WAR takes too much time
@rem better to build a standard stuff
set JAVA_HOME=C:\Program Files (x86)\Java\jdk1.8.0_66
mvn -DskipTests -DincludeScope=runtime clean dependency:copy-dependencies install
