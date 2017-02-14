using System.Data.Entity;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UserIdentificationMvc.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser<int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, int> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        //public Address MyAddress { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int EmployeeId { get; set; }
    }

    //public class Address
    //{
    //    public int Id { get; set; }
    //    public string City { get; set; }
    //    public string State { get; set; }
    //}

    public class CustomUserRole : IdentityUserRole<int> { }
    public class CustomUserClaim : IdentityUserClaim<int> { }
    public class CustomUserLogin : IdentityUserLogin<int> { }

    public class CustomRole : IdentityRole<int, CustomUserRole>
    {
        public CustomRole() { }
        public CustomRole(string name) { Name = name; }
    }

    public class CustomUserStore : UserStore<ApplicationUser, CustomRole, int,
        CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public CustomUserStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class CustomRoleStore : RoleStore<CustomRole, int, CustomUserRole>
    {
        public CustomRoleStore(ApplicationDbContext context)
            : base(context)
        {
        }
    }

    public class ApplicationDbContext :
        IdentityDbContext<ApplicationUser, CustomRole, int, CustomUserLogin, CustomUserRole, CustomUserClaim>
    {
        public ApplicationDbContext()
            : base("IdentityConnection")
        {
            //Create database always, even If exists (drop then create database always)
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ApplicationDbContext>());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Auth");

            // Mapping for ApiRole ---------------------------------------------------------

            // User Table
            modelBuilder.Entity<IdentityUser>()
                .Ignore(c => c.Email)
                .Ignore(c => c.EmailConfirmed)
                .Ignore(c => c.SecurityStamp)
                .Ignore(c => c.LockoutEnabled)
                .Ignore(c => c.LockoutEndDateUtc)
                .Ignore(c => c.TwoFactorEnabled)
                .Ignore(c => c.AccessFailedCount)
                //.Ignore(c => c.UserName)
                .Ignore(c => c.PhoneNumber)
                .Ignore(c => c.PhoneNumberConfirmed);

            modelBuilder.Entity<ApplicationUser>().Map(c =>
            {
                c.ToTable("User");
                c.Properties(p => new { p.Id, p.EmployeeId, p.FirstName, p.LastName, p.PasswordHash });
                //c.Property(p => p.Id).HasColumnName("UserId");
            }).HasKey(c => c.Id);


            modelBuilder.Entity<CustomUserRole>().HasKey(c => new {c.UserId, c.RoleId});
            modelBuilder.Entity<CustomUserLogin>().HasKey(d => d.UserId);
                
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Logins).WithOptional().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Claims).WithOptional().HasForeignKey(c => c.UserId);
            //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Roles).WithRequired().HasForeignKey(c => c.UserId);

        }


        /*
                    protected override void OnModelCreating(DbModelBuilder modelBuilder)
                    {
                        modelBuilder.HasDefaultSchema("Auth");

                        // Mapping for ApiRole ---------------------------------------------------------

                        // User Table
                        modelBuilder.Entity<IdentityUser>()
                                                      .Ignore(c => c.Email)
                                                      .Ignore(c => c.EmailConfirmed)
                                                      .Ignore(c => c.SecurityStamp)
                                                      .Ignore(c => c.LockoutEnabled)
                                                      .Ignore(c => c.LockoutEndDateUtc)
                                                      .Ignore(c => c.TwoFactorEnabled)
                                                      .Ignore(c => c.AccessFailedCount)
                                                      //.Ignore(c => c.UserName)
                                                      .Ignore(c => c.PhoneNumber)
                                                      .Ignore(c => c.PhoneNumberConfirmed);

                        modelBuilder.Entity<ApplicationUser>().Map(c =>
                        {
                            c.ToTable("User");
                            c.Properties(p => new { p.Id, p.EmployeeId, p.FirstName, p.LastName, p.PasswordHash });
                            c.Property(p => p.Id).HasColumnName("UserId");
                        }).HasKey(c => c.Id);

                        //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Logins).WithOptional().HasForeignKey(c => c.UserId);
                        //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Claims).WithOptional().HasForeignKey(c => c.UserId);
                        //modelBuilder.Entity<ApplicationUser>().HasMany(c => c.Roles).WithRequired().HasForeignKey(c => c.UserId);

                        //UserLogin Table
                        modelBuilder.Entity<IdentityUserLogin>().Map(c =>
                        {
                            c.ToTable("UserLogin");
                            c.Properties(p => new
                            {
                                p.UserId,
                                p.LoginProvider,
                                p.ProviderKey
                            });
                        }).HasKey(p => p.UserId);
                        //modelBuilder.Entity<IdentityUserLogin>().HasKey(i => i.LoginProvider);

                        // Role Table
                        modelBuilder.Entity<IdentityRole>().Map(c =>
                        {
                            c.ToTable("Role");
                            c.Properties(p => new
                            {
                                p.Name
                            });
                            c.Property(p => p.Id).HasColumnName("RoleId");
                        }).HasKey(p => p.Id);
                        //modelBuilder.Entity<IdentityRole>().HasMany(c => c.Users).WithRequired().HasForeignKey(c => c.RoleId);

                        //UserRole Table
                        modelBuilder.Entity<IdentityUserRole>().Map(c =>
                        {
                            c.ToTable("UserRole");
                            c.Properties(p => new
                            {
                                p.UserId,
                                p.RoleId
                            });
                        }).HasKey(c => new { c.UserId });
                        //modelBuilder.Entity<IdentityUserRole>().HasKey(c => new { c.UserId });

                        // UserClaim Table
                        modelBuilder.Entity<IdentityUserClaim>().Map(c =>
                        {
                            c.ToTable("UserClaim");
                            c.Properties(p => new
                            {
                                p.UserId,
                                p.ClaimValue,
                                p.ClaimType
                            });
                        }).HasKey(c => c.Id);

                    }
                    */
    }


}