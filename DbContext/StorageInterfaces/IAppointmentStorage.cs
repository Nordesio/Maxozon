using MaxozonContext.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.StorageInterfaces
{
    public interface IAppointmentStorage
    {
        void InsertAppointment(Appointment appointment);
        List<Appointment> GetAllAppointmentsByDoctor(Doctor doctor);
        List<Appointment> GetAllAvailableAppointmentsByDoctor(int id, DateTime currentDate);
        Appointment? GetLastAppointmentByDoctor(int doctorId);
        List<Appointment> GetAllAppointmentsByPatient(int id);
    }
}
