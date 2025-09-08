#!/bin/bash
cd ./Frontend/nauth-core
pwd
npm install --legacy-peer-deps
npm run build
rm -Rf ../nauth-app/src/lib/nauth-core
cp -Rf ./dist ../nauth-app/src/lib/nauth-core
