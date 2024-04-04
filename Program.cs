using ReadRequestBodyASPNETCoreWebAPI;
using System.Net.NetworkInformation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
    app.UseMiddleware<RequestBodyMiddleware>();
/**************Using Action Filters to Read the Request Body***********************/
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ReadRequestBodyActionFilter>();
});
/**************Using Action Filters to Read the Request Body***********************/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
