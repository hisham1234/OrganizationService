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
|Get User|POST /organization-service/api/users/{userId}|
|Update User|PUT /organization-service/api/users/{userId}|
|Delete User|DELETE /organization-service/api/users/{userId}|
|Create Office|POST /organization-service/api/offices|
|Get Office List|GET /organization-service/api/offices|
|Get Office|POST /organization-service/api/offices/{officeId}|
|Update Office|PUT /organization-service/api/offices/{officeId}|
|Delete Office|DELETE /organization-service/api/offices/{officeId}|
|Create Role|POST /organization-service/api/roles|
|Get Role List|GET /organization-service/api/roles|
|Get Role|POST /organization-service/api/roles/{roleId}|
|Update Role|PUT /organization-service/api/roleds/{roleId}|
|Delete Role|DELETE /organization-service/api/roles/{roleId}|

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
If you want to run it on a local PC, download and install MySQL 8.0‚ÆPostman on the local PC.

- [MySQL Community Server 8.0.25](https://dev.mysql.com/downloads/mysql/)
- [Postman](https://www.postman.com/downloads/)


### 2. Setting environment variables

|Variable|Value|
|:--|:--|
|MYSQL_CONNECTSTRING|server=*{ServerName}*;database=*{DatabaseName}*;user=*{UserName}*;password=*{UserPassword}*|

***

# Build and Test
Organizaiton-Service checks for the existence of the database when the application is started and automatically creates the relevant tables as needed, so there is no need to migrate the database.