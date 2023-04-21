using PeliculasAPi;

var builder = WebApplication.CreateBuilder(args);

//agrego la clase startup
var startup = new StartUp(builder.Configuration);

startup.ConfigureServices(builder.Services);


var app = builder.Build();

//agrego
startup.Configure(app, app.Environment);


app.Run();
