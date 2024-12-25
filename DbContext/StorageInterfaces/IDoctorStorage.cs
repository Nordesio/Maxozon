using MaxozonContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.StorageInterfaces
{
    public interface IDoctorStorage
    {
        List<Doctor> GetDoctors();
        Doctor GetDoctor(int id);
    }
}
