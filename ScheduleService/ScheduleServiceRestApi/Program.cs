using Microsoft.EntityFrameworkCore;
using ScheduleServiceBusinessLogic.Helpers;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceDatabaseImplement;
using ScheduleServiceDatabaseImplement.Implements;
using ScheduleServiceRestApi.HostedServices;
using ScheduleServiceRestApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScheduleServiceDatabase>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Storage
builder.Services.AddScoped<IDutyPersonStorage, DutyPersonStorage>();
builder.Services.AddScoped<IDutyScheduleStorage, DutyScheduleStorage>();
builder.Services.AddScoped<IGroupStorage, GroupStorage>();
builder.Services.AddScoped<ILessonTimeStorage, LessonTimeStorage>();
builder.Services.AddScoped<IScheduleItemStorage, ScheduleItemStorage>();
builder.Services.AddScoped<ITeacherStorage, TeacherStorage>();

// Business logic
builder.Services.AddScoped<IDutyPersonLogic, DutyPersonLogic>();
builder.Services.AddScoped<IDutyScheduleLogic, DutyScheduleLogic>();
builder.Services.AddScoped<IGroupLogic, GroupLogic>();
builder.Services.AddScoped<ILessonTimeLogic, LessonTimeLogic>();
builder.Services.AddScoped<IScheduleItemLogic, ScheduleItemLogic>();
builder.Services.AddScoped<IUniversityScheduleLogic, UniversityScheduleLogic>();
builder.Services.AddScoped<ITeacherLogic, TeacherLogic>();
builder.Services.AddScoped<ICoreImportLogic, CoreImportLogic>();

// Новая логика импорта расписания из внешнего API
builder.Services.AddScoped<IExternalScheduleImportLogic, ExternalScheduleImportLogic>();
builder.Services.AddScoped<IExternalScheduleSyncStateStorage, ExternalScheduleSyncStateStorage>();

// Core API
builder.Services.AddHttpClient<CoreApiService>(client =>
{
    var baseUrl = builder.Configuration["CoreApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("Не настроен CoreApi:BaseUrl");
    }

    client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
});

// External Schedule API
builder.Services.AddHttpClient<ExternalScheduleApiService>(client =>
{
    var baseUrl = builder.Configuration["ExternalScheduleApi:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("Не настроен ExternalScheduleApi:BaseUrl");
    }

    client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
});

builder.Services.AddHttpClient<MolServiceApiClient>(client =>
{
    var baseUrl = builder.Configuration["MolService:BaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new Exception("Не указан адрес MolService:BaseUrl в appsettings.json");
    }

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddHostedService<WeeklyScheduleSyncHostedService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();