using System.Text;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Web;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["JwtKey"] ?? throw new InvalidOperationException("JwtKey is not configured.");
var jwtIssuer = builder.Configuration["JwtIssuer"] ?? throw new InvalidOperationException("JwtIssuer is not configured.");
var jwtAudience = builder.Configuration["JwtAudience"] ?? throw new InvalidOperationException("JwtAudience is not configured.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException("JwtKey must be at least 32 bytes for HS256.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };

    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddDependencyInjection();
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthDb")));
var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.UseCors("Frontend");
app.UseAuthentication();
    

app.UseAuthorization();
app.MapControllers();
app.MapHub<Web.Hubs.AuthHub>("/hubs/auth");
app.UseHttpsRedirection();





app.Run();
