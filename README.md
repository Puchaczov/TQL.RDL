## RDL is domain specific language to querying time.

RDL is domain specific language usable to querying time. Syntax is similar to english language. It's goal is to allow typing every pattern you could imagine and express it in form closest to english sentence. It can be used for generating next occurences of events and as schedule time feeder for scheduler. It should be perfect as complementary for CRON expressions.

## Features

- Written in C# (.NET)
- Nuget availibility
- Usable with CRON expressions (shares same abstractions)
- Easy to use
- Various timeline resolution (seconds, minutes, hours, days, months, years)
- Extensible by providing own functions

## Examples

- In a monday of first or third week of month, get dates from 8:30AM to 11:29AM, for second and fourth, return 12:00 but only when day of week is tuesday or sunday.
```
repeat every minutes where 1 = 
(case 
    when GetWeekOfMonth() in (1,3) and GetDayOfWeek() = monday
    then GetTime() between Time(8, 30, 0) and Time(11, 30, 0)
    when GetWeekOfMonth() in (2,4) and GetDayOfWeek() in (tuesday, sunday)
    then GetTime() = Time(12, 0, 0)
    else 0 esac) start at '01.04.2017' stop at '30.04.2017'
```

- Get dates of tuesday and wednesday that are in first week of month and dates of thursday and friday in the third week of month starting from 01.12.2016

```  
repeat every days where 1 = 
(case
    when GetWeekOfMonth() = 1 and GetDayOfWeek() in (tuesday,wednesday) then 1 
    when GetWeekOfMonth() = 3 and GetDayOfWeek() in (thursday,friday) then 1 
    else 0 esac) start at '01.12.2016'
```

- Occurs every days at 7:30 am and 8:30 am

```
repeat every minutes where 
    (GetHour() = 7 and GetMinute() = 0 and GetSecond() = 0) or 
    (GetHour() = 8 and GetMinute() = 30 and GetSecond() = 0) 
    start at '04.01.2017'
```

## Installation

Download and install the latest version (nuget): **Install-Package TQL.RDL**

## Contributors

- Fork the repo
- Create new branch
- Change source code and commit & push your changes
- Create pull request

If you found a bug, open an issue and type query you couldn't evaluate. Feel free to request new functionality i you need something.

## License

Apache 2.0
