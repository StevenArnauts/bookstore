#!/bin/bash
echo "STARTING USERDATA!"
echo "DON'T FORGET TO ADJUST THE VALUE OF CHANGEME"

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
# For now at least, clone the runOnMacOS branch instead of master.
cd /var
sudo git clone --single-branch --branch freetemp https://github.com/StevenArnauts/bookstore.git

# Change owner
sudo chown -R ubuntu /var/bookstore

# Build and run some code (bookstore/Identiy project, in this case)
export ASPNETCORE_ENVIRONMENT=dev
export ConnectionStrings__identity="Host=bookstoredbtest4.c7xajvaahzcs.us-east-1.rds.amazonaws.com;Database=identity;Username=pencil42;Password=CHANGEME"
cd bookstore/Identity/
dotnet restore
dotnet build
dotnet run

# Note: SSL is assumed to be handled by an ALB in front of this instance (or this collection of instances).
