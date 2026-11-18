
using LibraryManagement.Core.Data;
using LibraryManagement.Core.Exceptions;
using LibraryManagement.Core.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using LibraryManagement.core.Services;
using Microsoft.AspNetCore.Identity;
using LibraryManagement.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<LibraryService>();
builder.Services.AddScoped<BookData>();
builder.Services.AddScoped<DbConnection>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<AppUser>, PasswordHasher<AppUser>>();
builder.Services.AddScoped<JwtService>();

//AddDbContext، کلاس LibraryDbContext را در سیستم Dependency Injection ثبت می‌کند.
//GetConnectionString("LibraryDB")، اتصال تعریف‌شده در appsettings.json را می‌خواند.
//UseSqlServer(...)، مشخص می‌کند EF Core از SQL Server استفاده کند.
builder.Services.AddDbContext<LibraryDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDB")));

var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["key"]
    ?? throw new InvalidOperationException("JWT key is missing.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience= jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey))
        };
    }
    );
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token"
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services.AddAuthorization();

var app = builder.Build();


app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features
            .Get<IExceptionHandlerPathFeature>()?.Error;

        var statusCode = exception switch
        {
            MemberNotFoundException => 404,
            BookNotFoundException => 404,
            MemberInactiveException => 400,
            BookNotAvailableException => 400,
            LastAdminCannotBeDemotedException => 409,
            _ => 500
        };

        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            message = exception?.Message ?? "An unexpected error occurred."
        });
    });
});




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "Library Management API v1");
    });
}

app.UseHttpsRedirection();



app.MapControllers();

app.Run();

//git branch test
