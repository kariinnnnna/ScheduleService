using Microsoft.EntityFrameworkCore;
using ScheduleServiceBusinessLogic.Implements;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceContracts.StorageContracts;
using ScheduleServiceDatabaseImplement;
using ScheduleServiceDatabaseImplement.Implements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ScheduleServiceDatabase>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IClassroomStorage, ClassroomStorage>();
builder.Services.AddScoped<IDutyPersonStorage, DutyPersonStorage>();
builder.Services.AddScoped<IDutyScheduleStorage, DutyScheduleStorage>();
builder.Services.AddScoped<IGroupStorage, GroupStorage>();
builder.Services.AddScoped<ILessonTimeStorage, LessonTimeStorage>();
builder.Services.AddScoped<IScheduleItemStorage, ScheduleItemStorage>();
builder.Services.AddScoped<ITeacherStorage, TeacherStorage>();

builder.Services.AddScoped<IClassroomLogic, ClassroomLogic>();
builder.Services.AddScoped<IDutyPersonLogic, DutyPersonLogic>();
builder.Services.AddScoped<IDutyScheduleLogic, DutyScheduleLogic>();
builder.Services.AddScoped<IGroupLogic, GroupLogic>();
builder.Services.AddScoped<ILessonTimeLogic, LessonTimeLogic>();
builder.Services.AddScoped<IScheduleItemLogic, ScheduleItemLogic>();
builder.Services.AddScoped<ITeacherLogic, TeacherLogic>();

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