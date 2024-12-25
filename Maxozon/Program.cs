using Microsoft.AspNetCore.Identity;
using MaxozonContext.StorageInterfaces;
using MaxozonContext.Implements;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IPatientStorage, PatientStorage>();
builder.Services.AddTransient<IAppointmentStorage, AppointmentStorage>();
builder.Services.AddTransient<IDoctorStorage, DoctorStorage>();

builder.Services.AddDistributedMemoryCache(); // ��� �������� ������ � ������
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // ��������� ������� ����� ������
    options.Cookie.HttpOnly = true; // ������������� cookie ��� ��������� ������ ��� HTTP
    options.Cookie.IsEssential = true; // ������������� cookie ��� ������������
});

var app = builder.Build();
app.UseSession();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
