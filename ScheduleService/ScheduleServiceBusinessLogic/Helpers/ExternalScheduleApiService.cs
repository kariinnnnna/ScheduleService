using ScheduleServiceBusinessLogic.Models.ExternalSchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Helpers
{
    public class ExternalScheduleApiService
    {
        private readonly HttpClient _httpClient;

        public ExternalScheduleApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetGroupsAsync()
        {
            return await ReadWrappedResponseAsync<List<string>>("groups")
                   ?? new List<string>();
        }

        public async Task<List<string>> GetTeachersAsync()
        {
            return await ReadWrappedResponseAsync<List<string>>("teachers")
                   ?? new List<string>();
        }

        public async Task<int> GetCurrentWeekAsync()
        {
            return await ReadWrappedResponseAsync<int>("current-week");
        }

        public async Task<ExternalScheduleVersionModel> GetLastVersionAsync()
        {
            var result = await ReadWrappedResponseAsync<ExternalScheduleVersionModel>("last-version");

            if (result == null)
            {
                throw new Exception("API расписания не вернуло данные последней версии.");
            }

            return result;
        }

        public async Task<List<ExternalScheduleLessonModel>> GetTimetableAsync(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                throw new ArgumentException("Не указан фильтр для получения расписания.");
            }

            var url = $"timetable?filter={Uri.EscapeDataString(filter.Trim())}";
            var timetable = await ReadWrappedResponseAsync<ExternalTimetableResponseBody>(url);

            if (timetable == null)
            {
                return new List<ExternalScheduleLessonModel>();
            }

            return MapTimetable(timetable);
        }

        private async Task<T?> ReadWrappedResponseAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Ошибка запроса к API расписания: {(int)response.StatusCode}. {content}");
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var wrapped = JsonSerializer.Deserialize<ExternalApiResponse<T>>(content, options);

            if (wrapped == null)
            {
                throw new Exception("Не удалось разобрать ответ API расписания.");
            }

            if (!string.IsNullOrWhiteSpace(wrapped.Error))
            {
                throw new Exception($"API расписания вернуло ошибку: {wrapped.Error}");
            }

            return wrapped.Response;
        }

        private static List<ExternalScheduleLessonModel> MapTimetable(ExternalTimetableResponseBody timetable)
        {
            var result = new List<ExternalScheduleLessonModel>();

            foreach (var weekPair in timetable.Weeks)
            {
                if (!int.TryParse(weekPair.Key, out var studyWeek))
                {
                    continue;
                }

                foreach (var day in weekPair.Value.Days)
                {
                    for (var pairIndex = 0; pairIndex < day.Lessons.Count; pairIndex++)
                    {
                        var pairLessons = day.Lessons[pairIndex];

                        foreach (var lesson in pairLessons)
                        {
                            result.Add(new ExternalScheduleLessonModel
                            {
                                StudyWeek = studyWeek,
                                Day = day.Day,
                                PairNumber = pairIndex + 1,
                                GroupName = lesson.Group?.Trim() ?? string.Empty,
                                TeacherName = lesson.Teacher?.Trim() ?? string.Empty,
                                ClassroomNumber = lesson.Room?.Trim() ?? string.Empty,
                                LessonName = lesson.NameOfLesson?.Trim() ?? string.Empty
                            });
                        }
                    }
                }
            }

            return result;
        }
    }
}
