using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubstanceSafe.Models
{
    public class SubstanceUsage
    {
        public int Id { get; set; }

        // Foreign key for SubstanceType
        public int SubstanceTypeId { get; set; }

        public DateTime UsageDate { get; set; } = DateTime.UtcNow; // Default to current UTC time
        public string? Notes { get; set; } // Notes can be nullable
        public decimal Amount { get; set; } // Use decimal for potentially fractional amounts
        public string Unit { get; set; } = string.Empty; // Unit might differ from default

        // Navigation property for SubstanceType
        [ForeignKey("SubstanceTypeId")]
        public virtual SubstanceType? SubstanceType { get; set; }
    }
}
