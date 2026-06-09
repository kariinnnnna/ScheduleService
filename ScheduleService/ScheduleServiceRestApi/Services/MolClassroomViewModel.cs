namespace ScheduleServiceRestApi.Services
{
    public class MolClassroomViewModel
    {
        public int Id { get; set; }

        public int CoreSystemId { get; set; }

        public string Number { get; set; } = string.Empty;

        public bool NotUseInSchedule { get; set; }
    }
}