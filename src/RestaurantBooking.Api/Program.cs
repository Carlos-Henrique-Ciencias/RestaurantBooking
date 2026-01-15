using Hangfire;
using Hangfire.PostgreSql;
using RestaurantBooking.Application.Commands;
using RestaurantBooking.Application.Queries;
using RestaurantBooking.Application.Interfaces;
using RestaurantBooking.Infrastructure;
using RestaurantBooking.Infrastructure.Services;
using RestaurantBooking.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

// --- CACHE (Redis) ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "Booking_";
});
builder.Services.AddMemoryCache();

// --- RABBITMQ ---
builder.Services.AddScoped<IRabbitMQService, RabbitMQService>();

// --- HANGFIRE (Banco Postgres) ---
builder.Services.AddHangfire(config => config
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

builder.Services.AddHangfireServer(); // O Servidor que roda os jobs

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

// --- CONFIGURAÇÃO DO PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

// Painel do Hangfire (Dashboard visual)
app.UseHangfireDashboard();

// Agendamento do Job Recorrente (Roda a cada hora)
using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    // Expressão Cron.Hourly roda de hora em hora. Para teste, pode mudar.
    recurringJobManager.AddOrUpdate<NoShowJob>("process-no-shows", job => job.ProcessNoShows(), Cron.Hourly);
}

app.MapControllers();

app.Run();
