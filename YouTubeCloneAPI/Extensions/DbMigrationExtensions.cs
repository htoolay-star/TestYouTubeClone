using YouTubeClone.Shared.Constants;
using YouTubeClone.Domain.Data.Seeder;

namespace YouTubeCloneAPI.Extensions
{
    public static class DbMigrationExtensions
    {
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var seeder = services.GetRequiredService<IDbSeeder>();
                await seeder.SeedAsync();
                Console.WriteLine(AuthMessages.System.SeedingSuccess);
            }
            catch (Exception ex)
            {
                // Logger သုံးထားရင် logger နဲ့ ထုတ်လို့ရပါတယ်
                Console.WriteLine(string.Format(AuthMessages.System.DatabaseSeedingError, ex.Message));
            }
        }
    }
}
