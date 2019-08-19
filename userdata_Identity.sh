# Install AWS CLI
sudo snap install aws-cli --classic

# register the Microsoft key, register the product repository, and install required dependencies
wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb

# Update the products available for installation, then install the .NET SDK.
sudo add-apt-repository universe
sudo apt-get install apt-transport-https
sudo apt-get update
sudo apt-get install dotnet-sdk-2.2=2.2.203-1

# Clone git repo
# For now at least, clone the runOnMacOS branch instead of master.
git clone --single-branch --branch runOnMacOS https://github.com/StevenArnauts/bookstore.git

# Build and run some code (bookstore/Identiy project, in this case)
cd bookstore/Identity/
dotnet Build
dotnet run

# Note: SSL is assumed to be handled by an ALB in front of this instance (or this collection of instances).
