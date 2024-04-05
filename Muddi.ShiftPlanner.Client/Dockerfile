# build app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj", "Muddi.ShiftPlanner.Client/"]
RUN dotnet restore "Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj"
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Client"
RUN dotnet clean "Muddi.ShiftPlanner.Client.csproj"
RUN dotnet build "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/publish

# build runtime image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY Muddi.ShiftPlanner.Client/nginx.conf /etc/nginx/nginx.conf