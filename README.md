# SpisSekcjiManager

The tool used by me to manage [sections list](https://sekcje.github.io/). Written in C#, uses Firebase as default external database.

## Requirements

* [.NET Core 3.1](https://dotnet.microsoft.com/download)
* [ChromeDriver](https://chromedriver.chromium.org/downloads)

## Installation

1. Clone this repository
2. Run `dotnet run` command
3. Move to bin/Debug/netcoreapp3.1 directory and paste chromedriver.exe file
4. Create data folder in bin/Debug/netcoreapp3.1 directory
5. Make two files in data folder: groups.json and settings.json
6. In groups.json put example content  

```json
{
  "lastUpdateDate": "04/07/2020",
  "name": "sections",
  "groups": [
    {
      "category": "Nauka",
      "link": "https://facebook.com/groups/277362736085803",
      "members": 92106,
      "membersGrowth": 1516,
      "name": "Ciekawostkawka"
    },
    {
      "link": "https://facebook.com/groups/postnostalgawka",
      "members": 80412,
      "membersGrowth": 1073,
      "name": "Jak będzie w 2030?"
    },
    {
      "link": "https://facebook.com/groups/375622829809203",
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
  "files": [
    {
      "input": "groups.json",
      "output": "groups.json",
      "path": "/groups"
    }
  ],
  "settings": {
    "autoFix": true,
    "autoCompare": true,
    "autoUpdate": false,
    "shouldUpdateStatus": false
  }
}
```

1. In login field you have to put your Facebook account e-mail, and in password Facebook password
2. Run program by using `dotnet run` command, write 1 and click Enter, the application will run Chrome, login into your Facebook account and start scraping groups

## JSON schemas

1. settings.json

```typescript
export interface Setup {
    login:    string;
    password: string;
    files:    File[];
    settings: Settings;
}

export interface File {
    input:  string;
    output: string;
    path?:   string;
}

export interface Settings {
    firebaseLink?:       string;
    autoFix:             boolean;
    autoCompare:         boolean;
    autoUpdate:          boolean;
    shouldUpdateStatus:  boolean;
}
```

2. groups.json (you can name this file whatever you want, don't forget about updating it in settings.json file)

```typescript
export interface Setup {
    groups:         Group[];
    lastUpdateDate: string;
    name:           string;
}

export interface Group {
    category?:      string[] | string;
    link:           string;
    members:        number;
    membersGrowth?: number;
    name:           string;
    isSection?:     boolean;
    isOpen?:        boolean;
    keywords?:      string[];
}
```
