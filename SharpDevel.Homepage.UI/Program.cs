var builder = WebApplication.CreateBuilder(args);

#if RELEASE
builder.Configuration.AddAzureKeyVault(Environment.GetEnvironmentVariable("VaultUri"));
#endif

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages().AddViewLocalization();
builder.Services.AddControllers();

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

// Culture comes from the browser's Accept-Language header (query string ?culture=de wins for testing).
app.UseRequestLocalization(options => options
	.SetDefaultCulture("en")
	.AddSupportedCultures("en", "de")
	.AddSupportedUICultures("en", "de"));

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();
