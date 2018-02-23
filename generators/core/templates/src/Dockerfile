FROM microsoft/dotnet:2.0.5-sdk-2.1.4-stretch AS build-env

LABEL <%= username %> <<%= email %>>
LABEL Description="This images is used to start the <%= appname %> microservice" Vendor="MicroCore" Version="1.0"

WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM microsoft/dotnet:2.0.5-runtime-stretch
WORKDIR /app
COPY --from=build-env /app/out ./
EXPOSE <%= portNumber %>/tcp
ENTRYPOINT ["dotnet", "out/<%= namespace %>.dll"]