using Microsoft.AspNetCore.Http;

/*Разработать middleware, которое динамически добавляет баннер в начало каждой HTML-страницы,
возвращаемой сервером.

Требования:
Middleware должно изменять тело HTML-ответа перед отправкой клиенту.
Вставлять <div> с текстом "Добро пожаловать! Сегодня скидка 10%!" перед тегом <body>.*/



var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Middleware для додавання банера
app.Use(async (context, next) =>
{
    // Зберігаємо оригінальний потік
    var origStan = context.Response.Body;

    using (var memoryStream = new MemoryStream())
    {
        context.Response.Body = memoryStream; // Перенаправляємо потік

        await next(); // Виконуємо наступний middleware або обробник запиту

        // Перевіряємо, чи відповідає HTML
        if (context.Response.ContentType != null && context.Response.ContentType.Contains("text/html"))
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memoryStream);
            string originalHtml = await reader.ReadToEndAsync();

            // Додаємо банер перед <body>
            string modifiedHtml = dodBan(originalHtml);

            context.Response.Body = origStan; // Повертаємо потік
            context.Response.ContentLength = modifiedHtml.Length; // Виправляємо довжину
            await context.Response.WriteAsync(modifiedHtml);
        }
        else
        {
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(origStan);
        }
    }
});

// Обробник основних маршрутів
app.Run(async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    if (context.Request.Path == "/postuser" && context.Request.Method == "POST")
    {
        var form = await context.Request.ReadFormAsync();
        string name = form["name"];
        string age = form["age"];

        // Формуємо динамічну HTML-сторінку
        string vidpovidHtml = $"<html><body><div><p>Ім'я: {name}</p><p>Вік: {age}</p></div></body></html>";

        await context.Response.WriteAsync(vidpovidHtml);
    }
    else
    {
        await context.Response.SendFileAsync("html/index.html");
    }
});

app.Run();

string dodBan(string html)
{
    string banner = "<div style='background-color: lightblue; text-align: center;'>Добро пожаловать! Сегодня скидка 10%!</div>";
    string bodyTag = "<body";
    int bodyIndex = html.IndexOf(bodyTag, StringComparison.OrdinalIgnoreCase);

    if (bodyIndex >= 0)
    {
        int bodyEndIndex = html.IndexOf(">", bodyIndex);
        if (bodyEndIndex > bodyIndex)
        {
            // Вставляємо банер одразу після <body>
            html = html.Insert(bodyEndIndex + 1, banner);
        }
    }

    return html;
}
