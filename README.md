# Public-Holidays-Exporter
ASP.NET Core MVC application that retrieves public holidays from the OpenHolidays API and exports them as CSV files.

## Technologies Used
- ASP.NET Core MVC
- C#
- Bootstrap
- OpenHolidays API - https://www.openholidaysapi.org/en/

## Features
- Search public holidays by country and year
- Optional custom date range
- Language Selection
- Displays results in a table
- Exports results to CSV
- Recent searches using session storage

## How to Run
1. Clone the GitHub repository from: https://github.com/JKF427003/Public-Holidays-Exporter.git
2. Open the solution in Visual Studio.
3. Restore NuGet packages if needed.
4. Run the project. (HTTPS preferred)
5. Open the web application in your browser.

- If needed or preferred, please ask the developer to quickly host the application, and they will send you the link to website. 

## How to Use
1. Select a country.
2. Select if you want to have a whole year or custom range. Then select the year or the range.
3. Select a language.
4. Click search.
5. Review the public holidays in the results table.
6. Click the CSV download button to export the results.
7. If needed, the website saves the recent searches, where you can find past searches and also can download the previous results.

## CSV Export
The exported CSV includes:
- Country code
- Language
- Date
- Holiday name

Example filename: public-holidays-MT-2026.csv

## Error Handling
The application handles:
- Invalid or missing input
- API connection issues
- Empty holiday results
- CSV generation errors

## Future Improvements
- Accept alternative or native country names (e.g. Suomi → Finland, Deutschland → Germany).
- Interactive map-based country selection for easier navigation.
- Automatic location detection (with user permission) to suggest the user's country.
- Allow users to add custom holidays that are not available through the OpenHolidays API.
- Implement local caching so previously retrieved data is available if the API becomes unavailable.
- Add support for additional interface languages.
- Add more unit tests for services and CSV generation.