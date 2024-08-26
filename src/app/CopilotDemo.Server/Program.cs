using Api.Azure.OpenAi.Extensions;
using Api.Azure.Search.Extensions;
using Api.Extensions.Endpoints;
using Api.Extensions.ExceptionHandler;
using Api.Features.Extensions;

var builder = WebApplication.CreateBuilder(args);
#if DEBUG
builder.Configuration.AddJsonFile($"local.settings.json", optional: true);
builder.Configuration.AddJsonFile($"local.settings.{builder.Environment.EnvironmentName}.json", optional: true);
#endif
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName));
builder.Services.AddProblemDetailsExceptionHandler();

builder.Services.AddFeatures();
builder.Services.AddOpenAi().AddAiSearch();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapFeatureEndpoints();

app.MapFallbackToFile("/index.html");

app.Run();
