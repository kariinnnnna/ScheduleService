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
    public class LessonTimeBindingModel : ILessonTimeModel
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Номер пары должен быть больше 0")]
        [Display(Name = "Номер пары")]
        public int PairNumber { get; set; }

        [Required(ErrorMessage = "Время начала обязательно")]
        [Display(Name = "Время начала")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Время окончания обязательно")]
        [Display(Name = "Время окончания")]
        public TimeSpan EndTime { get; set; }

        [StringLength(200, ErrorMessage = "Описание не должно превышать 200 символов")]
        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
