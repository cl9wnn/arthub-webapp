﻿using System.Net;
using MyFramework;
using MyORM;
using Npgsql;
using Persistence;
using Persistence.Entities;
using Persistence.Repositories;
using WebAPI;
using WebAPI.Services;

var httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:5050/");
httpListener.Start();

var serviceProvider = new MyServiceCollection();

serviceProvider.AddSingleton<QueryMapper>();
serviceProvider.AddSingleton<UserRepository>();
serviceProvider.AddSingleton<UserService>();
serviceProvider.AddSingleton<IS3Storage<string>,MinioService>();
serviceProvider.AddSingleton<AccountService>();

var authService = new AuthorizationService(); 

var routeHandler = new RouteHandler(serviceProvider, authService);

while (httpListener.IsListening)
{
    var context = await httpListener.GetContextAsync();

    _ = Task.Run(async () =>
    {
        await routeHandler.HandleRequest(context);
        context.Response.OutputStream.Close();
        context.Response.Close();
    });
}
httpListener.Stop();
httpListener.Close();



