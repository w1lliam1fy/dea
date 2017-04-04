using System;
using System.Collections.Generic;

namespace DEA.Database.Models
{

    public partial class Gang
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Gang()
        {
            Members = new HashSet<User>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public decimal LeaderId { get; set; }

        public double Wealth { get; set; } = 0.0;

        public DateTimeOffset Raid { get; set; } = DateTimeOffset.Now.AddYears(-1);

        public virtual Guild Guild { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Members { get; set; }
    }
}