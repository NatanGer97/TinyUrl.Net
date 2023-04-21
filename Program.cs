using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver.Core.Connections;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StackExchange.Redis;
using System.Net;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Channels;
using TinyUrl.Middleware;
using TinyUrl.Models;
using TinyUrl.Services;
using TinyUrl.Services.interfaces;
using ZstdSharp.Unsafe;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "Students Dashboard" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Auth header using the Bearer schema",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"

    });
    options.OperationFilter<AuthOpertionFiler>();


});

// mapper
builder.Services.AddAutoMapper(typeof(Program));

// mongo
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<UserService>();
builder.Services.AddScoped<IUrlService,UrlService>();

// redis
builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("redis:6379"));
builder.Services.AddSingleton<IRedisService, RedisService>();

builder.Services.AddTransient<ExceptionMiddleware>();


// authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(jwt =>
{
    var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value);
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false, // for dev
        ValidateAudience = false, // for dev
        RequireExpirationTime = true, // for dev - need to be updated when refresh token is add
        ValidateLifetime = true,
    };
    
    jwt.Events = new JwtBearerEvents();
    jwt.Events.OnAuthenticationFailed = context =>
    {
        Console.WriteLine("OnAuthenticationFailed");
        context.Exception.GetBaseException();
        
        return Task.CompletedTask;

    };
    
    jwt.Events.OnChallenge = context =>
    {
        Console.WriteLine("OnChallenge");
        
        context.HandleResponse();
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";
        var result = JsonConvert.SerializeObject(new { error = "Unauthorized" , message = context.ErrorDescription,
        StatusCode = context.Response.StatusCode});
        return context.Response.WriteAsync(result);
    };

});


builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();


app.Run();
