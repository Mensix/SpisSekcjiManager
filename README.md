# SpisSekcjiManager

The tool used by me to manage [sections list](https://sekcje.github.io/). Written in C#, uses Firebase as default external database.

## Requirements

- [.NET Core 3.1](https://dotnet.microsoft.com/download)
- [ChromeDriver](https://chromedriver.chromium.org/downloads)

## Installation

1. Clone this repository
2. Move to root directory and paste chromedriver.exe file
3. Create data folder in root directory
4. Make two files in data folder: groups.json and settings.json
5. In groups.json put example content

```json
{
  "lastUpdateDate": "04/07/2020",
  "name": "sections",
  "groups": [
    {
      "category": ["Nauka"],
      "link": "277362736085803",
      "members": 92106,
      "membersGrowth": 1516,
      "name": "Ciekawostkawka"
    },
    {
      "link": "postnostalgawka",
      "members": 80412,
      "membersGrowth": 1073,
      "name": "Jak będzie w 2030?"
    },
    {
      "link": "375622829809203",
      "members": 77827,
      "membersGrowth": 1174,
      "name": "Perfekcjonizmawka"
    }
  ]
}
```

1. And in settings.json

```json
{
  "login": "<email>",
  "password": "<password>",
  "files": [{"input": "groups", "output": "groups" }],
  "settings": {
    "autoFix": true,
    "autoCompare": true,
    "autoUpdate": false,
    "shouldParseHades": false,
    "shouldUpdateArchive": false
  }
}
```

1. In login field you have to put your Facebook account e-mail, and in password Facebook password
2. Run program by using `dotnet run` command, write 1 and click Enter, the application will run Chrome, login into your Facebook account and start scraping groups
