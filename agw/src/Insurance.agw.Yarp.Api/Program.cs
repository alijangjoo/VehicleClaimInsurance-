using Elastic.Apm.Api;
using Elastic.Apm.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo  // This will now work
    {
        Title = "API Gateway",
        Version = "v1",
        Description = "YARP API Gateway with Swagger documentation for all downstream services"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
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
            },
            new List<string>()
        }
    });

    // Optional: Add XML comments if you have them
    // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

builder.Services.AddElasticApmForAspNetCore();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "your-issuer",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "your-secret-key-minimum-32-characters-long"))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Authenticated", policy =>
        policy.RequireAuthenticatedUser());
});

var apiKeys = builder.Configuration.GetSection("ApiKeys");

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transformBuilderContext =>
    {
        // Correct way to access route ID
        var routeId = transformBuilderContext.Route.RouteId;
        var apiKeys = builder.Configuration.GetSection("ApiKeys");
        var apiKey = apiKeys[routeId];

        if (!string.IsNullOrEmpty(apiKey))
        {
            transformBuilderContext.AddRequestTransform(async context =>
            {
                context.ProxyRequest.Headers.Add("X-API-Key", apiKey);

                // Forward Bearer token
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    context.ProxyRequest.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            });
        }
    })
    .ConfigureHttpClient((context, handler) =>
    {
        handler.ActivityHeadersPropagator = DistributedContextPropagator.CreateDefaultPropagator();
    });

builder.Services.AddHealthChecks();
var app = builder.Build();

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway v1");
        c.RoutePrefix = "swagger"; // Serves at https://localhost:5001/swagger
    });
}

app.UseElasticApm();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapReverseProxy();

app.MapGet("/", () => "Yarp API Gateway with Elastic APM + Tracing");
app.Run();