using MaxozonContext.Models;
using MaxozonContext.StorageInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxozonContext.Implements
{
    public class PatientStorage : IPatientStorage
    {
        public void InsertPatient(Patient patient)
        {
            using var db = new MaxozonDatabase();
            db.Patients.Add(patient);
            db.SaveChanges();
        }
        public Patient GetPatient(int id)
        {
            using var db = new MaxozonDatabase();
            return db.Patients.FirstOrDefault(p => p.Id == id);
        }
        public Patient GetByNickname(string nickname)
        {
            using var db = new MaxozonDatabase();
            return db.Patients.FirstOrDefault(p => p.Nickname == nickname);
        }
        public Patient GetByEmail(string email)
        {
            using var db = new MaxozonDatabase();
            return db.Patients.FirstOrDefault(p => p.Email == email);
        }
        public void Update(Patient patient)
        {
            using var db = new MaxozonDatabase();
            var element = db.Patients.FirstOrDefault(rec => rec.Id == patient.Id);
            if (element == null)
                throw new Exception("Не найдено");
            CreateModel(patient, element);
            db.SaveChanges();
        }
        private Patient CreateModel(Patient model, Patient patient)
        {
            patient.Name = model.Name;
            patient.Email = model.Email;
            patient.EmailConfirmed = model.EmailConfirmed;
            patient.Password = model.Password;
            return patient;

        }
    }
}
