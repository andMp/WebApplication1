var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

//Создать форму которая будет хранить в себе тесты и ответы на эти тесты будет разного вида. Текст, радио батон и т.д.
//Как вариант принимать несколько правильных ответов и т.д и после отправки формы вывести количество правильных ответ 

app.Run(async (context) =>
{
    context.Response.ContentType = "text/html; charset=utf-8";

    if (context.Request.Path == "/postuser" && context.Request.Method == "POST")
    {
        var form = await context.Request.ReadFormAsync();
        int pravilno = 0;

        if (form["q1"] == "7") pravilno++;
        if (form["q2"] == "Київ") pravilno++;

        string[] prav = { "Сонце - це зірка", "Місяць - супутник Землі" };
        var obrani = form["q3"].ToArray();
        if (prav.All(obrani.Contains) && obrani.Length == prav.Length)
            pravilno++;

        var answer = form["q4"].ToString().Trim().ToLower();
        if (answer == "синій" || answer == "блакитний") pravilno++;

        if (form["q5"] == "8") pravilno++;

        await context.Response.WriteAsync($"<h2>Правильних відповідей: {pravilno} з 5!</h2>");
    }
    else
    {
        await context.Response.SendFileAsync("html/index.html");
    }
});

app.Run();



