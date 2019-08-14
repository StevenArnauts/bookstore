### Intro
This is a demo application to use for demonstrations and try out new things.

### Running
* Requires SQL Server (Express)
* Check the connection string in appsettings.json. The database and tables will be created automatically.
* To customize settings locally:
  * Don't modify appsettings.json, as you might affect others.
  * Instead, add a file appsettings.json.user, and overwrite anything you want. This file won't be checked in.

### Environments
* Set ASPNETCORE_ENVIRONMENT in launchSettings.json.
* Modify / Create the file appsettings.env.json for that environment


### To Do
* Unit of work? In EF Core disposing the DbContext doesn't seem to trigger SaveChanges any more?
* 
