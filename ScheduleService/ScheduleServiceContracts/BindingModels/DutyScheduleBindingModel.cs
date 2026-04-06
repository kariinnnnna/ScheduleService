using ScheduleServiceDataModels.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ScheduleServiceContracts.BindingModels
{
    public class DutyScheduleBindingModel : IDutyScheduleModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Дата обязательна")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Пара обязательна")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректное время пары")]
        [Display(Name = "ID времени пары")]
        public int? LessonTimeId { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public string? Place { get; set; }
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Дежурный обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный дежурный")]
        [Display(Name = "Дежурный")]
        public int DutyPersonId { get; set; }
    }
}
