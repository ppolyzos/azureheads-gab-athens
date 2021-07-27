# EventManagement
A tool to help you generate community event applications that can be used from AzureHeads community like global azure yearly event, like [https://global-azure-greece-2021.azurewebsites.net/](https://global-azure-greece-2021.azurewebsites.net/) 


## Development 

### User Secrets
All the projects of this solution are accompanied by configuration files like `appsettings.json` and `appsettings.<env>.json` files

In order to store secrets during development you can utilize the [user-secrets](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) tool.

#### Enable secret storage
The Secret Manager tool operates on project-specific configuration settings stored in your user profile.

The Secret Manager tool includes an init command in .NET Core SDK 3.0.100 or later. To use user secrets, run the following command in the project directory:
```
dotnet user-secrets init
```
and in order to set a secret, like the storage account used for `Web` project, you need to run the following command:
```
dotnet user-secrets set "ConnectionStrings:StorageAccount" "<storage_account_connection_string>"
```
or the Sessionize EventId:
```
dotnet user-secrets set "Sessionize:EventId" "<sessionize_event_id>"
```


## Logging
For logging [SEQ](https://datalust.co/seq) is used.

### Setup & Usage
You can set it up using [dev-tools](https://github.com/ppolyzos/dev-tools) and spin up a `seq` instance.

To view logs you can open [localhost/#/events](http://localhost/#/events).


## IoC

To setup the infrastructure in azure and start working with your environment you can run the terraform code that exists in `src\ioc\<env>\<region>` folder using:
```
$ terraform init    # to initialize a working directory containing Terraform configuration files.
$ terraform plan    # to create an execution plan
$ terraform apply   # to execute the actions proposed in a Terraform plan.

$ terraform destroy # to destroy all remote objects managed by a particular Terraform configuration and start again
```

For more info about Terraform please visit the following links:
* [Terraform CLI](https://www.terraform.io/docs/cli/index.html)
* [Terraform Azure Provider (azurerm)](https://registry.terraform.io/providers/hashicorp/azurerm/latest/docs)