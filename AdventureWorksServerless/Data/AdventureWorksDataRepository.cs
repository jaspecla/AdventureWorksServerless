using AdventureWorksServerless.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventureWorksServerless.Data
{
  public class AdventureWorksDataRepository
  {
    private readonly AdventureWorksContext _dbContext;
    public AdventureWorksDataRepository(AdventureWorksContext dbContext)
    {
      _dbContext = dbContext;
    }

    public SalesOrderHeader GetOrderFromOrderNumber(string orderNumber)
    {
      var order = _dbContext.SalesOrderHeader
        .Where(order => order.SalesOrderNumber == orderNumber)
        .Include(order => order.SalesOrderDetail)
        .Include(order => order.Customer)
        .Include(order => order.BillToAddress)
        .Include(order => order.ShipToAddress)
        .FirstOrDefault();

      return order;

    }
  }
}
