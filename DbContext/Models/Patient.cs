﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed {  get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public DateTime? Created { get; set; }
        public string Sex { get; set; }

    }
}
