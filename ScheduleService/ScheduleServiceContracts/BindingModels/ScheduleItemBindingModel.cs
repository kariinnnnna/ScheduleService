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
    public class ScheduleItemBindingModel : IScheduleItemModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Тип записи обязателен")]
        [Display(Name = "Тип")]
        public ScheduleItemType Type { get; set; }

        [Required(ErrorMessage = "Дата обязательна")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Дисциплина обязательна")]
        [StringLength(200, ErrorMessage = "Название дисциплины не должно превышать 200 символов")]
        [Display(Name = "Дисциплина")]
        public string Subject { get; set; } = string.Empty;

        [Display(Name = "ID времени пары")]
        public int? LessonTimeId { get; set; }

        [Display(Name = "Время начала")]
        public TimeSpan? StartTime { get; set; }

        [Display(Name = "Время окончания")]
        public TimeSpan? EndTime { get; set; }

        [Display(Name = "ID аудитории")]
        public int? ClassroomId { get; set; }

        [StringLength(50, ErrorMessage = "Номер аудитории не должен превышать 50 символов")]
        [Display(Name = "Аудитория")]
        public string? ClassroomNumber { get; set; }

        [Display(Name = "ID группы")]
        public int? GroupId { get; set; }

        [StringLength(100, ErrorMessage = "Название группы не должно превышать 100 символов")]
        [Display(Name = "Группа")]
        public string? GroupName { get; set; }

        [Display(Name = "ID преподавателя")]
        public int? TeacherId { get; set; }

        [StringLength(200, ErrorMessage = "Имя преподавателя не должно превышать 200 символов")]
        [Display(Name = "Преподаватель")]
        public string? TeacherName { get; set; }

        [StringLength(1000, ErrorMessage = "Комментарий не должен превышать 1000 символов")]
        [Display(Name = "Комментарий")]
        public string? Comment { get; set; }
        public bool IsImported { get; set; }
    }
}
