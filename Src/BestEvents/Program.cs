using BestEvents;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHostedService<BookingProcesser>();

builder.Services.AddSingleton<EventFilters>();
builder.Services.AddSingleton<Pagination<EventEntity>>();
builder.Services.AddSingleton<EntityMapper>();
builder.Services.AddSingleton<DtoMapper>();
builder.Services.AddSingleton<EntityMapper>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookingService, BookingService>();

builder.Services.AddSwaggerGen(options =>
{
    // Путь к XML-файлу с документацией
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseErrorHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.MapControllers();

app.Run();
