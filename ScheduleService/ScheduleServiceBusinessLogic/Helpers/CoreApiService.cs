using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleServiceBusinessLogic.Helpers
{
    public class CoreApiService
    {
        private readonly HttpClient _httpClient;

        public CoreApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CoreGroupDto>> GetGroupsAsync()
        {
            var response = await _httpClient.GetAsync("StudentGroups/GetStudentGroupList");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<CoreGroupDto>>();
            return result ?? new List<CoreGroupDto>();
        }

        public async Task<List<CoreTeacherDto>> GetTeachersAsync()
        {
            var response = await _httpClient.GetAsync("Lecturers/GetLecturerList");
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<List<CoreTeacherDto>>();
            return result ?? new List<CoreTeacherDto>();
        }
    }
}
