# If you haven't logged in yet, you'll need to before doing so.
#az login

# Retrieve the list of locations available for the Virtual Machines provider
az provider show --namespace Microsoft.compute --query "resourceTypes[?resourceType=='virtualMachines'].locations | [0]" > regions.json
# Create the directory where we'll store vmSizes per regions
New-Item -ItemType Directory vmSizes -Force

# Search and replace region names to proper regions
$regions = Get-Content -Raw -Path regions.json | ConvertFrom-Json | ForEach-Object { $_ -replace ' ', ''  }

# Retrieve VM sizes for each regions
$regions | ForEach-Object { az vm list-sizes --location $_ > .\vmSizes\$_`.json }

# Create the proper storage account
az group create --name vmSizeBrowser --location EastUS
az storage account create --name vmsizebrowser -g vmSizeBrowser --sku Standard_LRS
$connectionString = (az storage account show-connection-string --name vmsizebrowser -g vmSizeBrowser --query "connectionString") -replace '"',''
az storage container create --name default --connection-string "$connectionString"

# Upload all the VM Sizes to blob storage
az storage blob upload-batch --connection-string $connectionString --content-type json -d default/vmSizes -s .\vmSizes
az storage blob upload-batch --connection-string $connectionString --content-type json -d default/static -s .\static