using System;

namespace SubstanceSafe.Models
{
    public class SubstanceUsage
    {
        public int Id { get; set; }
        public string Substance { get; set; }
        public DateTime UsageDate { get; set; }
        public string Notes { get; set; }
        public int Amount { get; set; }
        public string Unit { get; set; }
        public string Category { get; set; }
    }
}