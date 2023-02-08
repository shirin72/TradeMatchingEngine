using Application.Factories;
using Application.OrderService.OrderCommandHandlers;
using Infrastructure.Order.CommandRepositories;
using Infrastructure.Order.QueryRepositories;
using Infrastructure.Trade.QueryRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TradeMatchingEngine.Orders.Repositories.Command;
using TradeMatchingEngine.Orders.Repositories.Query;
using TradeMatchingEngine.Trades.Repositories.Command;
using TradeMatchingEngine.Trades.Repositories.Query;
using TradeMatchingEngine.UnitOfWork;

namespace Infrastructure
{
    public static class BusinessDependencies
    {
        public static IServiceCollection DependencyHolder(this IServiceCollection services)
        {
            services.AddDbContext<TradeMatchingEngineContext>(options =>
            {
                options.UseSqlServer("Server=.;Initial Catalog=TradeMatchingEngine;Integrated Security=true;Persist Security Info=False;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;");
            });

            services.AddScoped<IOrderCommandRepository, OrderCommandRepository>();
            services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
            services.AddScoped<IAddOrderCommandHandlers, AddOrderCommandHandlers>();
            services.AddScoped<ITradeCommandRepository, TradeCommandRepository>();
            //builder.Services.AddScoped(typeof(ICommandRepository<>), typeof(CommandRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ITradeQueryRespository, TradeQueryRepository>();
            services.AddSingleton<IStockMarketFactory, StockMarketFactory>();
            services.AddScoped<IModifieOrderCommandHandler, ModifieOrderCommandHandler>();
            services.AddScoped<ICancellOrderCommandHandler, CancellOrderCommandHandler>();
            services.AddScoped<ICancellAllOrdersCommandHandler, CancellAllOrdersCommandHandler>();
            return services;
        }
    }
}
