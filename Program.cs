using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

var todoItems = app.MapGroup("/todoitems");

app.MapGet("/", () => "TodoApi");

todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetCompleteTodos);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);

static async Task<IResult> GetAllTodos(TodoDb db) => 
	TypedResults.Ok(await db.Todos.ToArrayAsync());

static async Task<IResult> GetCompleteTodos(TodoDb db) => 
	TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());

static async Task<IResult> GetTodo(int id, TodoDb db) => 
	await db.Todos.FindAsync(id)
		is Todo todo
			? TypedResults.Ok(todo)
			: TypedResults.NotFound();

static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
{
	db.Todos.Add(todo);
	await db.SaveChangesAsync();

	return TypedResults.Created($"/todoitems/{todo.Id}", todo);
}

static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
{
	var todo = await db.Todos.FindAsync(id);

	if (todo is null) return TypedResults.NotFound();

	todo.Name = inputTodo.Name;
	todo.IsComplete = inputTodo.IsComplete;

	await db.SaveChangesAsync();

	return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
	if (await db.Todos.FindAsync(id) is Todo todo)
	{
		db.Todos.Remove(todo);
		await db.SaveChangesAsync();
		return TypedResults.NoContent();
	}

	return TypedResults.NotFound();
}

app.Run();