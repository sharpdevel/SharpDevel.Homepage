using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

#if RELEASE
builder.Configuration.AddAzureKeyVault(Environment.GetEnvironmentVariable("VaultUri"));
#endif

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages().AddViewLocalization();
builder.Services.AddControllers();

// The App Service front end proxies requests; without this the client IP seen by the
// rate limiter would be the proxy's, putting every visitor into one bucket.
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownNetworks.Clear();
	options.KnownProxies.Clear();
});

builder.Services.AddRateLimiter(options =>
{
	options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
	options.AddPolicy("email", context => RateLimitPartition.GetFixedWindowLimiter(
		context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
		_ => new FixedWindowRateLimiterOptions
		{
			PermitLimit = 3,
			Window = TimeSpan.FromMinutes(10)
		}));
});

builder.Services.AddHsts(options => options.MaxAge = TimeSpan.FromDays(365));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseForwardedHeaders();

app.Use(async (context, next) =>
{
	context.Response.Headers["X-Content-Type-Options"] = "nosniff";
	context.Response.Headers["X-Frame-Options"] = "DENY";
	context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
	await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

// Culture comes from the browser's Accept-Language header (query string ?culture=de wins for testing).
app.UseRequestLocalization(options => options
	.SetDefaultCulture("en")
	.AddSupportedCultures("en", "de")
	.AddSupportedUICultures("en", "de"));

app.UseRouting();

app.UseRateLimiter();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
