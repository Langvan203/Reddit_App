using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Mapper;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net.WebSockets;
using System.Net;
using Reddit_App.Helpers;
using Microsoft.AspNetCore.WebSockets;
using Reddit_App.Helpers.SocketHelper;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// add authenciate
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Reddit_App", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "insert token: ",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
               Reference = new OpenApiReference
               {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
               }
               
            },new string[]{}
        }
    });
});



// add CORS
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

// get apiOption in appsetting
var apiOption = builder.Configuration.GetSection("ApiOptions").Get<ApiOptions>();
builder.Services.AddSingleton(apiOption);

// Add Authencation

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.RequireHttpsMetadata = false;
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = apiOption.ValidAudience,
        ValidIssuer = apiOption.ValidIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(apiOption.Secret))
    };
});

// connect to db
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    //var connectString = builder.Configuration.GetConnectionString("DbReddit_App");
    //options.UseSqlServer(connectString);
    options.UseSqlServer(apiOption.StringConnection);
});

// Add automapper
var mapperConfig = new MapperConfiguration(mapperConfig =>
{
    mapperConfig.AddProfile(new MappingContext());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

//builder.Services.AddSingleton<ConnectionManager>();
//builder.Services.AddSingleton<WebSocketHandle>();


var app = builder.Build();





if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors();

app.UseStaticFiles();

app.UseRouting();

// add websocket

//var webSocketOptions = new WebSocketOptions()
//{
//    KeepAliveInterval = TimeSpan.FromSeconds(10),
//    ReceiveBufferSize = 4 * 1024
//};

//app.UseWebSockets(webSocketOptions);
//var sendNoti = app.Services.GetService<SendNotiHandler>();
//app.UseMiddleware<WebSocketMiddlewareCustom>(sendNoti);


// swaggerUI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.DefaultModelsExpandDepth(-1);
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Reddit App V1");
    //
    c.DocumentTitle = "Reddit APP API";
    c.RoutePrefix = string.Empty;
});

//app.MapHub<DetectNewEvent>("/notificationHub");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
