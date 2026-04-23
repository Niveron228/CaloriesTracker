# ⚡ Calories & Workout Tracker 🍎🏋️‍♂️

A modern Fullstack application for tracking nutrition and workouts. Built with **.NET 8** and **React**, featuring a sleek **Glassmorphism** design and smooth animations.

This project demonstrates the use of relational databases, integration with third-party APIs (FatSecret), and secure authentication using JWTs via HTTP-only Cookies.

## ✨ Features
* **🔐 Full Authentication:** Secure Registration and Login with password hashing (BCrypt).
* **🍎 Calorie Tracker:** Search products, add meals, and automatic Macro (P/F/C) calculation.
* **💪 Workout Module:** Add exercises, sets, and weights with date-based history.
* **🎨 Modern UI:** Animated backgrounds, Glassmorphism effects, and custom Toast notifications.
* **🐳 Dockerized:** Entire stack (Frontend + Backend + DB) runs with a single command.

## 🚀 Tech Stack
* **Backend:** C#, ASP.NET Core (Web API)
* **Database:** SQLite
* **ORM:** Entity Framework Core
* **Authentication:** JWT (JSON Web Tokens) + HTTP-only Cookies
* **External Integrations:** [FatSecret API](https://platform.fatsecret.com/api/)

## 📸 Screenshots

### 🔐 Login & Registration
<div align="center">
  <img src="https://github.com/user-attachments/assets/357cf0a5-e296-4c76-8d12-b9d09a6bff23" width="450" alt="Login Screen" />
  <img src="https://github.com/user-attachments/assets/e27127a5-5b99-409a-8332-caaabefddc0e" width="415" alt="Registration Screen" />
</div>

### 🏠 Home Page
<div align="center">
  <img width="1848" height="915" alt="image" src="https://github.com/user-attachments/assets/7674c399-5dad-4ade-85df-b5db3b76d9ac" />
</div>

### 🍎 Dashboard & Tracking
<div align="center">
  <p><b>Calories Tracker</b></p>
  <img width="1850" height="919" alt="image" src="https://github.com/user-attachments/assets/02529b86-4911-46c7-a55f-4fe614a9ab22" />
  
  <p><b>Workout Tracking</b></p>
<img width="1849" height="914" alt="image" src="https://github.com/user-attachments/assets/ea908f06-f6f9-495c-ac54-72c15b0db60f" />

</div>

## 🛠️ How to Run Locally

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Niveron228/CaloriesTracker.git

2. **Configure "appsettings.json":**
```json
{
  "Jwt": {
    "Key": "your_super_secret_and_long_key_here",
    "Issuer": "CaloriesTrackerServer",
    "Audience": "CaloriesTrackerUsers"
  },
  "FatSecret": {
    "ClientId": "your_fatsecret_client_id",
    "ClientSecret": "your_fatsecret_client_secret"
  }
}
```
4. **Apply Migrations (Create Database)**
     `` dotnet ef database update``
5. **Run the project:**
   ``dotnet run``

   * Once running, the Swagger UI will be available at http://localhost:5230/swagger.

## 🗺️ Roadmap

* [ ] Implement full CRUD for the Meal Log (ability to delete and update the weight of existing records).

* [ ] Add a "Goals" system: personalized daily calorie and macro limits for each user.

* [ ] Integrate Redis for high-performance caching of frequent search queries.

* [ ] Expand the frontend client functionality.
