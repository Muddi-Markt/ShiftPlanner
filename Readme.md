# Muddi's Shift Planner

## How to contribute

First install dotnet 7.0.201 sdk and the newest version of docker (which inclues docker compose) then:

```shell
cd Docker/muddi-shiftplanner-compose/
docker compose up
```

this will start up the services you need for development.

### Keycloak

In [keycloak admin console](http://localhost:28080/admin/) login with user `admin` and pass `admin`, click on master realm and
on `Add realm`. Then `Select file` and select `./Docker/muddi-shiftplanner-compose/config/keycloak/realm-export.json`. Then create a new user called `api-admin@muddimarkt.org` in the Muddi Realm, make sure you set 'E-Mail verified' and then set the Credentials (password) as `admin`. Also give the user under Role Mappings all roles for the shift-planner Client. 

In Role Mappings you can also set Client Roles for shift-planner (e.g. admin, editor, super-admin or viewer)
### Dotnet secrets

Also you have to init [dotnet secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-7.0&tabs=windows)
if you want to use the alerting service:

##### secrets.json:

```json
{
  "Telegram": {
    "ApiToken": "YOUR-API-TOKEN"
  }
}
```

### Building Images

To build just run the .sh files in ./Docker. You may have to change the tag.

### Running

In your IDE just run those 3 projects:

* Muddi.ShiftPlanner.Client
* Muddi.ShiftPlanner.Server.Api
* Muddi.ShiftPlanner.Services.Alerting
