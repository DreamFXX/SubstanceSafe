using System.Collections.Generic;

namespace SubstanceSafe.Models;

public class SubstanceCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property for related Substances
    public virtual ICollection<SubstanceType>? SubstanceTypes { get; set; }
}
