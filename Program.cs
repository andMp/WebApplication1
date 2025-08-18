using WebApplication1.Services;

/*Задание:
Создать интерфейс ILoggerService с методом void Log(string message).
Реализовать ConsoleLoggerService, который пишет логи в Console.WriteLine().
Создать интерфейс IUserService с методом string GetUserById(int id).
Реализовать UserService, который:
Получает ILoggerService через DI.
В методе GetUserById(id) сначала логирует обращение, затем возвращает User{id}.
Зарегистрировать сервисы ILoggerService и IUserService в DI.
Добавить Middleware, который:
Получает IUserService из DI.
Вызывает GetUserById(1).
Записывает результат в HttpContext.Response.*/

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ILoggerService, ConsoleLoggerService>();
builder.Services.AddTransient<IUserService, UserService>();

var app = builder.Build();

app.Run(async context =>
{
    context.Response.ContentType = "text/plain; charset=utf-8";

    var userService = context.RequestServices.GetRequiredService<IUserService>();
    var user = userService.GetUserById(1);

    await context.Response.WriteAsync($"Результат из UserService: {user}");
});


namespace WebApplication1.Services
{
    public interface ILoggerService
    {
        void Log(string message);
    }

    public class ConsoleLoggerService : ILoggerService
    {
        public void Log(string message) => Console.WriteLine($"[LOG] {message}");
    }

    public interface IUserService
    {
        string GetUserById(int id);
    }

    public class UserService : IUserService
    {
        private readonly ILoggerService _logger;
        public UserService(ILoggerService logger) => _logger = logger;

        public string GetUserById(int id)
        {
            _logger.Log($"Вызван метод GetUserById с id={id}");
            return $"User{{id={id}}}";
        }
    }
}
