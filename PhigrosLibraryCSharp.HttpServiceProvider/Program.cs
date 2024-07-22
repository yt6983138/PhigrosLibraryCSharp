using PhigrosLibraryCSharp.HttpServiceProvider.Dependency;

namespace PhigrosLibraryCSharp.HttpServiceProvider;

public class Program
{
	public static void Main(string[] args)
	{
		WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddControllersWithViews();

		builder.Services.Configure<Config>(
			builder.Configuration.GetSection("PhigrosDataConfig"));

		builder.Services.AddSingleton<PhigrosData>();

		WebApplication app = builder.Build();

		// Configure the HTTP request pipeline.
		if (!app.Environment.IsDevelopment())
		{
			// app.UseExceptionHandler("/Index");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}
		app.MapControllers().AllowAnonymous();

		app.UseHttpsRedirection();
		//app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		//app.MapControllerRoute(
		//	name: "default",
		//	pattern: "{controller=Home}/{action=GetNewQrcode}");

		app.Run();
	}
}
