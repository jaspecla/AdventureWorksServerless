using System;
using System.Collections.Generic;
using System.Text;

namespace AdventureWorksServerless.Data
{
  public class PriceUpdate
  {
    public int ProductId { get; set; }
    public float NewPrice { get; set; }
  }
}
