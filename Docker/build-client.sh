#!/bin/bash

cd ..
current_timestamp=`date +%s`
sed -i "s/cacheVersion\ =\ '.*'/cacheVersion = '${current_timestamp}'/g" Muddi.ShiftPlanner.Client/wwwroot/service-worker.published.js
docker build -f Muddi.ShiftPlanner.Client/Dockerfile -t hse-server01:5000/muddi/shiftplanner/client -t muddi/shiftplanner/client .
docker push hse-server01:5000/muddi/shiftplanner/client
#docker save -o muddi-client.tar hse-server01:5000/muddi/shiftplanner/client

