using NoteApi.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using FluentValidation;
using NoteApi.V1.Models.Requests;
using NoteApi.V1.Models.Requests.Validators;
using NoteApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Note API", Version = "v1" });

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

builder.Services.AddScoped<IValidator<CreateNoteRequestModel>, CreateNoteRequestModelValidator>();
builder.Services.AddScoped<IValidator<UpdateNoteRequestModel>, UpdateNoteRequestModelValidator>();

builder.Services.AddDbContext<NoteDbContext>(opt => opt.UseInMemoryDatabase(databaseName: "NoteDb"));

var app = builder.Build();

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

app.MapControllers();

app.Run();
