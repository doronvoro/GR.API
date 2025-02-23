var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins("http://localhost:5173")  // Frontend React URL
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowLocalhost");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


List<TaskItem> tasks = new List<TaskItem>();
Random random = new Random();
string[] statuses = { "pending", "in progress", "completed", "overdue" };

// Get all tasks
app.MapGet("/tasks", () => Results.Ok(tasks));

// Create a new task
app.MapPost("/tasks", () => {
int newId = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
var newTask = new TaskItem
{
Id = newId,
Name = $"task{newId}",
Status = statuses[random.Next(statuses.Length)],
CreatedDate = DateTime.UtcNow,
UpdatedDate = null
};
tasks.Add(newTask);
return Results.Ok(newTask);
});

// Update task status
app.MapPut("/tasks/{id}", (int id, string status) => {
var task = tasks.FirstOrDefault(t => t.Id == id);
if (task == null) return Results.NotFound();

if (!statuses.Contains(status)) return Results.BadRequest("Invalid status");

task.Status = status;
task.UpdatedDate = DateTime.UtcNow;
return Results.Ok(task);
});

app.Run();

record TaskItem
{
    public int Id { get; init; }
    public string Name { get; set; }
    public string Status { get; set; }
    public DateTime CreatedDate { get; init; }
    public DateTime? UpdatedDate { get; set; }
}
