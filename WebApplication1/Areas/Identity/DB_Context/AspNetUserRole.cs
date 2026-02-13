namespace Identity.DB_Context
{
    public partial class AspNetUserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual AspNetRole Role { get; set; } = null!;

        public virtual AspNetUser User { get; set; } = null!;
    }
}
