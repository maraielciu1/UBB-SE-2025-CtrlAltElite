# Market Place Windows Desktop App

## Database Initialization

Use file [DBSchemaInit.sql](MarketPlace924/DBConnection/DBSchemaInit.sql) to initialize/reset the DB Schema.

## External Configation

The external configuration is read from a `MarketPlaceConfig.ini` file located in the current users home folder.

Create this file and add inside the Database connection string.

```ini
[Database]
ConnectionString=Data Source=localhost;Initial Catalog=IssDb;Integrated Security=True;TrustServerCertificate=True
```