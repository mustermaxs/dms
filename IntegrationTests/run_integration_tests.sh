#!/bin/bash

if [[ -z "$VIRTUAL_ENV" ]]; then
    echo -e "Run 'source setup_integration_test_env.sh'\nThen run this script"
    exit 1
fi

./venv/bin/python ./main.py

