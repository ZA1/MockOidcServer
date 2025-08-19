#!/usr/bin/env bash
podman buildx build \
  --platform=linux/amd64,linux/arm64 \
  -t za001/mock-oidc-server:0.0.4 \
  -t za001/mock-oidc-server:latest \
  -f MockOidcServer/Dockerfile .
  