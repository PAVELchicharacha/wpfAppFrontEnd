using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace labFrontEnd.model
{
    public class Product
    {
        public int Id { get; set; }

        public DateTime SaleDate { get; set; }

        public decimal ProductSales { get; set; }

        public int Quantity { get; set; }

        public decimal ProductCoast { get; set; }

        public int IdTovar { get; set; }
    }
}

