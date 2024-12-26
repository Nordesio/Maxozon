using MaxozonContext.Models;
using MaxozonContext.StorageInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.Implements
{
    public class AppointmentStorage : IAppointmentStorage
    {
        public void InsertAppointment(Appointment appointment)
        {
            using var db = new MaxozonDatabase();
            db.Appointments.Add(appointment);
            db.SaveChanges();
        }
        public List<Appointment> GetAllAppointmentsByDoctor(Doctor doctor)
        {
            using var db = new MaxozonDatabase();
            
            return db.Appointments.Where(ap => ap.DoctorId == doctor.Id).ToList();

        }
        public List<Appointment> GetAllAvailableAppointmentsByDoctor(int id, DateTime currentDate)
        {
            using var db = new MaxozonDatabase();

            var appointments = db.Appointments.Where(ap => ap.DoctorId == id).ToList();
            var availableAppointments = appointments
        .Where(a => a.DateOfReception > currentDate)
        .ToList();
            return availableAppointments;
        }
        public Appointment? GetLastAppointmentByDoctor(int doctorId)
        {
            using var db = new MaxozonDatabase();
            return db.Appointments
                 .Where(a => a.DoctorId == doctorId)
                 .AsEnumerable() // Переводим данные в память
                 .OrderByDescending(a => a.DateOfReception)
                 .ThenByDescending(a => DateTime.Parse(a.EndOfReception))
                 .FirstOrDefault();
        }

        public List<Appointment> GetAllAppointmentsByPatient(int id)
        {
            using var db = new MaxozonDatabase();
            return db.Appointments.Where(p => p.PatientId == id).ToList();
        }
    }
}
