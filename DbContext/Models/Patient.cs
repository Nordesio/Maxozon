using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbContext.Models
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Nickname { get; set; }
        public string Email { get; set; }
        public string EmailConfirmed {  get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Sex { get; set; }

    }
}
