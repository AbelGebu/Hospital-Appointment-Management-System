# Hospital Appointment Management System

A simple web application built with C# and ASP.NET Core MVC that allows patients to book appointments with doctors online.

---

## What Does This Project Do?

This system helps run a hospital's daily schedule by giving three types of users special access:

* **Patients:** Can log in, search for doctors, book open time slots, and view or cancel their appointments.
* **Doctors:** Can log in to see their daily schedule and view patient details for booked appointments.
* **Admins:** Can add and manage doctors and patients in the system.

---

## How It Works (The 4 Main Models)

1. **Doctor:** Stores the doctor's name and specialty.
2. **Patient:** Stores the patient's name and contact info.
3. **TimeSlot:** Shows available times a doctor is free .
4. **Appointment:** Connects a patient to a time slot once booked.

---

## Technologies Used

* **C# & ASP.NET Core MVC** (Web Framework)
* **ASP.NET Core Identity** (User Login & Roles)
* **Entity Framework Core & SQL Server** (Database)

---
