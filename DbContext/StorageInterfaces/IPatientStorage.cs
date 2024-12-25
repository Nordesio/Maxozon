using MaxozonContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.StorageInterfaces
{
    public interface IPatientStorage
    {
        void InsertPatient(Patient patient);
        Patient GetPatient(int id);
        void Update(Patient patient);
        Patient GetByNickname(string nickname);
        Patient GetByEmail(string email);
        

    }
}
