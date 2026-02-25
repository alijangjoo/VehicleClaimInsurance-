using Elastic.Apm.Api;
using Elastic.Apm.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddElasticApmForAspNetCore();

/// TODO: موقتا غیر فعال شده است
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = "your-issuer",
//             ValidAudience = "your-audience",
//             IssuerSigningKey = new SymmetricSecurityKey(
//                 Encoding.UTF8.GetBytes("your-secret-key-minimum-32-characters-long"))
//         };
//     });

// TODO: موقتا غیرفعال شده است
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Authenticated", policy =>
//        policy.RequireAuthenticatedUser());
//});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(transforms =>
    {
        transforms.AddRequestTransform(context =>
        {
            context.ProxyRequest.Headers.Remove("traceparent");
            context.ProxyRequest.Headers.Remove("tracestate");
            return ValueTask.CompletedTask;
        });

        transforms.AddRequestTransform(async context =>
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring("Bearer ".Length).Trim();
                    context.ProxyRequest.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
        });
    })
    .ConfigureHttpClient((context, handler) =>
    {
        handler.ActivityHeadersPropagator = DistributedContextPropagator.CreateDefaultPropagator();
    });

builder.Services.AddHealthChecks();
var app = builder.Build();

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

app.UseElasticApm();

// موقتا غیرفعال شده است
//app.UseAuthentication();
//app.UseAuthorization();

app.MapHealthChecks("/health");

app.MapReverseProxy();

app.MapGet("/", () => "Yarp API Gateway with Elastic APM + Tracing");
app.Run();