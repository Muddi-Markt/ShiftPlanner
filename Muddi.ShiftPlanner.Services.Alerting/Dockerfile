FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Services.Alerting/Muddi.ShiftPlanner.Services.Alerting.csproj", "Muddi.ShiftPlanner.Services.Alerting/"]
COPY ["Muddi.ShiftPlanner.Shared/Muddi.ShiftPlanner.Shared.csproj", "Muddi.ShiftPlanner.Shared/"]
RUN dotnet restore "Muddi.ShiftPlanner.Services.Alerting/Muddi.ShiftPlanner.Services.Alerting.csproj"
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Services.Alerting"
RUN dotnet build "Muddi.ShiftPlanner.Services.Alerting.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Services.Alerting.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muddi.ShiftPlanner.Services.Alerting.dll"]
