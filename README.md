# README #

This README would normally document whatever steps are necessary to get your application up and running.

### What is this repository for? ###

* Quick summary
* Version
* [Learn Markdown](https://bitbucket.org/tutorials/markdowndemo)

### How do I get set up? ###

* Summary of set up
* Configuration
* Dependencies
* Database configuration
* How to run tests
* Deployment instructions

### Contribution guidelines ###

* Writing tests
* Code review
* Other guidelines

### Who do I talk to? ###

* Repo owner or admin
* Other community or team contact


### How to deploy on Azure ###

* Create an application under AppServices in Azure
* Under Configuration => Application settings, add following keys:
  
DB__ConnectionString=data source=?;initial catalog=DF_PRMS_DB_Test;user id=?;password=?;MultipleActiveResultSets=True;App=EntityFramework;

Azure__CallbackPath=/signin-oidc
Azure__Domain=AppDFPRM.onmicrosoft.com
Azure__Instance=https://login.microsoftonline.com/
Azure__ClientId=?
Azure__TenantId=?
Azure__StorageConnectionString=DefaultEndpointsProtocol=https;AccountName=?;AccountKey=?;EndpointSuffix=core.windows.net

Jwt__ExpiryInMinutes=525600
Jwt__Site=http://www.security.org
Jwt__SigningKey=?

* Right click on project => Publish
  (For first time)
  New Profile => Azure => Azure App Services => Select your subscription => API Management

