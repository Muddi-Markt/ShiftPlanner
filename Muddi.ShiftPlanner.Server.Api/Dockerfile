FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Server.Api/Muddi.ShiftPlanner.Server.Api.csproj", "Muddi.ShiftPlanner.Server.Api/"]
COPY ["Muddi.ShiftPlanner.Server.Database/Muddi.ShiftPlanner.Server.Database.csproj", "Muddi.ShiftPlanner.Server.Database/"]
COPY ["Muddi.ShiftPlanner.Shared/Muddi.ShiftPlanner.Shared.csproj", "Muddi.ShiftPlanner.Shared/"]
RUN dotnet restore "Muddi.ShiftPlanner.Server.Api/Muddi.ShiftPlanner.Server.Api.csproj" -a $TARGETARCH
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Server.Api"
RUN dotnet build "Muddi.ShiftPlanner.Server.Api.csproj" -c Release -o /app/build -a $TARGETARCH

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Server.Api.csproj" -c Release -o /app/publish -a $TARGETARCH --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muddi.ShiftPlanner.Server.Api.dll"]
