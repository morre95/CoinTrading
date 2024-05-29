namespace CoinTrading
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();


            builder.Services.AddDistributedMemoryCache();

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddRazorPages(options =>
            {
                options.Conventions.AddPageRoute("/RegisterError", "Register/{text?}");
                //options.Conventions.AddPageRoute("/LoginError", "Login/{text?}");

                options.Conventions.AddPageRoute("/Logedin", "Index/{text?}");

            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthorization();

            // Auto redirect to Game page for faster testing ****************
            /*app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/Game");
                    return;
                }
                await next();
            });*/
            // ***************************************************************


            app.MapRazorPages();


            app.Run();
        }
    }
}
