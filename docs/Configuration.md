## :wrench: Configuration 
### :computer: CLI Args

| Arg | Description |
| --- | ----------- | 
| ``--genconf <path:optional>`` | Generates default config file in mentioned path  |
| | |
| ``-c <path:optional>`` ``--config <path:optional>`` | Path to the config file; if value not set: using defaults |
| ``-l`` ``--logfile`` | Writes logs as file(s) into ./logs |
| ``--logFileRetainCount {number}`` | The maximum number of log files that will be retained, including the current log file. For unlimited retention, pass -1. The default is 31. |
| ``--startNotMin`` | Starts the process not minimized |
| ``-v`` ``--verbose`` | More logs (for debugging) |
| ``--showServerConsole`` | Shows the server console (for debugging) |
| ``--useUnencryptedCom`` | Uses no encryption for the communication between processes (for debugging; not recommended) |
| | |
| ``--update`` | Install the latest available update |
| ``--updatemode`` | Describes when updates are searched and installed; More info [here](Updates.md) |
| ``--byPassUpdateLoopProtection`` | Bypasses the updateloop protection |

### Config file
Example config:

``example_config.json``
```JSON
{
  "Name": "SomeUpdater_NoAutoService_Launcher",
  "ServiceConfig": {
    "ServiceName": "Some_Updater_Service"
  },
  "ProcessConfig": {
    "WorkDir": "C:\\Program Files (x86)\\Some\\Live Update",
    "FilePath": "C:\\Program Files (x86)\\Some\\Live Update\\Live Update.exe",
    "Args": "/START",
    "Timeout": null
  },
  "CrashOnUpdateServiceNotFound": false,
  "ServiceStartTimeout": "00:00:10",
  "ServiceProperlyStartedDelay": "00:00:01",
  "ServiceShutdownDelay": "00:00:01",
  "StayingOpenBeforeEnding": "00:00:00.5000000"
}
```
