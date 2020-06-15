using System;
using System.Collections.Generic;

namespace AdventureWorksServerless.Models.Entities
{
    public partial class WeeklySpecial
    {
        public int SpecialId { get; set; }
        public int ProductId { get; set; }
        public decimal SalePrice { get; set; }

        public virtual Product Product { get; set; }
    }
}
