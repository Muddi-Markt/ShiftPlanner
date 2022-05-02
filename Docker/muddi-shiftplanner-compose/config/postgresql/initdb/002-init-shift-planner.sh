#!/bin/bash
set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    create user muddi with password 'superDuperSecret';
    create database shift_planner with owner muddi;
EOSQL
