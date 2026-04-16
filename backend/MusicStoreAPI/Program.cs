using Microsoft.AspNetCore.HttpOverrides;
using MusicStoreAPI.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<SongService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapGet("/", () => Results.Ok(new { message = "API is running" }));
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
Console.WriteLine($"API started on http://0.0.0.0:{port}");
Console.WriteLine($"Swagger: http://0.0.0.0:{port}/swagger");
Console.WriteLine($"Songs endpoint: http://0.0.0.0:{port}/api/songs");

app.Run($"http://0.0.0.0:{port}");
