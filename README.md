## RDL is small language usefull to querying time.

RDL is parser and evaluator for small language to querying time. Syntax is similar to english language. It's goal is to allow typing every pattern you could imagine and express it in form closest to english sentence. It can be used for generating next occurences of some event and as datetime feeder for scheduler. It should be perfect as complementary for CRON expressions.

## Features

- Written in C# (.NET)
- Nuget availibility
- Usable with CRON expressions (shares same abstractions)
- Easy to use
- Various timeline resolution (seconds, minutes, hours, days, months, years) 

## Examples

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

## Tests and examples

Describe and show how to run the tests with code examples.

## Contributors

- Fork the repo
- Create new branch
- Change source code and commit & push your changes
- Create pull request

If you found a bug, open an issue and type query you couldn't evaluate. Feel free to request new functionality i you need something.

## License

Apache 2.0
