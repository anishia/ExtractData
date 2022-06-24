using System;
using System.ComponentModel.DataAnnotations;

namespace APIApplication
{
    public class Expense
    {
        public string Date { get; set; }

        public string cost_centre { get; set; } 

        public decimal total { get; set; }

        public string payment_method { get; set; }
        public string vendor { get; set; }
        public string descripion { get; set; }
        [Display(Name = "Sales Tax")]
        public decimal salestax { get; set; }

        [Display(Name = "Total Excluding Tax")]
        public decimal totalexcludingTax { get; set; }
    }
    
    public class InputData
    {

        public String Email { get; set; }
        public dynamic Testdata { get; set; }
    }
}
