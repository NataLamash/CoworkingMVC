using CoworkingDomain.Model;
using Microsoft.AspNetCore.Identity;

public class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (await roleManager.FindByNameAsync("admin") == null)
            await roleManager.CreateAsync(new IdentityRole("admin"));
        if (await roleManager.FindByNameAsync("user") == null)
            await roleManager.CreateAsync(new IdentityRole("user"));

        string adminEmail = "admin@gmail.com";
        string adminPassword = "Qwerty_1";
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            var admin = new User { Email = adminEmail, UserName = adminEmail };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
