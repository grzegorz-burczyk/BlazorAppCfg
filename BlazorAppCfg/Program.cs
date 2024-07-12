using BlazorAppCfg.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<ICompanyInfoService, CompanyInfoService>();


builder.Configuration.AddJsonFile("companyinfo.json", optional: true, reloadOnChange: true);

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

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public interface ICompanyInfoService
{
    CompanyInfo GetCompanyInfo();
    Task SaveCompanyInfoAsync(CompanyInfo companyInfo);
}

public class CompanyInfoService : ICompanyInfoService
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _env;
    private const string ConfigFileName = "companyinfo.json";

    public CompanyInfoService(IConfiguration configuration, IWebHostEnvironment env)
    {
        _configuration = configuration;
        _env = env;
    }

    public CompanyInfo GetCompanyInfo()
    {
        var companyInfo = new CompanyInfo();
        _configuration.GetSection("CompanyInfo").Bind(companyInfo);
        return companyInfo;
    }

    public async Task SaveCompanyInfoAsync(CompanyInfo companyInfo)
    {
        var filePath = Path.Combine(_env.ContentRootPath, ConfigFileName);
        var json = await File.ReadAllTextAsync(filePath);
        var jsonObj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (jsonObj.ContainsKey("CompanyInfo"))
        {
            jsonObj["CompanyInfo"] = companyInfo;
        }
        else
        {
            jsonObj.Add("CompanyInfo", companyInfo);
        }

        var output = JsonSerializer.Serialize(jsonObj, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(filePath, output);
    }
}

public class CompanyInfo
{
    public string PhoneNumber { get; set; }
    public string Location { get; set; }
}