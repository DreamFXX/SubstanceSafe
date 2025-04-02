    using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SubstanceSafe.Models
{
    public class SubstanceType
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? DefaultUnit { get; set; } // Optional default unit

        // Foreign key for SubstanceCategory
        public int CategoryId { get; set; }

        // Navigation property for SubstanceCategory
        [ForeignKey("CategoryId")]
        public virtual SubstanceCategory? Category { get; set; }

        // Navigation property for related SubstanceUsages
        public virtual ICollection<SubstanceUsage>? SubstanceUsages { get; set; }
    }
}
