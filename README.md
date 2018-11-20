# PH.RollingZipRotator.log4net [![NuGet Badge](https://buildstats.info/nuget/PH.RollingZipRotatorLog4net)](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/)

A Zip utility to perform a very simple log4net file rotation.
The code perform a zip-compression on every log-rotated file and delete it, watching on 
log4net output directory reading settings of appenders.


On every zip files after first TimeSpan created perform a zip-group.
On every zip files after second TimeSpan created perform a zip-archive and set archive attribute.

The package is available on  [nuget](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/) 

## Code Examples

**Perform a Rotation every 1 minutes and a second archive rotation every 10 min**
```c#
//already configured log4net logger...
var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.Create(new TimeSpan(0, 1, 0), new TimeSpan(0, 10, 0));
//remember to star watching!
instance.StartWatch();
```
