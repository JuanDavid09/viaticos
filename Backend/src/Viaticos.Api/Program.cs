using Viaticos.Application;
using Viaticos.Infrastructure;
using Viaticos.Infrastructure.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Viáticos API", Version = "v1" });
    options.AddSecurityDefinition("DevUserEmail", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "MVP: email del usuario demo. Ej: empleado@empresa.com",
        Name = "X-Dev-User-Email",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<DevUserMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
