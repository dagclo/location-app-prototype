FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["rest-api.csproj", ""]
RUN dotnet restore "rest-api.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "rest-api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "rest-api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "rest-api.dll"]