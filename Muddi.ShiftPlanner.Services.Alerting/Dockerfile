FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Services.Alerting/Muddi.ShiftPlanner.Services.Alerting.csproj", "Muddi.ShiftPlanner.Services.Alerting/"]
COPY ["Muddi.ShiftPlanner.Shared/Muddi.ShiftPlanner.Shared.csproj", "Muddi.ShiftPlanner.Shared/"]
RUN dotnet restore "Muddi.ShiftPlanner.Services.Alerting/Muddi.ShiftPlanner.Services.Alerting.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Services.Alerting"
RUN dotnet build "Muddi.ShiftPlanner.Services.Alerting.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Services.Alerting.csproj" -c Release -o /app/publish -a $TARGETARCH --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muddi.ShiftPlanner.Services.Alerting.dll"]
