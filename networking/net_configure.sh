#!/usr/bin/bash

mkdir ./logs/ 

if ! hash python3; then
    echo "[!] ERR: python is not installed"
    exit 1
fi

ver=$(python3 -V 2>&1 | sed 's/.* \([0-9]\).\([0-9]\).*/\1\2/')
if [ "$ver" -lt "31" ]; then
    echo "[!] ERR: This script requires python 3.10 or greater"
    exit 1
fi
