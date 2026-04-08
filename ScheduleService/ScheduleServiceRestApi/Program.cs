using Microsoft.EntityFrameworkCore;
using ScheduleServiceBusinessLogic.Helpers;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceDatabaseImplement;
using ScheduleServiceDatabaseImplement.Implements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScheduleServiceDatabase>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDutyPersonStorage, DutyPersonStorage>();
builder.Services.AddScoped<IDutyScheduleStorage, DutyScheduleStorage>();
builder.Services.AddScoped<IGroupStorage, GroupStorage>();
builder.Services.AddScoped<ILessonTimeStorage, LessonTimeStorage>();
builder.Services.AddScoped<IScheduleItemStorage, ScheduleItemStorage>();
builder.Services.AddScoped<ITeacherStorage, TeacherStorage>();

builder.Services.AddScoped<IDutyPersonLogic, DutyPersonLogic>();
builder.Services.AddScoped<IDutyScheduleLogic, DutyScheduleLogic>();
builder.Services.AddScoped<IGroupLogic, GroupLogic>();
builder.Services.AddScoped<ILessonTimeLogic, LessonTimeLogic>();
builder.Services.AddScoped<IScheduleItemLogic, ScheduleItemLogic>();
builder.Services.AddScoped<IUniversityScheduleLogic, UniversityScheduleLogic>();
builder.Services.AddScoped<ITeacherLogic, TeacherLogic>();

builder.Services.AddHttpClient<CoreApiService>(client =>
{
    var baseUrl = builder.Configuration["CoreApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("Не настроен CoreApi:BaseUrl");
    }

    client.BaseAddress = new Uri(baseUrl.EndsWith("/") ? baseUrl : baseUrl + "/");
});

builder.Services.AddTransient<ICoreImportLogic, CoreImportLogic>();


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