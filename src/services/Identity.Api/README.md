# Identity.Api project
This project will provide authentication mechanism to the solution.

## Database Setup
The application uses SQL Server to keep user accounts.

### Development Environment
You can use Sql Server in a docker container. To make your life easier you can use the project [dev-tools](https://github.com/ppolyzos/dev-tools)
to easily setup different databases.

### First Usage
1. Start sql server in a docker container by running `docker-compose up` in `dbs/sql-server/` folder.
2. Create `Identity.Api` database using:
   ```
   CREATE DATABASE [Identity.Api];
   ```
3. Update in `appsettings.development.json` the connection string with the following:
   ```
   "IdentityDataContextConnection": "Server=localhost;Database=Identity.Api;User Id=sa;Password=<YourStrong@Passw0rd>;"
   ```
4. Run ef migrations from project `Identity.Api` root folder: 
   ```
   cd event-management\src\services\Identity.API> 
   > dotnet ef database update
   ```
   The project will build and the ef migrations will be applied and authentication related tables will be available.

5. Seed database with sample users / roles / etc. by setting `Data:Seed` to `true` property in `appsettings.json` file. 
   In the first run default users will be created and stored in the appropriate tables.

### Re-Create Database
If you want to re-create the database you can do that using:
```
# Sql Query
USE master;
ALTER DATABASE [Identity.Api] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
DROP DATABASE [Identity.Api] ;

CREATE DATABASE [Identity.Api];

# EF migrations
> dotnet ef database update

# Run the application with Seed option set to true
```
