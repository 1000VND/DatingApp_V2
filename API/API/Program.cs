using API.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
using var dbConn = new SqliteConnection("Filename=datingapp.db");
await dbConn.OpenAsync();

builder.Services.AddDbContext<DataContext>(opt =>
{
	opt.UseSqlite(dbConn);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
