### Intro
This is a demo application to use for demonstrations and try out new things.

### Running
* Requires ProgreSQL 10 (https://www.postgresql.org/download/windows/)
* Check the connection string in appsettings.json. The database and tables will be created automatically.
* To customize settings locally:
  * Don't modify appsettings.json, as you might affect others.
  * Instead, add a file appsettings.json.user, and overwrite anything you want. This file won't be checked in.

### Environments
* Set ASPNETCORE_ENVIRONMENT in launchSettings.json.
* Modify / Create the file appsettings.env.json for that environment


### To Do
* Unit of work? In EF Core disposing the DbContext doesn't seem to trigger SaveChanges any more?
* Add explicit EF mappings (1 per type)
* Split customercontroller (or do it in the training)


<br/><br/>
## Running on MacOS with PostgreSQL and VS Code

### Install PostgreSQL and Postico
On MacOS, a great and simple PostgreSQL implementation is Postgres.app together with Postico client.
* Download Postgres.app from https://postgresapp.com/downloads.html  
Move to Applications folder and open it.  
Click "Initialize" to create a new server  
You now have a PostgreSQL server running on your Mac with default settings:

Setting | Default
--- | --- 
Host	         |localhost
Port	         |5432
User	         |your system user name
Database	      |same as user
Password	      |none
Connection URL	|postgresql://localhost

* Download Postico from https://eggerapps.at/postico/  
  Move to Applications folder and open it.


### Prepare database
In Postico, click "+Database" and name it bookstore.  
Open menu Navigate | Go to Terminal.  
Paste the following commands:
```
create user pencil42 with encrypted password 'xxxxxxxx';
grant all privileges on database bookstore to pencil42;
```
Note: Adjust password as necessary (must match the one used in the connection string in appsettings.dev.json)
Note: More fine-grained privileges may be appropriate, to look into.
Note: Do the same for the 'identity' database (except creating the user)

### Server
* Make sure you have .Net 2.2 SDK installed.
* Make sure you trust the bookstore ca !!!
* Clone and open project folder in VS Code.
  ```
  git clone https://github.com/StevenArnauts/bookstore.git
  cd bookstore
  code .
  ```
* When asked about unresolved dependencies or required assets to build and debug, click yes :)

* Go to Debug, click Start Debugging (the green arrow), select .Net Core and "Server" project.

* In launch.json, set  
  ``` 
  "ASPNETCORE_ENVIRONMENT": "dev"
  ```

* Click Start Debugging again.  
  This should cause the DB to be initialized and seeded.  
  Go check in Postico.

#### Running both Identity and Server at the same time
This is a bit complicated :)  
For now, see launch.json.sample and tasks.json.sample - these are copies of the files in .vscode folder.  
Specifically, look at the ASPNETCORE_URLS and the "compoounds" section in launch.json.sample.  
For some background cf e.g. https://elanderson.net/2018/04/run-multiple-projects-in-visual-studio-code/


### Alternative

Launch settings are considered to be a local thing, and are therefore excluded git in .gitignore. However, without Visual Studio 
it's convenient to start from an existing file. Launch settings must be in a file called launchSettings.json and must be in a Properties 
subfolder. 
#### Server Launch Settings
Copy the file Configuration\server.json to Server\Properties\launchSettings.json

#### Identity Launch Settings
Copy the file Configuration\identity.json to Identity\Properties\launchSettings.json

#### Run Identity Server from the command line
When debugging Identity Server simultaneously is not needed, an alternative approach is to run the project 
simply from the command line with the following command

```
Identity>dotnet run
```
#### Run Server from Visual Studio (Code)
Press F5, and choose Server as the project to run and debug. Or modify .vscode\launch.json.
VS Code will add this section to launch.json
```
"env": {
    "ASPNETCORE_ENVIRONMENT": "Development"
}
```
Either remove it, or change the environment to "dev"

#### Debug
VS Code will by default open http://0.0.0.0:6001, but you should use the HTTPS urls https://localhost:6101 and https://localhost:6103. 
