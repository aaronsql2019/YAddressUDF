## MS SQL Server User-Defined Function (UDF) for Address Correction and Geocoding

This SQL Server UDF takes a postal address as input and returns a properly parsed and corrected address as output together with the latitude and longitude of its location on the map.

# Getting Started

The following tutorial is courtesy of Ed Katibah, Spatial Program Manager with the Microsoft SQL Server team, from his blog "Spatial Ed" http://blogs.msdn.com/edkatibah.

Yuri Software has created two SQL Server-compatible geocoding functions which have been released on CodePlex. In order to make these functions usable in a test environment, Yuri Software has created a web service to which these functions connect. In a production environment, a high performance local YAddress server would be used. Each of these functions is described, below, along with simple T-SQL coding examples.

### ProcessAddress

The ProcessAddress function is the first of the YAddress geocoding implementations for SQL Server. Executing the following T-SQL illustrates the values returned by YAddress:

```sql
SELECT * FROM ProcessAddress('3641 Mt Diablo Blvd', 'Lafayette, CA')
```

ErrorCode: 0 <br>
ErrorMessage: <br>
AddressLine1: 3641 MT DIABLO BLVD <br>
AddressLine2: LAFAYETTE, CA 94549-9998 <br>
Number: 3641 <br>
PreDir: <br>
Street: MT DIABLO <br>
Suffix: BLVD <br>
PostDir: <br>
Sec: <br>
SecNumber: <br>
City: LAFAYETTE <br>
State: CA <br>
Zip: 94549 <br>
Zip4: 9998 <br>
County: CONTRA COSTA <br>
CountyFP: 06-013 <br>
Latitude: 37.891068 <br>
Longitude: -122.126023 <br>
PreciseGeocode: 1

Since this function returns a latitude and longitude value as floating point numbers, it is compatible with both SQL Server 2005 and SQL Server 2008. 

Let's use SQL Server 2008 to construct a geography instance from the latitude and longitude returned from the ProcessAddress function, in this case using the OGC-compliant STGeomFromText() constructor:

```sql
DECLARE @lat FLOAT 
DECLARE @lon FLOAT 
DECLARE @geo GEOGRAPHY 
SELECT @lat = latitude, @lon = longitude FROM ProcessAddress('3641 Mt Diablo Blvd', 'Lafayette, CA') 
SELECT @geo = GEOGRAPHY::STGeomFromText('Point('  CAST(@lon AS VARCHAR(32))  ' '  CAST(@lat AS VARCHAR(32))  ')',4326) 
SELECT @geo.STAsText() 
GO 
--POINT (-122.126 37.8911)
```

While the STGeomFromText() constructor certainly works, it's an effort to construct the Well-Known Text Point string. Let's construct the T-SQL using the Point() constructor:

```sql
DECLARE @lat FLOAT 
DECLARE @lon FLOAT 
DECLARE @geo GEOGRAPHY 
SELECT @lat = latitude, @lon = longitude FROM ProcessAddress('3641 Mt Diablo Blvd', 'Lafayette, CA') 
SELECT @geo = GEOGRAPHY::Point(@lat,@lon,4326) 
SELECT @geo.STAsText() 
GO 
--POINT (-122.126023 37.891068)
```

This construction is certainly much easier and undoubtedly more performant. But Yuri Software has another trick up their sleeve - a SQL Server 2008-specific function: ProcessAddress2008. 


###ProcessAddress2008

ProcessAddress2008() returns a geography instance directly, eliminating the need for the constructor. Here is the full set of data returned from the function:

```sql
SELECT * FROM ProcessAddress2008('3641 Mt Diablo Blvd', 'Lafayette, CA')
```

ErrorCode: 0 <br>
ErrorMessage: <br>
AddressLine1: 3641 MT DIABLO BLVD <br>
AddressLine2: LAFAYETTE, CA 94549-9998 <br>
Number: 3641 <br>
PreDir: <br>
Street: MT DIABLO <br>
Suffix: BLVD <br>
PostDir: <br>
Sec: <br>
SecNumber: <br>
City: LAFAYETTE <br>
State: CA <br>
Zip: 94549 <br>
Zip4: 9998 <br>
County: CONTRA COSTA <br>
CountyFP: 06-013 <br>
Location: 0xE6100000010C8D4127840EF24240CEE2C5C210885EC0 <br>
PreciseGeocode: 1

The Location field contains an instance of type geography, directly. To show the advantage of this approach, let's construct the same basic query, used in ProcessAddress, above:

```sql
DECLARE @geo GEOGRAPHY 
SELECT @geo = Location FROM ProcessAddress2008('3641 Mt Diablo Blvd', 'Lafayette, CA') 
SELECT @geo.STAsText() 
GO 
--POINT (-122.126023 37.891068)
```

We could be even more concise and achieve the same result:

```sql
SELECT Location.STAsText() FROM ProcessAddress2008('3641 Mt Diablo Blvd', 'Lafayette, CA') 
GO 
--POINT (-122.126023 37.891068)
```

###Registering The Function
The last observation surrounds the setup commands used to register the new function with SQL Server. There are occasions where databases that have been restored from backups do not have the database owner correctly specified. This issue surfaced while trying to create the assembly from the YAddressSqlFunction.dll:

> The database owner SID recorded in the master database differs from the database owner SID recorded in database 'SampleZipcodes'. You should correct this situation by resetting the owner of database 'SampleZipcodes' using the ALTER AUTHORIZATION statement.

> Msg 6582, Level 16, State 1, Procedure ProcessAddress, Line 3 

> Assembly 'YAddressSqlFunction' is not visible for creating SQL objects. Use ALTER ASSEMBLY to change the assembly visibility. 

 In order to ameliorate this issue, use the following stored procedure, setting the database owner to system administrator (sa) before creating the assembly:

```sql
sp_changedbowner 'sa' 
```

