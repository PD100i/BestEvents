using Events.Application;
using Events.Infrastructure;
using Events.Presentation;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation(builder.Configuration);

builder.Services.AddControllers();


var app = builder.Build();

app.Use(async (context, next) =>
{
   
    var authHeader = context.Request.Headers["Authorization"].ToString();

    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
    {
        try
        {
            var token = authHeader.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Выводим claims в консоль отладки
            System.Diagnostics.Debug.WriteLine($"[DEBUG JWT] User: {jwtToken.Subject}");
            foreach (var claim in jwtToken.Claims)
            {
                System.Diagnostics.Debug.WriteLine($"  {claim.Type}: {claim.Value}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG JWT] Invalid token format: {ex.Message}");
        }
    }
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandler();

app.Use(async (context, next) =>
{
    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
    {
        try
        {
            var token = authHeader.Substring(7);
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Выводим claims в консоль отладки
            System.Diagnostics.Debug.WriteLine($"[DEBUG JWT] User: {jwtToken.Subject}");
            foreach (var claim in jwtToken.Claims)
            {
                System.Diagnostics.Debug.WriteLine($"  {claim.Type}: {claim.Value}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG JWT] Invalid token format: {ex.Message}");
        }
    }
    await next();
});




app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapControllers();

app.Run();
