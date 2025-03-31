using System.Collections.Generic;

namespace SubstanceSafe.Models
{
    public class SubstanceCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // Default to empty string to avoid null warnings

        // Navigation property for related SubstanceTypes
        public virtual ICollection<SubstanceType>? SubstanceTypes { get; set; }
    }
}
