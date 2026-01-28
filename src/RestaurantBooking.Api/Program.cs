using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore; // Necessário para Migrate()
using RestaurantBooking.Application.Commands;
using RestaurantBooking.Application.Queries;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure;
using RestaurantBooking.Infrastructure.Persistence; // Necessário para acessar o DbContext
using RestaurantBooking.Infrastructure.Services;
using RestaurantBooking.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// --- CONFIGURAÇÃO DO CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// --- CACHE (Redis) ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Booking_";
});
builder.Services.AddMemoryCache();

// --- RABBITMQ ---
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

// --- HANGFIRE ---
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));
builder.Services.AddHangfireServer();

// --- HANDLERS ---
builder.Services.AddScoped<CreateReservationHandler>();
builder.Services.AddScoped<GetAllReservationsHandler>();
builder.Services.AddScoped<ConfirmReservationHandler>();
builder.Services.AddScoped<CheckInReservationHandler>();
builder.Services.AddScoped<GetDashboardMetricsHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- APLICAÇÃO AUTOMÁTICA DAS TABELAS (MIGRATION) ---
// Isso aqui resolve o erro "relation does not exist"
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        var context = services.GetRequiredService<RestaurantBookingDbContext>();
        // Se o banco não existir, cria. Se existir, atualiza as tabelas.
        context.Database.Migrate();
        Console.WriteLine("✅ Banco de dados migrado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Erro ao migrar banco: {ex.Message}");
    }

    // Configura o Job do Hangfire
    var recurringJobManager = services.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<NoShowJob>("process-no-shows", job => job.ProcessNoShows(), Cron.Hourly);
}

// --- PIPELINE ---
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.UseHangfireDashboard();

app.MapControllers();
app.Run();
