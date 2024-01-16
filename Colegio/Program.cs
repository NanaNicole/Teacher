using Colegio;
using Colegio.Services;
using Colegio.Services.Interfaces;
using Student.Producer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlServer<ColegioContext>(builder.Configuration.GetConnectionString("colegio"));
builder.Services.AddScoped<ITeacher, Teacher>();
builder.Services.AddScoped<IColegioContext, ColegioContext>();
builder.Services.AddScoped<IProducer, Producer>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseWelcomePage();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
