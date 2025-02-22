using System.Net;
using Application.Interfaces;
using Application.Services;
using Application.Services;
using Infrastructure;
using MyFramework;
using MyFramework.TemplateGenerator;
using MyORM;
using MyORM.interfaces;
using Persistence.interfaces;
using Persistence.Repositories;

var serviceProvider = new MyServiceCollection();

serviceProvider.AddSingleton<IQueryMapper, QueryMapper>(() =>
    new QueryMapper(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!));

serviceProvider.AddSingleton<IS3Storage<string>, MinioService>(() => 
                new MinioService("image-bucket", 
        Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY")!,
        Environment.GetEnvironmentVariable("MINIO_SECRET_KEY")!));

serviceProvider.AddSingleton<IPasswordHasher, PasswordHasher>();

serviceProvider.AddSingleton<IUserRepository,UserRepository>();
serviceProvider.AddSingleton<UserService>();

serviceProvider.AddSingleton<IArtistRepository,ArtistRepository>();
serviceProvider.AddSingleton<ArtistService>();

serviceProvider.AddSingleton<IArtworkRepository,ArtworkRepository>();
serviceProvider.AddSingleton<ArtworkService>();

serviceProvider.AddSingleton<IMarketRepository,MarketRepository>();
serviceProvider.AddSingleton<MarketService>();

serviceProvider.AddSingleton<ISavingFavouriteRepository,SavingFavouriteRepository>();
serviceProvider.AddSingleton<SavingFavouriteService>();

serviceProvider.AddSingleton<AccountService>();
serviceProvider.AddSingleton<FileService>();

var authService = new AuthorizationService();

var templateGenerator = new TemplateGenerator();
var errorSender = new ErrorSender(templateGenerator);
var routeHandler = new RouteHandler(serviceProvider, errorSender, authService);

var app = new App(routeHandler);
await app.StartAsync();
