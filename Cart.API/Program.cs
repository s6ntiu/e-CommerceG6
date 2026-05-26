// Registrar Repository e Initializer
builder.Services.AddSingleton<Cart.API.Data.DatabaseInitializer>();
builder.Services.AddScoped<Cart.API.Data.CartRepository>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar DB
using (var scope = app.Services.CreateScope())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    var initializer = scope.ServiceProvider.GetRequiredService<Cart.API.Data.DatabaseInitializer>();
    initializer.Initialize();
}

// Mapear los endpoints
Cart.API.Endpoints.CartEndpoints.MapCartEndpoints(app);