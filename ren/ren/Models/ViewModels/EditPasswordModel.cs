using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ren.Models.ViewModels
{
    public class EditPasswordModel
    {
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string NewPassword1 { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword1", ErrorMessage = "Passwords don't match")]
        public string NewPassword2 { get; set; }

    }
}
