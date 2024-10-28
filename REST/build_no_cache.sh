#!/bin/bash

if sudo docker compose build ; then
    gotify push "Successfully built"
else
    gotify push "Failed to build"
fi
