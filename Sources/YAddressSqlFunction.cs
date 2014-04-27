using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;

public class YAddressSqlFunction
{
    // Our enumerable class. The IEnumerable interface comes from List<>.
    class YAddressResults : List<SqlFunction.com.yurisw.app.Address>
    {
        public YAddressResults(string AddressLine1, string AddressLine2, string UserKey = null)
        {
            // Process the address by calling the Web Service
            SqlFunction.com.yurisw.app.YAddressWebService ap =
                new SqlFunction.com.yurisw.app.YAddressWebService();
            //if (!string.IsNullOrEmpty(sProxyServerUri))
            //    ap.Proxy = new WebProxy(sProxyServerUri, true);
            SqlFunction.com.yurisw.app.Address adr =
                ap.Process(AddressLine1, AddressLine2, UserKey);
            ap.Dispose();

            // Store results in a 1-item list
            Add(adr);
        }
    }

    // Init method of the User Defined Table Valued findtion
    [SqlFunction(FillRowMethodName = "FillRow")]
    public static IEnumerable InitMethod(string AddressLine1, string AddressLine2, string UserKey = null)
    {
        return new YAddressResults(AddressLine1, AddressLine2, UserKey);
    }

    // FillRow method of the User Defined Table Valued function
    public static void FillRow(
        Object obj, 
        out int ErrorCode,
        out string ErrorMessage,
        out string AddressLine1,
        out string AddressLine2,
        out string Number,
        out string PreDir,
        out string Street,
        out string Suffix,
        out string PostDir,
        out string Sec,
        out string SecNumber,
        out string City,
        out string State,
        out string Zip,
        out string Zip4,
        out string County,
        out string CountyFP,
        out string CensusBlock,
        out string CensusTract,
        out double Latitude,
        out double Longitude,
        out string GeocodePrecision)
    {
        // Get a hold of our Address object
        SqlFunction.com.yurisw.app.Address ad = 
            (SqlFunction.com.yurisw.app.Address)obj;

        // Populate output values
        ErrorCode = ad.ErrorCode;
        ErrorMessage = ad.ErrorMessage;
        AddressLine1 = ad.AddressLine1;
        AddressLine2 = ad.AddressLine2;
        Number = ad.Number;
        PreDir = ad.PreDir;
        Street = ad.Street;
        Suffix = ad.Suffix;
        PostDir = ad.PostDir;
        Sec = ad.Sec;
        SecNumber = ad.SecNumber;
        City = ad.City;
        State = ad.State;
        Zip = ad.Zip;
        Zip4 = ad.Zip4;
        County = ad.County;
        CountyFP = ad.CountyFP;
        CensusBlock = ad.CensusBlock;
        CensusTract = ad.CensusTract;
        Latitude = ad.Latitude;
        Longitude = ad.Longitude;
        GeocodePrecision = ad.GeocodePrecision.ToString();
    }


}

