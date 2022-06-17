#!/bin/bash
set -e

cd ..
current_timestamp=`date +%s`
#sed -i "s/cacheVersion\ =\ '.*'/cacheVersion = '${current_timestamp}'/g" Muddi.ShiftPlanner.Client/wwwroot/service-worker.published.js
docker build -f Muddi.ShiftPlanner.Services.Alerting/Dockerfile -t hse-server01:5000/muddi/shiftplanner/alerting-service -t muddi/shiftplanner/alerting-service . --network=host
docker push hse-server01:5000/muddi/shiftplanner/alerting-service
#docker save -o muddi-client.tar hse-server01:5000/muddi/shiftplanner/client