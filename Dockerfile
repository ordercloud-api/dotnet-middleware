# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
ENV PROJ_NAME=Customer.OrderCloud.Api
WORKDIR /app
COPY . .
RUN dotnet restore
RUN dotnet publish ./${PROJ_NAME}/${PROJ_NAME}.csproj -c Release -o /app/build

EXPOSE 5000

WORKDIR /app/build
CMD dotnet Customer.OrderCloud.Api.dll --urls "http://localhost:5000"
