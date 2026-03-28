using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// VULNERABILITY 1: Hardcoded credentials (detected by SAST)
var connectionString = "Server=localhost;Database=mydb;User Id=admin;Password=admin123;";
var apiKey = "sk-prod-1234567890abcdef";

// VULNERABILITY 2: SQL Injection (detected by SAST)
app.MapGet("/users/{username}", (string username) =>
{
    // Directly interpolating user input into SQL query
    var query = $"SELECT * FROM Users WHERE Username = '{username}'";
    return Results.Ok(new { query });
});

// VULNERABILITY 3: Sensitive data exposure (detected by SAST)
app.MapGet("/config", () =>
{
    return Results.Ok(new
    {
        connectionString,
        apiKey,
        environment = Environment.GetEnvironmentVariables()
    });
});

// VULNERABILITY 4: No input validation
app.MapPost("/execute", ([FromBody] string command) =>
{
    var result = System.Diagnostics.Process.Start("bash", $"-c {command}");
    return Results.Ok("Executed");
});

app.Run();