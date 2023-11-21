using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMICalculator.Model
{
    public class Loan
    {
        public string Month { get; set; }
        public double Year { get; set; }
        public double Principal { get; set; }
        public double Interest { get; set; }
        public double TotalPayment { get; set; }
        public double Balance { get; set; }
    }
}
