using Application.OrderService.OrderCommandHandlers;
using Infrastructure;
using Infrastructure.Order.CommandRepositories;
using Infrastructure.Order.QueryRepositories;
using Infrastructure.Trade.QueryRepositories;
using Microsoft.EntityFrameworkCore;
using TradeMatchingEngine;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<TradeMatchingEngineContext>(options =>
    {
        options.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngine;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");
    }
);
//builder.Services.AddScoped<ITrade, Trade>();
builder.Services.AddScoped<IOrderCommand, OrderCommandRepository>();
builder.Services.AddScoped<IOrderQuery, OrderQueryRepository>();
builder.Services.AddScoped<IAddOrderCommandHandlers, AddOrderCommandHandlers>();
builder.Services.AddScoped<ITradeCommand, TradeCommandRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    //// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints => endpoints.MapControllers());
app.UseAuthorization();

//app.MapRazorPages();

app.Run();
