using System.Net.Http.Json;

namespace ScheduleServiceRestApi.Services
{
    public class MolServiceApiClient
    {
        private readonly HttpClient _httpClient;

        public MolServiceApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<MolClassroomViewModel>> GetClassroomsAsync(
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(
                "api/Classroom/GetAll",
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<List<MolClassroomViewModel>>(
                    cancellationToken: cancellationToken);

            return result ?? new List<MolClassroomViewModel>();
        }
    }
}