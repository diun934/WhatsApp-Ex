using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace all_on_whatsapp.Model
{
    public class DbModel
    {
        public class User
        {
            public int Id { get; set; }
            public string? UserName { get; set; } = " - ";
            public string? SystemName { get; set; } = " - ";
            public string? Phone { get; set; } = " - ";
            public string? Level { get; set; } = " - ";
            public string? Tag { get; set; } = " - ";
            public string? Remarks { get; set; } = " - ";

        }
        public class Order
        {
            public int Id { get; set; }
            public string SystemName { get; set; } = " - ";
            public string Salesman { get; set; } = " - ";
            public string? Type { get; set; } = " - ";
            public string? Currency { get; set; } = " - ";
            public double BusinessAmount { get; set; }
            public string? BusinessObject { get; set; }
            public double CostAmount { get; set; }
            public string? PayObject { get; set; }
            public double PayRate { get; set; }
            public string? Remarks { get; set; } = " - ";
        }

    }
}
