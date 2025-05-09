# build app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG TARGETARCH
WORKDIR /src
COPY ["Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj", "Muddi.ShiftPlanner.Client/"]
RUN dotnet restore "Muddi.ShiftPlanner.Client/Muddi.ShiftPlanner.Client.csproj"  -a $TARGETARCH
COPY . .
WORKDIR "/src/Muddi.ShiftPlanner.Client"
RUN dotnet clean "Muddi.ShiftPlanner.Client.csproj"
RUN dotnet build "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/build  -a $TARGETARCH

FROM build AS publish
ARG TARGETARCH
RUN dotnet publish "Muddi.ShiftPlanner.Client.csproj" -c Release -o /app/publish  -a $TARGETARCH

# build runtime image
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot/ .
COPY Muddi.ShiftPlanner.Client/nginx.conf /etc/nginx/nginx.conf