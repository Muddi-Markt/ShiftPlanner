#!/bin/bash

cd ..
docker build -f Muddi.ShiftPlanner.Client/Dockerfile -t hse-server01:5000/muddi/shiftplanner/client -t muddi/shiftplanner/client .
docker push hse-server01:5000/muddi/shiftplanner/client
