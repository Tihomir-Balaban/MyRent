using MyRent.Config;
using MyRent.Interfaces.Services;
using MyRent.Interfaces.Services.Shared;
using MyRent.Services;
using MyRent.Services.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<MyRentApiOptions>()
    .Bind(builder.Configuration.GetSection(MyRentApiOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.BaseUrl), "MyRentApi configuration is missing BaseUrl.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Guid), "MyRentApi configuration is missing Guid.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Token), "MyRentApi configuration is missing Token.")
    .ValidateOnStart();

builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();
builder.Services.AddDistributedMemoryCache();

builder.Services.AddScoped<IPropertyListService, PropertyListService>();
builder.Services.AddScoped<IMemoryCacheService, MemoryCacheService>();
builder.Services.AddHttpClient<IMyRentClientService, MyRentClientService>((sp, client) =>
{

    var options = sp
        .GetRequiredService<Microsoft.Extensions.Options.IOptions<MyRentApiOptions>>()
        .Value;

    client.BaseAddress = new Uri(options.BaseUrl);

    client.DefaultRequestHeaders.Add("guid", options.Guid);
    client.DefaultRequestHeaders.Add("token", options.Token);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();