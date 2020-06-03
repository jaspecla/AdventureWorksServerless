﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AdventureWorksServerless.Models.Entities
{
  public partial class Address
  {
    public Address()
    {
      CustomerAddress = new HashSet<CustomerAddress>();
      SalesOrderHeaderBillToAddress = new HashSet<SalesOrderHeader>();
      SalesOrderHeaderShipToAddress = new HashSet<SalesOrderHeader>();
    }

    public int AddressId { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string City { get; set; }
    public string StateProvince { get; set; }
    public string CountryRegion { get; set; }
    public string PostalCode { get; set; }
    public Guid Rowguid { get; set; }
    public DateTime ModifiedDate { get; set; }

    [JsonIgnore]
    public virtual ICollection<CustomerAddress> CustomerAddress { get; set; }
    [JsonIgnore]
    public virtual ICollection<SalesOrderHeader> SalesOrderHeaderBillToAddress { get; set; }
    [JsonIgnore]
    public virtual ICollection<SalesOrderHeader> SalesOrderHeaderShipToAddress { get; set; }
  }
}