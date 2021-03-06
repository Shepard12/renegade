﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ren.Models.ViewModels
{
    public class EditEmailModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [DataType(DataType.EmailAddress)]
        [StringLength(40, ErrorMessage = "Email Max Length is 40")]
        public string NewEmail { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [StringLength(50, ErrorMessage = "Password Max Length is 50")]
        public string Password { get; set; }

    }
}
