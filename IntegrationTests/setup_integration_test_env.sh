#!/bin/bash

if [ "$BASH_SOURCE" == "$0" ]; then
    echo -e "Run 'source setup_integration_test_env.sh'\n Script must be sourced!"
    exit 1
fi

python -m venv venv
source venv/bin/activate
pip install -r requirements.txt