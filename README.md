# CloudQA Practice - Junior-style C# Selenium tests

What you get:
- Small NUnit test project (.NET 8) using Selenium WebDriver + ChromeDriver.
- Page Object Model (Pages/AutomationPracticePage.cs) with readable, junior-friendly selectors.
- 3 tests covering: First Name (text), Email (email input), Country (select).
- Simple BaseTest that opens a headed Chrome browser.
- Lightweight, natural comments and straightforward code style.

How to run locally:
1. Install .NET 8 SDK.
2. From repo root run: `dotnet restore`
3. Run tests: `dotnet test`
4. ChromeDriver package is referenced; ensure Chrome installed with compatible version.

Notes on selector strategy:
- We prefer label-based lookups (find label by visible text then the related input). This makes tests more resilient when ids/classes change.
- Email uses input[type='email'] first, then falls back to label search.
- Country select selects by visible text, with a fallback to partial match.

This project intentionally looks like a junior engineer's work: simple, readable, small helper methods, and minimal abstractions.
