#!/bin/bash

cd ..
docker build -f Muddi.ShiftPlanner.Server.Api/Dockerfile -t hse-server01:5000/muddi/shiftplanner/api -t muddi/shiftplanner/api .
docker push hse-server01:5000/muddi/shiftplanner/api
