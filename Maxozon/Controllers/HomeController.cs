using Maxozon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MaxozonContext.Implements;
using MaxozonContext.Models;
using MaxozonContext.StorageInterfaces;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
namespace Maxozon.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPatientStorage _patientStorage;
        private readonly IDoctorStorage _doctorStorage;
        private readonly IAppointmentStorage _appointmentStorage;
        private static Patient pre_registration_user;
        private static int code_ver;
        public static Patient auth_user = null;
        public HomeController(ILogger<HomeController> logger, IDoctorStorage doctorStorage, IAppointmentStorage appointmentStorage, IPatientStorage patientStorage)
        {
            _logger = logger;
            _doctorStorage = doctorStorage;
            _appointmentStorage = appointmentStorage;
            _patientStorage = patientStorage;
        }

        public IActionResult Index()
        {
            ViewBag.logged = HttpContext.Session.GetInt32("logged");
            return View(_doctorStorage.GetDoctors());
        }
        public IActionResult Authorisation()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Authorisation(string Email, string Password)
        {
            if (Email != null && Password != null)
            {
                Patient pat = new Patient();
                pat.Email = Email;
                Patient test_user = _patientStorage.GetByEmail(Email);
                if (test_user == null)
                {
                    ViewBag.Message = "User does not exist!";
                    return View();
                }
                if(test_user.Password != Password)
                {
                    ViewBag.Message = "Wrong password!";
                    return View();
                }
                auth_user = test_user;
                HttpContext.Session.SetInt32("logged", 1);
                HttpContext.Session.SetInt32("UserId", test_user.Id);  
                return RedirectToAction(nameof(HomePage));
            }

             else
            {
                ViewBag.Message = "Input password/login";
                return View();
                //throw new Exception("Введите логин/пароль");
            }
        }
        public IActionResult HomePage()
        {
            int? id = HttpContext.Session.GetInt32("UserId");
            if(id == null)
            {
                return View();
            }
            else
            {
                return View(_patientStorage.GetPatient((int)id));
            }
        }
        public IActionResult EditUser()
        {
            var new_user = auth_user;
            new_user.Password = "";
            return View(new_user);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(string Password, string Password_repeat)
        {
            Patient pat = new Patient();
            if (Password != Password_repeat)
            {
                ViewBag.Message = "Passwords does not match";
                return RedirectToAction(nameof(HomePage));
            }
            if (auth_user != null)
            {
                pat = _patientStorage.GetPatient(auth_user.Id);
                pat.Password = Password;
                _patientStorage.Update(pat);
                auth_user = _patientStorage.GetPatient(pat.Id);
                return RedirectToAction(nameof(HomePage));
            }
            else
            {

                ViewBag.Message = "User not authenticated";
                return View();
            }
            
        }
        public IActionResult Registration()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Registration(Patient patient, string pass_repeat)
        {
            Random rnd = new Random();
            if (patient.Password != pass_repeat)
            {
                return View(patient);
            }
            if(_patientStorage.GetByNickname(patient.Nickname) != null)
            {
                return View();
            }
            pre_registration_user = patient;
            code_ver = rnd.Next(100000);
            var mail = MaxozonContext.SendMessage.CreateMail(pre_registration_user.Email, code_ver);
            MaxozonContext.SendMessage.SendMail(mail);
            return RedirectToAction(nameof(DoubleAuth));

        }
        [HttpPost]
        public async Task<IActionResult> DoubleAuth(int code)
        {
            if (code == code_ver)
            {
                return RedirectToAction(nameof(Success));
            }
            else
            {
                ViewBag.Message = "Wrong code";
                return DoubleAuth();
                //throw new Exception("Неверный код");
            }
        }
        [HttpGet]
        public IActionResult Success()
        {
            if(_patientStorage.GetByEmail(pre_registration_user.Email) == null)
            {
                HttpContext.Session.SetInt32("logged", 1);
                pre_registration_user.EmailConfirmed = true;
                pre_registration_user.Created = DateTime.Now;
                _patientStorage.InsertPatient(pre_registration_user);
                auth_user = pre_registration_user;
                HttpContext.Session.SetInt32("UserId", auth_user.Id);
            }
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.SetInt32("logged", 0);
            HttpContext.Session.SetInt32("UserId", 0);
            auth_user = null;
            pre_registration_user = null;
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult DoubleAuth()
        {
            return View();
        }
        public IActionResult Appointments(int Docid)
        {
            DateTime dt = DateTime.Now;
            ViewBag.Logged = HttpContext.Session.GetInt32("UserId");
            HttpContext.Session.SetInt32("DoctorId", Docid);
            return View(_appointmentStorage.GetAllAvailableAppointmentsByDoctor(Docid, dt));
        }
        public IActionResult UserAppointments()
        {
            int? pat_id = HttpContext.Session.GetInt32("UserId");
            List<Appointment> apps = new List<Appointment>();
            if (pat_id != null)
            {
                apps = _appointmentStorage.GetAllAppointmentsByPatient((int)pat_id);

                // Загрузка информации о врачах вручную
                foreach (var app in apps)
                {
                    if (app.DoctorId.HasValue)
                    {
                        app.Doctor = _doctorStorage.GetDoctor(app.DoctorId.Value);
                    }
                }
            }
            return View(apps);
        }
        public IActionResult CreateAppointment()
        {
            if (HttpContext.Session.GetInt32("DoctorId") == null)
            {
                // Если DoctorId не найден в сессии, можно вернуть ошибку или перенаправить на другую страницу
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Logged = HttpContext.Session.GetInt32("UserId");
            int docId = HttpContext.Session.GetInt32("DoctorId") ?? 0;
            var appointment = new Appointment { DoctorId = docId };
            return View(appointment);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAppointment(Appointment appointment)
        {
            int docId = HttpContext.Session.GetInt32("DoctorId") ?? 0;
            var app = _appointmentStorage.GetLastAppointmentByDoctor(docId);

            DateTime startOfReception;
            if (app == null)
            {
                // Если записей нет, начинаем через месяц с 8:30
                startOfReception = DateTime.Now.AddDays(1).Date.AddHours(8).AddMinutes(30);
            }
            else
            {
                DateTime lastAppointmentEndTime;

                // Преобразуем время окончания последней записи в DateTime, используя дату из DateOfReception
                DateTime appointmentDate = app.DateOfReception ?? DateTime.Now;  // Если нет DateOfReception, берем текущую дату

                // Преобразуем время окончания в TimeSpan
                TimeSpan endTime = TimeSpan.Parse(app.EndOfReception);

                // Составляем полное время окончания последней записи
                lastAppointmentEndTime = appointmentDate.Date.Add(endTime);

                // Увеличиваем время на 1 час
                startOfReception = lastAppointmentEndTime.AddMinutes(15);
            }

            // Проверка, если время начала приема больше 17:00, то переносим на следующий день
            if (startOfReception.Hour >= 17)
            {
                startOfReception = startOfReception.AddDays(1).Date.AddHours(8).AddMinutes(30);
            }

            // Устанавливаем время окончания через 1 час
            DateTime endOfReception = startOfReception.AddHours(1);

            // Если время окончания приема больше 17:00, начинаем следующий день с 8:30
            if (endOfReception.Hour >= 17)
            {
                startOfReception = startOfReception.AddDays(1).Date.AddHours(8).AddMinutes(30);
                endOfReception = startOfReception.AddHours(1);
            }

            // Присваиваем значения модели
            appointment.StartOfReception = startOfReception.ToString("HH:mm");
            appointment.EndOfReception = endOfReception.ToString("HH:mm");
            appointment.DateOfReception = startOfReception; // Прием на следующий день
            int? id = 0;
            if(auth_user == null)
            {
                id = null;
            }
            else
            {
                id = auth_user.Id;
                appointment.Email = auth_user.Email;
            }
            appointment.PatientId = id;
            _appointmentStorage.InsertAppointment(appointment);

            return RedirectToAction(nameof(Appointments), new { Docid = docId });

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
