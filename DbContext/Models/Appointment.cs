using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbContext.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public int? DoctorId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public DateTime? BirthDate { get; set;}
        public string LivingPlace { get; set; }
        public string Email {  get; set; }
        public string Sympthomes { get; set; }
        public virtual Doctor? Doctor { get; set; }
    }
}
