# PH.RollingZipRotator.log4net [![NuGet Badge](https://buildstats.info/nuget/PH.RollingZipRotatorLog4net)](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/)

A Zip utility to perform a very simple log4net file rotation.
The code perform a zip-compression on every log-rotated file and delete it, watching on 
log4net output directory reading settings of appenders.


Can be configured using a config xml file such [example.cfg.xml](./example.cfg.xml)

The package is available on  [nuget](https://www.nuget.org/packages/PH.RollingZipRotatorLog4net/) 

## Code Examples

**Start Performing Rotation With Config File**
```c#
//already configured log4net logger...
var configPath = @"a config xml file path";
 var instanceRunning =
                PH.RollingZipRotatorLog4net.RollingFileFactory.GetConfig(configPath).StartWatch();

```


**Perform a Rotation**
```c#
//already configured log4net logger...
var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();
//remember to start watching!
instance.StartWatch();
```

**Enable Debug Mode**
```c#
//already configured log4net logger with Debug level...
var instance =
                PH.RollingZipRotatorLog4net.RollingFileFactory.CreateSimple();

instance.DebugEnabled(true);
//remember to start watching!
instance.StartWatch();
```
