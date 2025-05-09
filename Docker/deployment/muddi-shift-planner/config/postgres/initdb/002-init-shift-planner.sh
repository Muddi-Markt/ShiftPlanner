#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    create user ${MUDDI_POSTGRES_USER} with password '${MUDDI_POSTGRES_PASSWORD}';
    create database ${MUDDI_POSTGRES_DB} with owner muddi;
EOSQL