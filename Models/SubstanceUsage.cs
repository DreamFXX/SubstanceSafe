using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubstanceSafe.Models
{
    public class SubstanceUsage
    {
        public int Id { get; set; }

        [ForeignKey("SubstanceType")]
        public int SubstanceTypeId { get; set; }

        public DateTime UsageDate { get; set; } = DateTime.UtcNow;
        public string? Notes { get; set; }
        public decimal Amount { get; set; } // Use decimal for potentially fractional amounts
        public string Unit { get; set; } = string.Empty; // Unit might differ from default

        [ForeignKey("SubstanceTypeId")]
        public virtual SubstanceType? SubstanceType { get; set; }
    }
}
