# :wrench: Configuration 
## :computer: CLI Args

There are 3 operation modes:<br>
``SWAPS.exe <operationmode:optional> <args>``

Standard Args that can always be used:
| Arg | Description |
| --- | ----------- | 
| ``-l``<br>``--logfile`` | Writes logs as file(s) into ./logs |
| ``--logFileRetainCount {number}`` | The maximum number of log files that will be retained, including the current log file. For unlimited retention, pass -1. The default is 31. |
| ``-v``<br>``--verbose`` | More logs (for debugging) |

### Operation modes
#### ``run`` / default
Default mode if nothing is specified.<br>
Starts a normal workflow run.

| Arg | Description |
| --- | ----------- | 
| ``-c <path:optional>``<br>``--config <path:optional>`` | Path to the config file; if value not set: using defaults |
| ``--abortOnConfigVersionMismatch`` | Abort on configuration file version mismatch |
| ``--lockFileFoundMode`` | Describes what is done, when a lockfile is found for the configuration; More info [here](Lockfile.md) |
| | |
| ``--startNotMin`` | Starts the process not minimized |
| ``--showServerConsole`` | Shows the server console (for debugging) |
| ``--useUnencryptedCom`` | Uses no encryption for the communication between processes (for debugging; not recommended) |
| | |
| ``--updatemode`` | Describes when updates are searched and installed; More info [here](Updates.md) |
| ``--byPassUpdateLoopProtection`` | Bypasses the updateloop protection |

#### ``genconfig``
Generates the configuration file

| Arg | Description |
| --- | ----------- | 
| ``-p <path:optional>``<br>``--path <path:optional>`` | Generates default config file in mentioned path  |

#### ``update``
Updates the program (if possible)

| Arg | Description |
| --- | ----------- | 
| ``--byPassUpdateLoopProtection`` | Bypasses the updateloop protection; Forces an update |

## Config file
→ see also: [Migration notes](ConfigurationMigration.md)

Example configs:

``liveupdate.json``
```JSON
{
  "Version": 3,
  "Name": "LiveUpdate_Launcher",
  "LockFile": {
    "Enabled": false,
    "LockFileExtension": ".lock",
    "ReNewLockFileAfter": "1.00:00:00",
    "LockFileInvalidAfter": "3.00:00:00"
  },
  "Services": {
    "CrashOnUpdateServiceNotFound": true,
    "StartTimeout": "00:00:10",
    "ProperlyStartedDelay": "00:00:01",
    "ShutdownDelay": "00:00:01",
    "Configs": [
      {
        "ServiceName": "MSI_LiveUpdate_Service",
        "CrashOnUpdateServiceNotFound": null,
        "StartTimeout": null
      }
    ]
  },
  "Processes": {
    "Configs": [
    {
      "Key": "live update",
      "WorkDir": "C:\\Program Files (x86)\\MSI\\Live Update",
      "FilePath": "C:\\Program Files (x86)\\MSI\\Live Update\\Live Update.exe",
      "Args": "/START",
      "Timeout": null,
      "Async": false,
      "DependsOn": []
    }
  ]
  },
  "StayingOpenBeforeEnding": "00:00:00.5000000"
}
```

``openvpn.json`` - Only with required and overridden  fields
```JSON
{
  "Version": 3,
  "Name": "OpenVPN_Launcher",
  "Services": {
    "CrashOnUpdateServiceNotFound": true,
    "Configs": [
      {
        "ServiceName": "agent_ovpnconnect"
      },
      {
        "ServiceName": "ovpnhelper_service"
      }
    ]
  },
  "Processes": {
    "Configs": [
      {
        "WorkDir": "C:\\Program Files\\OpenVPN Connect\\",
        "FilePath": "C:\\Program Files\\OpenVPN Connect\\OpenVPNConnect.exe",
        "DependsOn": []
      }
    ]
  }
}
```