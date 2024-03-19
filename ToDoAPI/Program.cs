using Microsoft.EntityFrameworkCore;
using Treblle.Net.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

if (builder.Environment.IsDevelopment())
{
    var projectId = builder.Configuration["ToDoAPI:TREBLLE_PROJECT_ID"];
    var apiKey = builder.Configuration["ToDoAPI:TREBLLE_API_KEY"];
    
    builder.Services.AddTreblle(apiKey, projectId); 
}
else if (builder.Environment.IsProduction())
{
    var projectId = Environment.GetEnvironmentVariable("TREBLLE_PROJECT_ID");
    var apiKey = Environment.GetEnvironmentVariable("TREBLLE_API_KEY");

    builder.Services.AddTreblle(apiKey, projectId);
}

var app = builder.Build();


var todoItems = app.MapGroup("/todoitems");

todoItems.MapGet("/treblle", () => "Treblle is awesome")
    .UseTreblle();

todoItems.MapGet("/", async (TodoDb db) =>
    await db.Todos.ToListAsync())
    .UseTreblle();

todoItems.MapGet("/notComplete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsNotComplete).ToListAsync())
    .UseTreblle();

todoItems.MapGet("/complete", async (TodoDb db) =>
    await db.Todos.Where(t => t.IsComplete).ToListAsync())
    .UseTreblle();

todoItems.MapGet("/{id}", async (int id, TodoDb db) =>
    await db.Todos.FindAsync(id)
        is Todo todo
            ? Results.Ok(todo)
            : Results.NotFound())
    .UseTreblle();


todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();

    return Results.Created($"/todoitems/{todo.Id}", todo);

 }).UseTreblle();


todoItems.MapPut("/{id}", async (int id, Todo inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);

    if (todo is null) return Results.NotFound();

    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;

    await db.SaveChangesAsync();

    return Results.NoContent();
}).UseTreblle();


todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
}).UseTreblle();

app.UseTreblle(useExceptionHandler: true);

app.Run();