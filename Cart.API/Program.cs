// Registrar Repository e Initializer
builder.Services.AddSingleton<Cart.API.Data.DatabaseInitializer>();
builder.Services.AddScoped<Cart.API.Data.CartRepository>();

var app = builder.Build();

// Inicializar DB
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<Cart.API.Data.DatabaseInitializer>();
    initializer.Initialize();
}

// Mapear los endpoints
Cart.API.Endpoints.CartEndpoints.MapCartEndpoints(app);
