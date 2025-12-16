var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/health", () =>
{
    return Results.Ok(new { status = "ok" });
});

app.MapPost("/api/suma", async (HttpContext context) =>
{
    SumaRequest? request = null;
    try
    {
        request = await context.Request.ReadFromJsonAsync<SumaRequest>();
    }
    catch (System.Text.Json.JsonException)
    {
        // Invalid JSON or empty body
    }
    if (request == null || request.A == null || request.B == null)
    {
        return Results.BadRequest(new { error = "Invalid request body" });
    }
    int result = request.A.Value + request.B.Value;
    return Results.Ok(new { result = result });
});

app.Run();

public class SumaRequest
{
    public int? A { get; set; }
    public int? B { get; set; }
}

namespace DevSecOpsDemo.Api
{
    public partial class Program { }
}
