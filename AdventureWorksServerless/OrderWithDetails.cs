using AdventureWorksServerless.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventureWorksServerless
{
  public class OrderWithDetails
  {
    public SalesOrderHeader Header { get; set; }
    public IEnumerable<SalesOrderDetail> LineItems { get; set; }
  }
}
