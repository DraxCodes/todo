﻿using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToDo.Entities;
using ToDo.WebApi.Entities;
using ToDo.WebApi.Helpers;
using ToDo.WebApi.Repositories;
using ToDo.WebApi.Services;
using ToDo.WebApi.Storage;

namespace ToDo.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var defaultUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "todoist",
                Password = PasswordStorage.CreateHash("todoist")
            };

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IReadonlyStorage<User>>(provider => new JsonReadonlyStorage<User>(defaultUser));
            services.AddScoped<IReadWriteStorage<TodoList>, JsonReadWriteStorage<TodoList>>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITodoListRepository, TodoListRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            
            app.UseMvc();
        }
    }
}
