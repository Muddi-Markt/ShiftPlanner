FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj", "Muddi.ShiftPlanner.Client/"]
RUN dotnet restore "Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj"
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Client"
RUN dotnet build "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Muddi.ShiftPlanner.Client.dll"]
