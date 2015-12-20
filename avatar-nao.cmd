@echo off
"C:\Program Files (x86)\Java\jdk1.8.0_66\bin\java" -Xms256m -Xmx256m -Djava.awt.headless=true -cp target/dependency/*;target/classes org.lskk.lumen.avatar.nao.AvatarNaoApp %*
