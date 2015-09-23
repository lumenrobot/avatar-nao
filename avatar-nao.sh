#!/bin/bash
SCRIPT_DIR="$(dirname $0)"
java -cp $SCRIPT_DIR'/target/dependency/*':$SCRIPT_DIR'/target/classes' id.ac.itb.lumen.avatar.nao.AvatarNaoApp "$@"
