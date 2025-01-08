using System.Net;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using Infrastructure;
using MyFramework;
using MyORM;
using Persistence.Repositories;

var httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:5050/");
httpListener.Start();

var serviceProvider = new MyServiceCollection();

serviceProvider.AddSingleton<QueryMapper>();
serviceProvider.AddSingleton<UserRepository>();
serviceProvider.AddSingleton<ArtistRepository>();
serviceProvider.AddSingleton<ArtworkRepository>();
serviceProvider.AddSingleton<MarketRepository>();
serviceProvider.AddSingleton<SavingFavouriteRepository>();
serviceProvider.AddSingleton<UserService>();
serviceProvider.AddSingleton<AccountService>();
serviceProvider.AddSingleton<FileService>();
serviceProvider.AddSingleton<ArtistService>();
serviceProvider.AddSingleton<ArtworkService>();
serviceProvider.AddSingleton<MarketService>();
serviceProvider.AddSingleton<SavingFavouriteService>();
serviceProvider.AddSingleton<IS3Storage<string>,MinioService>();
serviceProvider.AddSingleton<IPasswordHasher, PasswordHasher>();


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



