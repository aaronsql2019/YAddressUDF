using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;
using System.Net;
using System.Xml.Serialization;

public class YAddressSqlFunction
{
    // Our data model for serialization
    public class Address
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Number { get; set; }
        public string PreDir { get; set; }
        public string Street { get; set; }
        public string Suffix { get; set; }
        public string PostDir { get; set; }
        public string Sec { get; set; }
        public string SecNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Zip4 { get; set; }
        public string County { get; set; }
        public string StateFP { get; set; }
        public string CountyFP { get; set; }
        public string CensusTract { get; set; }
        public string CensusBlock { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int GeoPrecision { get; set; }
    }

    public static Address CallYAddress(string sAddressLine1, string sAddressLine2, string sUserKey)
    {
        string sRequest = string.Format(
            "http://www.yaddress.net/api/address?AddressLine1={0}&AddressLine2={1}&UserKey={2}",
            Uri.EscapeDataString(sAddressLine1 == null ? "" : sAddressLine1),
            Uri.EscapeDataString(sAddressLine2 == null ? "" : sAddressLine2),
            Uri.EscapeDataString(sUserKey == null ? "" : sUserKey));
        HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(sRequest);
        req.Accept = "application/xml";
        WebResponse res = req.GetResponse();
        XmlSerializer serializer = new XmlSerializer(typeof(Address));
        return (Address)serializer.Deserialize(res.GetResponseStream());
    }
    
    // Our enumerable class. The IEnumerable interface comes from List<>.
    class YAddressResults : List<Address>
    {
        public YAddressResults(string AddressLine1, string AddressLine2, string UserKey = null)
        {
            // Process the address by calling the Web Service
            Address adr = CallYAddress(AddressLine1, AddressLine2, UserKey);

            // Store results in a 1-item list
            Add(adr);
        }
    }

    // Init method of the User Defined Table Valued function
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
        out int GeoPrecision)
    {
        // Get a hold of our Address object
        Address ad = (Address)obj;

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
        GeoPrecision = ad.GeoPrecision;
    }
}

