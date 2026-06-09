using ScheduleServiceContracts.BindingModels;
using ScheduleServiceContracts.BusinessLogicContracts;
using ScheduleServiceRestApi.Services;

namespace ScheduleServiceRestApi.HostedServices
{
    public class WeeklyScheduleSyncHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<WeeklyScheduleSyncHostedService> _logger;

        public WeeklyScheduleSyncHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<WeeklyScheduleSyncHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var enabled = _configuration.GetValue<bool>("WeeklySync:Enabled", true);

            if (!enabled)
            {
                _logger.LogInformation("Еженедельная синхронизация ScheduleService отключена в настройках.");
                return;
            }

            var runOnStart = _configuration.GetValue<bool>("WeeklySync:RunOnStart", false);

            if (runOnStart)
            {
                await RunSynchronizationAsync(stoppingToken);
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var nextRun = GetNextRunDateTime();
                var delay = nextRun - DateTime.Now;

                if (delay < TimeSpan.Zero)
                {
                    delay = TimeSpan.Zero;
                }

                _logger.LogInformation(
                    "Следующая автоматическая синхронизация ScheduleService запланирована на {NextRun}",
                    nextRun);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    return;
                }

                if (!stoppingToken.IsCancellationRequested)
                {
                    await RunSynchronizationAsync(stoppingToken);
                }
            }
        }

        private async Task RunSynchronizationAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation("Начата автоматическая синхронизация ScheduleService.");

                using var scope = _scopeFactory.CreateScope();

                var coreImportLogic =
                    scope.ServiceProvider.GetRequiredService<ICoreImportLogic>();

                var externalScheduleImportLogic =
                    scope.ServiceProvider.GetRequiredService<IExternalScheduleImportLogic>();

                var molServiceApiClient =
                    scope.ServiceProvider.GetRequiredService<MolServiceApiClient>();

                await coreImportLogic.ImportAllAsync();

                stoppingToken.ThrowIfCancellationRequested();

                var classrooms = await molServiceApiClient.GetClassroomsAsync(stoppingToken);

                var classroomNumbers = classrooms
                    .Where(x => x.CoreSystemId > 0)
                    .Where(x => !x.NotUseInSchedule)
                    .Where(x => !string.IsNullOrWhiteSpace(x.Number))
                    .Select(x => x.Number.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(x => x)
                    .ToList();

                if (!classroomNumbers.Any())
                {
                    _logger.LogWarning(
                        "Автоматическая синхронизация расписания не выполнена: " +
                        "MolService не вернул аудитории, подходящие для расписания.");

                    return;
                }

                var forceImport = _configuration.GetValue<bool>(
                    "WeeklySync:ForceScheduleImport",
                    false);

                var result = await externalScheduleImportLogic.ImportAsync(
                    new ExternalScheduleImportBindingModel
                    {
                        ClassroomNumbers = classroomNumbers,
                        BaseDate = DateTime.Today,
                        ForceImport = forceImport
                    });

                _logger.LogInformation(
                    "Автоматическая синхронизация ScheduleService завершена. " +
                    "Передано аудиторий: {ClassroomCount}. " +
                    "Обработано групп: {ProcessedGroupsCount} из {TotalGroupsCount}. " +
                    "Получено занятий: {ReceivedLessonsCount}. " +
                    "Найдено занятий в аудиториях: {FilteredByClassroomCount}. " +
                    "Создано: {CreatedCount}. Пропущено: {SkippedCount}. Ошибок: {ErrorCount}.",
                    classroomNumbers.Count,
                    result.ProcessedGroupsCount,
                    result.TotalGroupsCount,
                    result.ReceivedLessonsCount,
                    result.FilteredByClassroomCount,
                    result.CreatedCount,
                    result.SkippedCount,
                    result.ErrorCount);

                if (result.Errors.Any())
                {
                    foreach (var error in result.Errors.Take(10))
                    {
                        _logger.LogWarning("Ошибка автоматического импорта расписания: {Error}", error);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Автоматическая синхронизация ScheduleService остановлена.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка автоматической синхронизации ScheduleService.");
            }
        }

        private DateTime GetNextRunDateTime()
        {
            var dayOfWeekName = _configuration["WeeklySync:DayOfWeek"] ?? "Monday";

            if (!Enum.TryParse<DayOfWeek>(dayOfWeekName, true, out var targetDayOfWeek))
            {
                targetDayOfWeek = DayOfWeek.Monday;
            }

            var hour = _configuration.GetValue<int>("WeeklySync:Hour", 3);
            var minute = _configuration.GetValue<int>("WeeklySync:Minute", 30);

            hour = Math.Clamp(hour, 0, 23);
            minute = Math.Clamp(minute, 0, 59);

            var now = DateTime.Now;

            var daysUntilTarget =
                ((int)targetDayOfWeek - (int)now.DayOfWeek + 7) % 7;

            var nextRun = now.Date
                .AddDays(daysUntilTarget)
                .AddHours(hour)
                .AddMinutes(minute);

            if (nextRun <= now)
            {
                nextRun = nextRun.AddDays(7);
            }

            return nextRun;
        }
    }
}