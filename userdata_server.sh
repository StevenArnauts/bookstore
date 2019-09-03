#!/bin/bash
echo "STARTING USERDATA!"
echo "DON'T FORGET TO ADJUST THE VALUE OF CHANGEME AND POSSIBLY OTHER ENVIRONMENT VARIABLES"

# Note: SSL is assumed to be handled by an ALB in front of this instance (or this collection of instances).

# Register the Microsoft key, register the product repository, and install required dependencies
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Update the products available for installation, then install the .NET SDK.
sudo add-apt-repository universe
sudo apt-get install apt-transport-https -y
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2=2.2.203-1 -y

# Install AWS CLI
sudo snap install aws-cli --classic


# Clone git repo
# For now at least, clone the freetemp branch instead of master.
cd /var
sudo git clone --single-branch --branch freetemp https://github.com/StevenArnauts/bookstore.git

# Change owner
sudo chown -R ubuntu /var/bookstore

# Build and run some code (bookstore/Server project, in this case)
export DOTNET_CLI_HOME=/tmp
export ASPNETCORE_ENVIRONMENT=dev
export ASPNETCORE_URLS=https://*:443
export ConnectionStrings__bookstore="Host=bookstoredbfwi.c7xajvaahzcs.us-east-1.rds.amazonaws.com;Database=bookstore;Username=pencil42;Password=CHANGEME"
export Web__Authority="https://ids-fwi.sbx.pencil42-apps.be"

# Log all env vars
export

cd bookstore/Server/
dotnet restore
dotnet build
nohup dotnet run > dotnetrun_output.txt 2>&1 &
