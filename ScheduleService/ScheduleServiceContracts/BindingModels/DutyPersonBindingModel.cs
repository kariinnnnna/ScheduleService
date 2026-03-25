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
    public class DutyPersonBindingModel : IDutyPersonModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [StringLength(100, ErrorMessage = "Фамилия не должна превышать 100 символов")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имя обязательно")]
        [StringLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Должность не должна превышать 150 символов")]
        [Display(Name = "Должность")]
        public string? Position { get; set; }

        [Phone(ErrorMessage = "Некорректный номер телефона")]
        [StringLength(30, ErrorMessage = "Телефон не должен превышать 30 символов")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный email")]
        [StringLength(150, ErrorMessage = "Email не должен превышать 150 символов")]
        [Display(Name = "Email")]
        public string? Email { get; set; }
    }
}
