#!/bin/bash

rm -rf obj
rm -rf bin
rm -rf gen/Google

# Build with source
dotnet build -c Release -p:EmbedSource=true Scailo.Sdk.csproj
dotnet pack -c Release -p:EmbedSource=true Scailo.Sdk.csproj