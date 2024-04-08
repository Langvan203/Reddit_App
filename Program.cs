using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Reddit_App.Common;
using Reddit_App.Database;
using Reddit_App.Mapper;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DatabaseContext>(options =>
{
    var connectString = builder.Configuration.GetConnectionString("DbReddit_App");
    options.UseSqlServer(connectString);
    //options.UseSqlServer(apiOption.StringConnection);
});
var mapperConfig = new MapperConfiguration(mapperConfig =>
{
    mapperConfig.AddProfile(new MappingContext());
});
IMapper mapper = mapperConfig.CreateMapper();
builder.Services.AddSingleton(mapper);
// get apioption in appseting
//var apiOption = builder.Configuration.GetSection("ApiOptions").Get<ApiOptions>();
//builder.Services.AddSingleton(apiOption);
var app = builder.Build();

// connect to db


// Add automapper


// Configure the HTTP request pipeline.

// comment to deploy
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
