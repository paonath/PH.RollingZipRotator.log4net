# PH.RollingZipRotator.log4net [![NuGet Badge](https://buildstats.info/nuget/PH.RollingZipRotatorLog4net)](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/)

A Zip utility to perform a very simple log4net file rotation.
The code perform a zip-compression on every log-rotated file and delete it, watching on 
log4net output directory reading settings of appenders.


The package is available on  [nuget](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/) 

## Code Examples

**Perform a Rotation**
```c#
//already configured log4net logger...
var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();
//remember to star watching!
instance.StartWatch();
```
