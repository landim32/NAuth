#!/bin/bash
docker stop nauth-api1
docker rm nauth-api1
docker build -t nauth-api -f NAuth.API/Dockerfile .
docker run --name nauth-api1 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORTS=443 --network docker-network nauth-api &
docker ps