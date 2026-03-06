using Elastic.Apm.NetCoreAll;
using FluentValidation;
using Insurance.CacheService.Endpoints.Api.Models.Configuration;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.GetFromCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.RemoveFromCache;
using Insurance.CacheService.Endpoints.Api.Models.Dtos.SetToCache;
using Insurance.CacheService.Endpoints.Api.Validators;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Serilog;
using System.Diagnostics;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    // Add Elastic APM
    builder.Services.AddAllElasticApm();

    builder.Services.Configure<CacheOptions>(
        builder.Configuration.GetSection(CacheOptions.SectionName));

    builder.Services.AddStackExchangeRedisCache(options =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Redis");
        options.Configuration = connectionString;
        options.InstanceName = "CacheApi";
    });

    builder.Services.AddValidatorsFromAssemblyContaining<GetFromCacheRequestValidator>();

    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? ["*"];
            var allowedMethods = builder.Configuration.GetSection("Cors:AllowedMethods").Get<string[]>() ?? ["*"];
            var allowedHeaders = builder.Configuration.GetSection("Cors:AllowedHeaders").Get<string[]>() ?? ["*"];

            if (allowedOrigins.Contains("*"))
                policy.AllowAnyOrigin();
            else
                policy.WithOrigins(allowedOrigins);

            if (allowedMethods.Contains("*"))
                policy.AllowAnyMethod();
            else
                policy.WithMethods(allowedMethods);

            if (allowedHeaders.Contains("*"))
                policy.AllowAnyHeader();
            else
                policy.WithHeaders(allowedHeaders);
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Cache API",
            Version = "v1",
            Description = "A high-performance Redis-based caching API"
        });
    });

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
    });

    var app = builder.Build();

    // Configure W3C Activity format for distributed tracing
    Activity.DefaultIdFormat = ActivityIdFormat.W3C;
    Activity.ForceDefaultIdFormat = true;

    // Use Elastic APM
    app.UseAllElasticApm(builder.Configuration);

    app.UseResponseCompression();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cache API V1");
            c.RoutePrefix = string.Empty;
        });
    }

    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
            diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress);
        };
    });

    app.UseHttpsRedirection();
    app.UseCors();

    app.MapPost("/cache/get", async (GetFromCacheRequest request, IDistributedCache cache, IValidator<GetFromCacheRequest> validator) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        try
        {
            var value = await cache.GetStringAsync(request.Key);
            Log.Information("Cache GET operation for key: {Key}, Found: {Found}", request.Key, value != null);
            return Results.Ok(new GetFromCacheResponse(value));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error getting cache value for key: {Key}", request.Key);
            throw;
        }
    });

    app.MapPost("/cache/set", async (SetToCacheRequest request, IDistributedCache cache, IValidator<SetToCacheRequest> validator, IOptions<CacheOptions> cacheOptions) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        try
        {
            var options = new DistributedCacheEntryOptions();

            if (request.AbsoluteExpirationMinutes.HasValue)
            {
                options.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(request.AbsoluteExpirationMinutes.Value);
            }
            else
            {
                options.AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(cacheOptions.Value.DefaultExpirationMinutes);
            }

            if (request.SlidingExpirationMinutes.HasValue)
            {
                options.SlidingExpiration = TimeSpan.FromMinutes(request.SlidingExpirationMinutes.Value);
            }

            await cache.SetStringAsync(request.Key, request.Value, options);
            Log.Information("Cache SET operation for key: {Key}, AbsoluteExpirationMinutes: {AbsoluteExpirationMinutes}, SlidingExpirationMinutes: {SlidingExpirationMinutes}",
                request.Key, request.AbsoluteExpirationMinutes ?? cacheOptions.Value.DefaultExpirationMinutes, request.SlidingExpirationMinutes);
            return Results.Ok(new SetToCacheResponse(true));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error setting cache value for key: {Key}", request.Key);
            throw;
        }
    });

    app.MapPost("/cache/remove", async (RemoveFromCacheRequest request, IDistributedCache cache, IValidator<RemoveFromCacheRequest> validator) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
        }

        try
        {
            await cache.RemoveAsync(request.Key);
            Log.Information("Cache REMOVE operation for key: {Key}", request.Key);
            return Results.Ok(new RemoveFromCacheResponse(true));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error removing cache value for key: {Key}", request.Key);
            throw;
        }
    });

    app.MapGet("/cache/stats", () =>
    {
        return Results.Ok(new
        {
            ServerTime = DateTime.UtcNow,
            Version = "1.0.0",
            Status = "Healthy"
        });
    });

    Log.Information("Cache API started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}