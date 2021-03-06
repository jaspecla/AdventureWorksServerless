﻿using AdventureWorksServerless.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorksServerless.Data
{
  public class AdventureWorksDataRepository
  {
    private readonly AdventureWorksContextFactory _dbContextFactory;
    public AdventureWorksDataRepository(AdventureWorksContextFactory dbContextFactory)
    {
      _dbContextFactory = dbContextFactory;
    }

    public async Task<SalesOrderHeader> GetOrderFromOrderNumberAsync(string orderNumber)
    {
      SalesOrderHeader order = new SalesOrderHeader();

      using (var dbContext = await _dbContextFactory.CreateAsync())
      {
        order = await dbContext.SalesOrderHeader
          .Where(order => order.SalesOrderNumber == orderNumber)
          .Include(order => order.SalesOrderDetail)
            .ThenInclude(sod => sod.Product)
          .Include(order => order.Customer)
          .Include(order => order.BillToAddress)
          .Include(order => order.ShipToAddress)
          .FirstOrDefaultAsync();
      }

      return order;
    }

    public async Task<Product> GetProductFromProductIdAsync(int productId)
    {
      Product product;
      using (var dbContext = await _dbContextFactory.CreateAsync())
      {
        product = await dbContext.Product.FindAsync(productId);
      }

      return product;
    }

    public async Task<Product> UpdateProductWithPriceAsync(int productId, decimal newPrice)
    {
      Product product;

      using (var dbContext = await _dbContextFactory.CreateAsync())
      {
        product = dbContext.Product.Find(productId);
        product.ListPrice = newPrice;
        await dbContext.SaveChangesAsync();
      }

      return product;
    }
    public async Task<IEnumerable<WeeklySpecial>> GetWeeklySpecials()
    {
      IEnumerable<WeeklySpecial> specials;

      using (var dbContext = await _dbContextFactory.CreateAsync())
      {
        specials = await dbContext.WeeklySpecial
          .Include(special => special.Product)
          .ToArrayAsync();
      }

      return specials;
    }

  }
}
