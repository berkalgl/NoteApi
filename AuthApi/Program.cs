using AuthApi.Configuration;
using AuthApi.Data;
using AuthApi.Filters;
using AuthApi.V1.Models.Requests;
using AuthApi.V1.Models.Requests.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SecuritySettings>(builder.Configuration.GetSection("SecuritySettings"));
SecuritySettings securitySettings = new SecuritySettings();
builder.Configuration.GetSection("SecuritySettings").Bind(securitySettings);

builder.Services.AddApiVersioning(options => { options.ReportApiVersions = true; });

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers(options => options.Filters.Add(new ProblemDetailsExceptionFilter()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
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
                        Type=ReferenceType.SecurityScheme,
                        Id="Bearer"
                    }
                },
            new string[]{}
        }
    });

    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Auth API", Version = "v1" });

    c.TagActionsBy(api =>
    {
        if (api != null)
        {
            return new[] { api.GroupName };
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint");
    });

    c.DocInclusionPredicate((_, _) => true);
});

builder.Services.AddScoped<IValidator<LoginRequestModel>, LoginRequestModelValidator>();
builder.Services.AddScoped<IValidator<UpdateUserRequestModel>, UpdateUserRequestModelValidator>();

builder.Services.AddDbContext<UserDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "UserDb"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateActor = true,
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidAudience = securitySettings.Audience,
            ValidIssuer = securitySettings.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.Secret))
        };
    });

builder.Services.AddCors(option =>
{
    option.AddPolicy("allow", builder =>
    {
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
        // two different sub domains are two different origins
        builder.AllowAnyOrigin();
    });
});

var app = builder.Build();

var dbInstance = app.Services.CreateScope().ServiceProvider.GetRequiredService<UserDbContext>();
SeedData.Initialize(dbInstance);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseCors("allow");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
