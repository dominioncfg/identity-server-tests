Create Container & Run:
    docker build -t customidp -f TestIdentityServer/Dockerfile .
    docker run --name CustomIdp -p 32250:443  -v "C:\Users\josec\AppData\Roaming\Microsoft\UserSecrets:/root/.microsoft/usersecrets:ro" -v "C:\Users\josec\AppData\Roaming\ASP.NET\Https:/root/.aspnet/https:ro" -e "ASPNETCORE_ENVIRONMENT=Development" -e "ASPNETCORE_URLS=https://+:443;http://+:80" customidp

Migrate Identity Server
    dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration --project TestIdentityServer  -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
    dotnet ef migrations add InitialIdentityServerConfigurationDbMigration --project TestIdentityServer -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb

Docker Compose
    docker-compose -f "C:\Users\josec\source\repos\TestIdentityServer\docker-compose.yml" -f "C:\Users\josec\source\repos\TestIdentityServer\docker-compose.local.override.yml" down
    docker-compose -f "C:\Users\josec\source\repos\TestIdentityServer\docker-compose.yml" -f "C:\Users\josec\source\repos\TestIdentityServer\docker-compose.local.override.yml" up --build