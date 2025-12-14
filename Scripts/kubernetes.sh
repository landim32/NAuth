#!/bin/bash
pwd
cd ../Backend/NAuth
docker build -t emagine/nauth:local -f NAuth.API/Dockerfile .
kind load docker-image emagine/nauth:local --name rancher-local
cd NAuth.API
kubectl apply -f kubernetes.yaml
kubectl -n apps get pods,svc,ingress