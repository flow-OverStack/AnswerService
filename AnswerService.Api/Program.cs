using AnswerService.Api;
using AnswerService.Api.Middlewares;
using AnswerService.Api.Settings;
using AnswerService.Application.DependencyInjection;
using AnswerService.Cache.Settings;
using AnswerService.DAL.DependencyInjection;
using AnswerService.GrpcClient.DependencyInjection;
using AnswerService.Messaging.DependencyInjection;
using AnswerService.Messaging.Settings;
using AnswerService.Outbox.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KeycloakSettings>(builder.Configuration.GetSection(nameof(KeycloakSettings)));
builder.Services.Configure<KafkaSettings>(builder.Configuration.GetSection(nameof(KafkaSettings)));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection(nameof(RedisSettings)));

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddControllers();
builder.Services.AddLocalization(options => options.ResourcesPath = nameof(AnswerService.Application.Resources));

builder.Services.AddAuthenticationAndAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddGrpcClients();
builder.Services.AddMassTransitServices();
builder.Services.AddOutbox();

builder.Host.AddLogging(builder.Configuration);

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication();

builder.AddOpenTelemetry();
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddCors(builder.Configuration, builder.Environment);

var app = builder.Build();

app.UseStatusCodePages();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<WarningHandlingMiddleware>();

app.UseRouting();
app.MapControllers();
app.UseLocalization();
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.MapHealthChecks("health", new HealthCheckOptions { ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse });
app.UseForwardedHeaders(builder.Configuration);
app.UseCors("DefaultCorsPolicy");


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
    await app.Services.MigrateDatabaseAsync();
}

app.UseSwagger();

app.LogListeningUrls();

await app.RunAsync();