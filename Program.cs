using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options
  => options.AddPolicy(name: "CorsPolicyName", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
builder.Services.AddHttpClient(); // Add HttpClient here
var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors();
app.MapGet("/", async ([FromQuery] string c, HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    string coords= c;
    var httpClient = httpClientFactory.CreateClient();
    var apiUrl = "https://cbsapi.tkgm.gov.tr/megsiswebapi.v3/api/parsel/"+coords.Replace(",","/").Replace(" ",""); // Replace with your API URL 41.143677/29.145663

    var response = await httpClient.GetAsync(apiUrl);

    if (response.IsSuccessStatusCode)
    {
        var content = await response.Content.ReadAsStringAsync();
        await context.Response.WriteAsync(content);
    }
    else
    {
        context.Response.StatusCode = (int)response.StatusCode;
        await context.Response.WriteAsync(response.ReasonPhrase);
    }
}).RequireCors("CorsPolicyName"); ;

app.Run();
