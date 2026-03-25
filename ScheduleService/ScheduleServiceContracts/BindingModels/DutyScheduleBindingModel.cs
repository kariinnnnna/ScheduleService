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

        [Display(Name = "ID времени пары")]
        public int? LessonTimeId { get; set; }

        [Display(Name = "Время начала")]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "Время окончания")]
        public TimeSpan? EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Место не должно превышать 200 символов")]
        [Display(Name = "Место")]
        public string? Place { get; set; }

        [StringLength(1000, ErrorMessage = "Комментарий не должен превышать 1000 символов")]
        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Дежурный обязателен")]
        [Range(1, int.MaxValue, ErrorMessage = "Некорректный дежурный")]
        [Display(Name = "Дежурный")]
        public int DutyPersonId { get; set; }
    }
}
