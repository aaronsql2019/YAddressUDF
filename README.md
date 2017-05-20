## MS SQL Server User-Defined Function (UDF) for Address Correction and Geocoding

This SQL Server User Defind Function (UDF) will correct, validate, standardize and geocode postal addresses in Microsoft SQL Server. 
The UDF can be called directly from SQL queries, included in stored procedures, or incorporated in ETL packages built with SSIS. 

# Setup

1. Download YAddress UDF Binaries.
2. Place "YAddressSqlFunction.dll" on a local disk in your SQL Server. 
3. Follow installation steps in "SQL Setup Script.sql". 


### Common Setup Issues

If you get any SQL errors while executing the setup script, make sure your database has a valid owner. For a variety of reasons, such as backup restores, etc., databases may have invalid or non-existent owners. This will preclude proper UDF registration. Correct the issue by setting the database owner to system administrator 'sa': 

```sql
sp_changedbowner 'sa'
```

# Usage

### ProcessAddress

ProcessAddress is a table-valued UDF. Executing the following T-SQL illustrates the values returned by YAddress: 

```sql
SELECT * FROM ProcessAddress('506 Fourth Avenue Unit 1', 'Asbury Prk, NJ', NULL) 
```
ErrorCode: 0
ErrorMessage:
AddressLine1: 506 4TH AVE APT 1
AddressLine2: ASBURY PARK, NJ 07712-6086
Number: 506
PreDir:
Street: 4TH
Suffix: AVE
PostDir:
Sec: APT
SecNumber: 1
City: ASBURY PARK
State: NJ
Zip: 7712
Zip4: 6086
County: MONMOUTH
CountyFP: 25
CensusTract: 1015
CensusBlock: 8070.03
Latitude: 40.223571
Longitude: -74.005973
GeocodePrecision: 5 

### SQL Spatial Geography

ProcessAddress returns address location as two floating point values of latitude and longitude. To convert them to SQL Server spatial type Geography: 

```sql
SELECT Location = GEOGRAPHY::Point(Latitude, Longitude, 4326)
FROM ProcessAddress('506 Fourth Avenue Unit 1', 'Asbury Prk, NJ', NULL) 
```

