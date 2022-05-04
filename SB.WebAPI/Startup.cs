using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Core.IServices;
using Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SB.Domain.IRepositories;
using SB.Domain.Services;
using SB.EFCore;
using SB.EFCore.Repositories;
using SB.WebAPI.Middleware;
using SB.WebAPI.Util;

namespace SB.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "SB.WebAPI", Version = "v1"});
                
                var filePath = Path.Combine(System.AppContext.BaseDirectory, "SB.WebAPI.xml");
                c.IncludeXmlComments(filePath);
                
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme  
                {  
                    Name = "Authorization",  
                    Type = SecuritySchemeType.Http,  
                    Scheme = "basic",  
                    In = ParameterLocation.Header,  
                    Description = "Basic Authorization header using the Bearer scheme."  
                });  
                c.AddSecurityRequirement(new OpenApiSecurityRequirement  
                {  
                    {  
                        new OpenApiSecurityScheme  
                        {  
                            Reference = new OpenApiReference  
                            {  
                                Type = ReferenceType.SecurityScheme,  
                                Id = "basic"  
                            }  
                        },  
                        new string[] {}  
                    }  
                });  
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            
            //Application DB Context
            services.AddDbContext<SbContext>(opt =>
            {
                //We proudly use Microsoft SQL Server - and accept the EULA!!
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });
            
            //CORS Policy Setup
            services.AddCors(options =>
            {
                options.AddPolicy("Dev-cors", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
                options.AddPolicy("Prod-cors", policy =>
                {
                    policy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                } );
            });

            services.AddHttpContextAccessor();
            
            services.AddScoped<I_RW_Repository<Supporter>,SupporterRepository>();
            services.AddScoped<ISupporterService, SupporterService>();

            services.AddScoped<I_RW_Repository<Ticket>,TicketRepository>();
            services.AddScoped<ITicketService, TicketService>();

            services.AddScoped<I_RW_Repository<UserInfo>, UserInfoRepository>();

            services.AddScoped<I_RW_Repository<Answer>, AnswerRepository>();

            services.AddScoped<I_RW_Repository<LiveChat>, LiveChatRepository>();
            services.AddScoped<ILiveChatService, LiveChatService>();

            services.AddSingleton<IWebsocketHandler, WebsocketHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline..
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SB.WebAPI v1"));

                app.UseCors("Dev-cors");
            }
            else
            {
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SB.WebAPI v1"));
                app.UseCors("Prod-cors");
            }
            
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var ctx = scope.ServiceProvider.GetService<SbContext>();
                //ctx.Database.EnsureDeleted();
                ctx.Database.EnsureCreated();
            }

            app.UseWebSockets();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}