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
* Add explicit EF mappings (1 per type)
* Split customercontroller (or do it in the training)




### Running on MacOS with PostgreSQL and VS Code

Note: it seems to be possible to build/debug multiple projects at the same time, but no time to figure that out yet. Cf e.g. https://elanderson.net/2018/04/run-multiple-projects-in-visual-studio-code/

#### Install PostgreSQL and Postico
On MacOS, a great and simple PostgreSQL implementation is Postgres.app together with Postico client.
* Download Postgres.app from https://postgresapp.com/downloads.html
Move to Applications folder and open it.
Click "Initialize" to create a new server
You now have a PostgreSQL server running on your Mac with default settings:
   Host	            localhost
   Port	            5432
   User	            your system user name
   Database	        same as user
   Password	        none
   Connection URL	postgresql://localhost

* Download Postico from https://eggerapps.at/postico/
  Move to Applications folder and open it.


#### Prepare database
In Postico, click "+Database" and name it bookstore.
Open menu Navigate | Go to Terminal.
Paste the following commands:
```
create user pencil42 with encrypted password 'xxxxxxxx';
grant all privileges on database bookstore to pencil42;
```
Note: More fine-grained privileges may be appropriate, to look into.


#### Server
* Make sure you have .Net 2.2 SDK installed.
* Make sure you truste the bookstore ca !!!
* Clone and open project folder in VS Code.
  ```
  git clone https://github.com/StevenArnauts/bookstore.git
  cd bookstore
  code .
  ```
* When asked about unresolved dependencies or required assets to build and debug, click yes :)

* In Server.csproj, change the line
  ```
  <Exec Command="xcopy /Y $(SolutionDir)local.pfx $(ProjectDir)" />
  ```
  to
  ```
  <Exec Command="xcopy /Y $(SolutionDir)local.pfx $(ProjectDir)" Condition=" '$(OS)' == 'Windows_NT' " />
  <Exec Command="cp ../local.pfx $(ProjectDir)" Condition=" '$(OS)' != 'Windows_NT' " />
  ```
  (Note: Haven't tested this on Windows yet! Also, if solution is accepted, then )

* Go to Debug, click Start Debugging (the green arrow), select .Net Core and "Server" project.

* In launch.json:
  Change "ASPNETCORE_ENVIRONMENT": "dev"

* Click Start Debugging again.
  This should cause the DB to be initialized. Go check in Postico.

#### Running both Identity and Server at the same time
This is a bit complicated :)
For now, see launch.json.sample and tasks.json.sample - these are copies of the files in .vscode folder.
Specifically, look at the ASPNETCORE_URLS and the "compoounds" section in launch.json.sample.