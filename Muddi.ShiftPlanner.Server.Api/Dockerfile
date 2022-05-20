FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Server.Api/Muddi.ShiftPlanner.Server.Api.csproj", "Muddi.ShiftPlanner.Server.Api/"]
COPY ["Muddi.ShiftPlanner.Server.Database/Muddi.ShiftPlanner.Server.Database.csproj", "Muddi.ShiftPlanner.Server.Database/"]
COPY ["Muddi.ShiftPlanner.Shared/Muddi.ShiftPlanner.Shared.csproj", "Muddi.ShiftPlanner.Shared/"]
RUN dotnet restore "Muddi.ShiftPlanner.Server.Api/Muddi.ShiftPlanner.Server.Api.csproj"
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Server.Api"
RUN dotnet build "Muddi.ShiftPlanner.Server.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Server.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muddi.ShiftPlanner.Server.Api.dll"]
