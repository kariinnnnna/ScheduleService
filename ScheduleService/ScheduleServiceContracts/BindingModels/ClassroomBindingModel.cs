using ScheduleServiceDataModels.Enums;
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
    public class ClassroomBindingModel : IClassroomModel
    {
        public int Id { get; set; }

        [Display(Name = "ID аудитории в Core System")]
        public int CoreSystemId { get; set; }

        [Required(ErrorMessage = "Номер аудитории обязателен")]
        [StringLength(50, ErrorMessage = "Номер аудитории не должен превышать 50 символов")]
        [Display(Name = "Номер аудитории")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Тип аудитории обязателен")]
        [Display(Name = "Тип аудитории")]
        public ClassroomType Type { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Вместимость не может быть отрицательной")]
        [Display(Name = "Вместимость")]
        public int Capacity { get; set; }

        [Display(Name = "Не использовать в расписании")]
        public bool NotUseInSchedule { get; set; }
    }
}
