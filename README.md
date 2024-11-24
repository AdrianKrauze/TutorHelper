

# TutorHelper API Backend

The **TutorHelper API** is a backend application designed to simplify tutoring management – it handles user registration, student and lesson management, and Google Calendar synchronization. This API provides a structured solution for tutors to organize their workflow and efficiently manage lessons and student data.

## Table of Contents

- [Project Overview](#project-overview)
- [API Endpoints](#api-endpoints)
  - [Account Management](#account-management)
  - [Student Management](#student-management)
  - [Lesson Management](#lesson-management)
  - [Google Calendar Integration](#google-calendar-integration)
- [Sample Requests](#sample-requests)

## Project Overview

The **TutorHelper API** backend provides a flexible and powerful solution for tutors, offering features such as:

- **User registration and management** – Secure login, password reset, and email verification.
- **Student management** – Create, edit, and view student profiles, including individual conditions and subjects.
- **Lesson scheduling** – Supports lesson creation, updates, and reminders, with Google Calendar integration.

## API Endpoints

### Account Management

- **`POST /api/Account/register`** – Register a new user.
- **`POST /api/Account/login`** – User login.
- **`POST /api/Account/logout`** – Log out the current user.
- **`POST /api/Account/change-password`** – Change the current user password.
- **`POST /api/Account/forgot-password`** – Request password reset instructions.
- **`POST /api/Account/reset-password`** – Reset user password.
- **`GET /api/Account/confirm-email`** – Confirm user email with a unique 
- 
- 
- .

### Student Management

- **`POST /api/students/create`** – Add a new student record.
- **`GET /api/students/list/active`** – Retrieve a list of active students.
- **`PATCH /api/students/edit/{id}`** – Edit student information.
- **`DELETE /api/students/{studentId}/delete`** – Delete a student record.

### Lesson Management

- **`POST /api/Lesson/create/with-student`** – Create a lesson linked to an existing student. (optionally add lesson to synced google calendar)
- **`POST /api/Lesson/create/without-student`** – Create a lesson without linking it to a student.
- **`PATCH /api/Lesson/{lessonId}/update/with-student`** – Update a lesson associated with a student. (if lesson is synced with google calendar, update event obj in google calendar)
- **`DELETE /api/Lesson/{lessonId}`** – Delete a lesson.

### Google Calendar Integration

- **`GET /api/Calendar/get-google-data`** – Retrieve Google Calendar data and cache data. 
- **`GET /api/Calendar/refresh-calendar-data`** – Force resfresh when cache data exist in calendar data.

## Sample Requests

### User Registration

**Endpoint**: `POST /api/Account/register`

**Body**:
```json
{
    "email": "user@example.com",
    "password": "password123",
    "confirmPassword": "password123",
    "firstName": "John",
    "lastName": "Doe"
}
```

**Description**: Creates a new user account.

---

### Creating a Lesson with a Student

**Endpoint**: `POST /api/Lesson/create/with-student`

**Body**:
```json
{
    "studentId": "12345",
    "duration": 60,
    "price": 100.0,
    "date": "2024-10-30T14:00:00Z",
    "pushBoolean": true,
    "pushTimeBeforeLesson": 15,
    "repeat": false
}
```

**Description**: Creates a lesson associated with an existing student, with an option to set a reminder.

---
