using System;
using Microsoft.AspNetCore.Builder;        
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagement.Data;
using Westwind.AspNetCore.Markdown;


var builder = WebApplication.CreateBuilder(args);

// DATABASE
builder.Services.AddDbContext<DataContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(cs);
});


//Expose DbContext through interface
builder.Services.AddScoped<IDataContext>(sp => sp.GetRequiredService<DataContext>());

// Register abstractions for DI (extension method)
builder.Services.AddDomainServices();


//UI + MVC + HTTP CLIENT
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers();

builder.Services.AddHttpClient("ServerAPI", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["AppBaseUrl"] ?? "https://localhost:7084/");
});
builder.Services.AddMarkdown();

// BUILD & MIDDLEWARE PIPELINE
var app = builder.Build();

if (!app.Environment.IsDevelopment())
    app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseMarkdown();

// Endpoints
app.MapControllers();
app.MapRazorPages();      
app.MapBlazorHub();       
app.MapFallbackToPage("/_Host"); // ⬅ fallback to Blazor at root


app.Run();
