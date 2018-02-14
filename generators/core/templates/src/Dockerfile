FROM microsoft/dotnet:1.1.1-sdk

LABEL <%= username %> <<%= email %>>
LABEL Description="This images is used to start the <%= appname %> microservice" Vendor="MicroCore" Version="1.0"

COPY . /build
WORKDIR /build
RUN dotnet restore
RUN dotnet publish -c Release -o out

EXPOSE <%= portNumber %>/tcp
ENTRYPOINT ["dotnet", "out/<%= namespace %>.dll"]
