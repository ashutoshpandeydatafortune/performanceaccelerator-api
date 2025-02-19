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

### How to deploy on AWS ###

* API is running on AWS Lightsail
* Generate a new rsa private/public key pair on local
* Copy public key to c:\programdata\ssh\administrators_authorized_keys
* Set permissions for file:
  Right click on file => Properties window => Security tab
  Click Advanced (at the bottom).
  Click Disable inheritance (if it is enabled).
  When prompted, choose "Convert inherited permissions into explicit permissions".
  Administrator to have Read, Read and Execute, Write permissions.
  Click Apply and OK.

* Copy private key to Github repository => Settings => Security => Secrets and Variables => Actions
  => New repository secret => LIGHTSAIL_SSH_PRIVATE_KEY


### How to deploy on Azure ###

* Create an application under AppServices in Azure
* Under Settings => Environment variables, add following keys:
  
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

SMTP_HOST=smtp.office365.com
SMTP_PORT=587
SMTP_USERNAME=?
SMTP_PASSWORD=?

* Right click on project => Publish
  (For first time)
  New Profile => Azure => Azure App Services => Select your subscription => API Management

