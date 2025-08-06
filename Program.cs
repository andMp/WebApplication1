using Microsoft.AspNetCore.Http;
/*Создай REST API для управления задачами (To-Do List) с возможностью создания, редактирования, удаления и получения задач.
На фронтенде реализуй интерфейс, позволяющий добавлять, редактировать и удалять задачи через API.*/

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Index}/{id?}");

app.Run();
