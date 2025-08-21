
/*Задание:
Создай маршрут /user/{id}, который возвращает JSON-объект с фиктивными данными пользователя
(например, { "id": 1, "name": "John Doe", "email": "john@example.com" }).

Создай маршрут /echo, который принимает JSON в POST-запросе и возвращает его обратно в ответе. Используй MapPost.

Добавь маршрут /sum, который принимает два числа через QueryString (?a=5&b=10) и возвращает их сумму.

Создай кастомный миддлвар, который логирует все входящие HTTP-запросы перед их обработкой Map.

Добавь в Map зависимость, например, сервис IGreetingService, который будет возвращать разные приветствия в зависимости от времени суток.

Создай маршрут /headers, который возвращает JSON с заголовками текущего запроса.

Создай список объектов (например, товары) и реализуй маршрут /products,
который принимает параметр category в QueryString и фильтрует список товаров по категории.*/

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IGreetingService, GreetingService>();//вітання

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors("AllowAll");

app.Use(async (context, next) =>//логування
{
    Console.WriteLine($"[{DateTime.Now}] {context.Request.Method} {context.Request.Path}");
    await next.Invoke();
});

app.MapGet("/user/{id}", (int id) =>//  /user/{id}
{
    var user = new { id, name = "John Doe", email = "john@example.com" };
    return Results.Json(user);
});

app.MapPost("/echo", async (HttpRequest request) =>// /echo (POST)
{
    using var reader = new StreamReader(request.Body);
    var body = await reader.ReadToEndAsync();
    var json = JsonSerializer.Deserialize<object>(body);
    return Results.Json(json);
});

app.MapGet("/sum", (int a, int b) =>// /sum?a=5&b=10
{
    return Results.Json(new { result = a + b });
});

app.MapGet("/greet", (IGreetingService greetingService) =>// /greet (приклад)
{
    return Results.Json(new { message = greetingService.GetGreeting() });
});

app.MapGet("/headers", (HttpRequest request) =>//  /headers
{
    var headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
    return Results.Json(headers);
});

var products = new List<Product> // Список товаров и /products?category=Electronics

{
    new Product { Id = 1, Name = "Laptop", Category = "Electronics" },
    new Product { Id = 2, Name = "Phone", Category = "Electronics" },
    new Product { Id = 3, Name = "Table", Category = "Furniture" },
    new Product { Id = 4, Name = "Chair", Category = "Furniture" },
};

app.MapGet("/products", (string? category) =>
{
    var filtered = string.IsNullOrEmpty(category)
        ? products
        : products.Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
    return Results.Json(filtered);
});

app.Run();

// Моделі и сервіс
public record Product
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Category { get; init; }
}

public interface IGreetingService
{
    string GetGreeting();
}

public class GreetingService : IGreetingService
{
    public string GetGreeting()
    {
        var hour = DateTime.Now.Hour;
        if (hour < 9) return "Доброго ранку!";
        if (hour < 18) return "Доброго дня!";
        return "Доброго вечора!";
    }
}

