namespace DEA.Database.Models
{
    public partial class ModRole
    {
        public int Id { get; set; }

        public int PermissionLevel { get; set; }

        public decimal RoleId { get; set; }

        public virtual Guild Guild { get; set; }
    }
}
