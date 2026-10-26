#!/bin/sh
set -eu

: "${SHIFT_PLANNER_KEYCLOAK_URL:?not set - e.g. https://keycloak.example.com}"
: "${SHIFT_PLANNER_KEYCLOAK_REALM:?not set - e.g. muddi}"
: "${SHIFT_PLANNER_API_URL:?not set - e.g. https://api.example.com}"

envsubst < /appsettings.template.json > /usr/share/nginx/html/appsettings.json
echo "rendered /usr/share/nginx/html/appsettings.json"

exec "$@"
