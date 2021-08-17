[![Build status](https://dev.azure.com/NEE-devops/Timeline/_apis/build/status/Build%20Organization-Service)](https://dev.azure.com/NEE-devops/Timeline/_build/latest?definitionId=13)
# Introduction 
Organization-Service provides access to the Organization-Storage database. Access data is three entities: users, offices, roles.
This service checks for the existence of a MySQL database at the start, automatically creates these entities if they do not exist, and provides CRUD processing for the entities.

***
### API references
API references is defined as follows.


|Functionality|Interface|
|:--|:--|
|Create User|POST /organization-service/api/users|
|Get User List|GET /organization-service/api/users|
|Get User|GET /organization-service/api/users/{userId}|
|Update User|PUT /organization-service/api/users/{userId}|
|Delete User|DELETE /organization-service/api/users/{userId}|
|Create Office|POST /organization-service/api/offices|
|Get Office List|GET /organization-service/api/offices|
|Get Office|GET /organization-service/api/offices/{officeId}|
|Get Users List of Specific Office|GET /organization-service/api/offices/{officeId}/users|
|Update Office|PUT /organization-service/api/offices/{officeId}|
|Delete Office|DELETE /organization-service/api/offices/{officeId}|
|Create Role|POST /organization-service/api/roles|
|Get Role List|GET /organization-service/api/roles|
|Get Role|GET /organization-service/api/roles/{roleId}|
|Update Role|PUT /organization-service/api/roleds/{roleId}|
|Delete Role|DELETE /organization-service/api/roles/{roleId}|
|Login my account|POST /organization-service/api/accounts/login|
|Delete my account|POST /organization-service/api/accounts/me|
|Update my account|PUT /organization-service/api/accounts/me|
|Get my account|Get /organization-service/api/accounts/me|
|Verify account |Get /organization-service/api/accounts/verify|

### Authentication Example
First of all you need to get your authentication Token by doing a login request.

Post /organization-service/api/accounts/login
```JSON

{
    "Email": "admin",
    "Password": "admin"
}

{
    "token": "ey....",
    "message": "Authentication success"
}
```

All the other function are protected, and need a valid token to be used. The token value need to be used in the header of the request.
#### Header value
|Key|Value|
|:--|:--|
|Authorization|Bearer {YOUR_TOKEN}|

By default an user with "admin/admin" credential has been created.

***
### DTO DataFormat
Data Transfer Object(DTO) is defined as follows. CreatedAt and UpdateAt are not allowed to be set from the outside.

**UserDTO**

|Column name|Data type|
|:--|:--|
|ID|int|
|Email|string|
|Password|string|
|FirstName|string|
|LastName|string|
|OfficeID|int?|
|Roles|collection|

**OfficeDTO**

|Column name|Data type|
|:--|:--|
|ID|int|
|OfficeName|string|
|ParentOfficeID|int?|

**RoleDTO**

|Column name|Data type|
|:--|:--|
|ID|int|
|RoleName|string|

***

# Getting Started

### 1. Installation process
If you want to run it on a local PC, download and install MySQL 8.0??Postman on the local PC.

- [MySQL Community Server 8.0.25](https://dev.mysql.com/downloads/mysql/)
- [Postman](https://www.postman.com/downloads/)


### 2. Setting environment variables

|Variable|Value|
|:--|:--|
|MYSQL_CONNECTSTRING|server=*{ServerName}*;database=*{DatabaseName}*;user=*{UserName}*;password=*{UserPassword}*|
|APPINSIGHTS_INSTRUMENTATIONKEY|This value is normally configured automatically at the creation of the appinsights|
|APPLICATIONINSIGHTS_CONNECTION_STRING|This value is normally configured automatically at the creation of the appinsights|

***

# Build and Test
Organizaiton-Service checks for the existence of the database when the application is started and automatically creates the relevant tables as needed, so there is no need to migrate the database.