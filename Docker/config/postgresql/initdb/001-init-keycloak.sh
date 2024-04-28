#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    create user keycloak with password 'superDuperSecret';
    create database keycloak with owner keycloak;
EOSQL
