using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

/*Задание: REST API для управления расписанием стримов на ASP.NET Core
Цель:
Создать REST API, которое позволяет стримерам планировать стримы, а пользователям — просматривать расписание.

Функционал API:
1. Работа со стримами
Создать запланированный стрим (POST /streams)
Входные данные: title, streamerId, startTime
API возвращает streamId.
Получить список всех запланированных стримов (GET /streams)
Опционально: фильтрация по streamerId.
Получить информацию о конкретном стриме (GET /streams/{streamId}
Удалить запланированный стрим (DELETE /streams/{streamId})
Фильтрация по дате (например, стримы на сегодня).
Добавить статус (запланирован / завершён).
Отправка напоминаний (например, за 10 минут до начала стрима, но это потребует фоновых задач)*/


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Sqlite") ?? "Data Source=streams.db"));

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Streams API", Version = "v1" });
});

builder.Services.AddHostedService<ReminderService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.UseCors();
app.UseDefaultFiles(); // шукає index.html
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Streams API v1"));
}

var group = app.MapGroup("/streams");

group.MapPost("/", async (AppDbContext db, CreateStreamDto dto) =>
{
    var item = new RozkStr
    {
        Nazva = dto.Nazva,
        StreamerId = dto.StreamerId,
        Pochatok = dto.Pochatok.ToUniversalTime()
    };
    db.Streams.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/streams/{item.Id}", new { streamId = item.Id });
});

group.MapGet("/", async (AppDbContext db, int? streamerId, string? date, bool? today) =>
{
    var q = db.Streams.AsQueryable();

    if (streamerId.HasValue && streamerId.Value > 0)
        q = q.Where(x => x.StreamerId == streamerId.Value);

    if (today == true)
    {
        var nowUtc = DateTime.UtcNow;
        var start = new DateTime(nowUtc.Year, nowUtc.Month, nowUtc.Day, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddDays(1);
        q = q.Where(x => x.Pochatok >= start && x.Pochatok < end);
    }
    else if (!string.IsNullOrWhiteSpace(date) && DateTime.TryParse(date, out var d))
    {
        var start = DateTime.SpecifyKind(d.Date, DateTimeKind.Utc);
        var end = start.AddDays(1);
        q = q.Where(x => x.Pochatok >= start && x.Pochatok < end);
    }

    var list = await q.OrderBy(x => x.Pochatok).ToListAsync();
    return Results.Ok(list);
});

group.MapGet("/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var item = await db.Streams.FindAsync(id);
    return item is null ? Results.NotFound() : Results.Ok(item);
});

group.MapDelete("/{id:guid}", async (AppDbContext db, Guid id) =>
{
    var item = await db.Streams.FindAsync(id);
    if (item is null) return Results.NotFound();
    db.Streams.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

group.MapPatch("/{id:guid}/status", async (AppDbContext db, Guid id, UpdateStatusDto body) =>
{
    var item = await db.Streams.FindAsync(id);
    if (item is null) return Results.NotFound();
    item.Status = body.Status;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

app.Run();