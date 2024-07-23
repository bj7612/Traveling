using Microsoft.AspNetCore.Identity;

namespace Traveling.Models
{
    // 11-9 【应用】定制用户模型并添加初始化用户数据
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public ShoppingCart ShoppingCart { get; set; }

        // Orders


        // overload four table: AspNetUserRoles, AspNetUserClaim AspNetUserLogin and AspNetUserToken
        public virtual ICollection<IdentityUserRole<string>>? UserRoles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }
        public virtual ICollection<IdentityUserToken<string>> Tokens { get; set; }
    }
}
