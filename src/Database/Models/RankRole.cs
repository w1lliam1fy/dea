namespace DEA.Database.Models
{
    public partial class RankRole
    {
        public int Id { get; set; }

        public double CashRequired { get; set; }

        public decimal RoleId { get; set; }

        public virtual Guild Guild { get; set; }
    }
}
