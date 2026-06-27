using PublicHolidaysExporter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.AddScoped<IOpenHolidaysService, OpenHolidaysService>();
builder.Services.AddHttpClient<IOpenHolidaysService, OpenHolidaysService>(client => { client.BaseAddress = new Uri("https://openholidaysapi.org/"); });
builder.Services.AddScoped<ICsvExportService, CsvExportService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Holidays}/{action=Index}/{id?}");

app.Run();
