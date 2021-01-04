# :wrench: Configuration 
## :computer: CLI Args

There are 3 operation modes:<br>
``SWAPS.exe <operationmode:optional> <args>``

Standard Args that can always be used:
| Arg | Description |
| --- | ----------- | 
| ``-l`` ``--logfile`` | Writes logs as file(s) into ./logs |
| ``--logFileRetainCount {number}`` | The maximum number of log files that will be retained, including the current log file. For unlimited retention, pass -1. The default is 31. |
| ``-v`` ``--verbose`` | More logs (for debugging) |

### Operation modes
#### ``run`` / default
Default mode if nothing is specified.<br>
Starts a normal workflow run.

| Arg | Description |
| --- | ----------- | 
| ``-c <path:optional>`` ``--config <path:optional>`` | Path to the config file; if value not set: using defaults |
| | |
| ``--startNotMin`` | Starts the process not minimized |
| ``--showServerConsole`` | Shows the server console (for debugging) |
| ``--useUnencryptedCom`` | Uses no encryption for the communication between processes (for debugging; not recommended) |
| | |
| ``--updatemode`` | Describes when updates are searched and installed; More info [here](Updates.md) |
| ``--byPassUpdateLoopProtection`` | Bypasses the updateloop protection |

#### ``genconfig``
Generates configuration

| Arg | Description |
| --- | ----------- | 
| ``--path <path:optional>`` | Generates default config file in mentioned path  |

#### ``update``
Forces an update

| Arg | Description |
| --- | ----------- | 
| ``--byPassUpdateLoopProtection`` | Bypasses the updateloop protection |

## Config file
Example configs:

``liveupdate.json``
```JSON
{
  "Name": "LiveUpdate_Launcher",
  "ServiceConfigs": [
    {
      "ServiceName": "MSI_LiveUpdate_Service"
    }
  ],
  "ProcessConfigs": [
    {
      "Key": "abc",
      "WorkDir": "C:\\Program Files (x86)\\MSI\\Live Update",
      "FilePath": "C:\\Program Files (x86)\\MSI\\Live Update\\Live Update.exe",
      "Args": "/START",
      "Timeout": null,
      "Async": false,
      "DependsOn": []
    }
  ],
  "CrashOnUpdateServiceNotFound": false,
  "ServiceStartTimeout": "00:00:10",
  "ServiceProperlyStartedDelay": "00:00:01",
  "ServiceShutdownDelay": "00:00:01",
  "StayingOpenBeforeEnding": "00:00:00.5000000"
}
```

``openvpn.json``
```JSON
{
  "Name": "OpenVPN_Launcher",
  "ServiceConfigs": [
    {
      "ServiceName": "agent_ovpnconnect"
    },
	{
      "ServiceName": "ovpnhelper_service"
    }
  ],
  "ProcessConfigs": [
    {
      "Key": null,
      "WorkDir": "C:\\Program Files\\OpenVPN Connect\\",
      "FilePath": "C:\\Program Files\\OpenVPN Connect\\OpenVPNConnect.exe",
      "Args": null,
      "Timeout": null,
      "Async": false,
      "DependsOn": []
    }
  ],
  "CrashOnUpdateServiceNotFound": false,
  "ServiceStartTimeout": "00:00:10",
  "ServiceProperlyStartedDelay": "00:00:00",
  "ServiceShutdownDelay": "00:00:01",
  "StayingOpenBeforeEnding": "00:00:00.5000000"
}
```
