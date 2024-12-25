using MaxozonContext.Models;
using MaxozonContext.StorageInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.Implements
{
    public class DoctorStorage : IDoctorStorage
    {
        public List<Doctor> GetDoctors()
        {
            using var db = new MaxozonDatabase();
            return db.Doctors.ToList();
        }
        public Doctor GetDoctor(int id)
        {
            using var db = new MaxozonDatabase();
            return db.Doctors.SingleOrDefault(d => d.Id == id);
        }

    }
}
