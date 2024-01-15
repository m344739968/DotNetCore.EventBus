using Autofac.Core;
using DotNetCore.EventBus;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEventBus(builder.Configuration, x =>
{
    // 
});

// 注入redis
var csredis = new CSRedis.CSRedisClient(builder.Configuration["Redis:ConnectionString"]);
RedisHelper.Initialization(csredis);
builder.Services.AddSingleton<IDistributedCache>(new Microsoft.Extensions.Caching.Redis.CSRedisCache(RedisHelper.Instance));

// 初始化数据库
var init = builder.Services.BuildServiceProvider().GetRequiredService<MysqlInitialization>();
await init.InitializeAsync(); //.ConfigureAwait(false).GetAwaiter();

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
