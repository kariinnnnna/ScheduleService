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
    public class TeacherBindingModel : ITeacherModel
    {
        public int Id { get; set; }

        [Display(Name = "ID преподавателя в Core System")]
        public int CoreSystemId { get; set; }

        [Required(ErrorMessage = "Имя преподавателя обязательно")]
        [StringLength(200, ErrorMessage = "Имя преподавателя не должно превышать 200 символов")]
        [Display(Name = "Преподаватель")]
        public string TeacherName { get; set; } = string.Empty;
    }
}
