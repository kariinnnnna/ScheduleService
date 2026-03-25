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
    public class GroupBindingModel : IGroupModel
    {
        public int Id { get; set; }

        [Display(Name = "ID группы в Core System")]
        public int CoreSystemId { get; set; }

        [Required(ErrorMessage = "Название группы обязательно")]
        [StringLength(100, ErrorMessage = "Название группы не должно превышать 100 символов")]
        [Display(Name = "Группа")]
        public string GroupName { get; set; } = string.Empty;
    }
}
