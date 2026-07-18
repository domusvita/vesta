using Vesta.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddVestaApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();