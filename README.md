# MyRent API Demo (ASP.NET Core MVC)

This project is a simple ASP.NET Core MVC application that consumes the **MyRent API** and displays a list of properties with detailed views for each property.

---

## Tech Stack

- **.NET 8 (LTS)**
- **ASP.NET Core MVC**
- **HttpClient + Options pattern**
- **Tabler UI**
- **Razor Views**
---

## Prerequisites

- .NET 8 SDK
- Visual Studio / JetBrains Rider / VS Code
- Valid **MyRent API, Guid & Token**

---

## Configuration

If you know what it is you know what it is.
### appsettings.Development.json
```json
{
  "MyRentApi": {
    "BaseUrl": "YOU_API_URL_HERE",
    "Guid": "YOUR_GUID_HERE",
    "Token": "YOUR_TOKEN_HERE"
  }
}
