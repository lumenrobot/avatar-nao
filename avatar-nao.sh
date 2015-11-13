#!/bin/bash
SCRIPT_DIR="$(dirname $0)"
java -Xms512m -Xmx512m -cp $SCRIPT_DIR'/target/dependency/*':$SCRIPT_DIR'/target/classes' org.lskk.lumen.avatar.nao.AvatarNaoApp "$@"
