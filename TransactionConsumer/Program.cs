using Microsoft.EntityFrameworkCore;
using TransactionConsumer.Data;
using TransactionConsumer.Data.Settings;
using TransactionConsumer.Middleware;
using TransactionConsumer.Services.Common;
using TransactionConsumer.Services.Transactions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<TransactionSettings>(
    builder.Configuration.GetSection("TransactionSettings"));

builder.Services.AddDbContext<TransactionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDateTimeProvider, DateTimeProvider>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.UseMiddleware<ErrorHandlingMiddleware>();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
    context.Database.EnsureCreated();
}

app.Run();
