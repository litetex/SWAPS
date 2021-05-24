# Configuration Migration
Guide about what to update on a new configuration version

Note: You can [always generate a new configuration](Configuration.md#genconfig) to see the changes

## Configuration-Versions

### v3
<details>
<summary>
<b>Examples</b>
</summary>

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

``openvpn.json`` - Not required fields are removed
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

</details>

#### Changes
* Updated version property<br>``  "Version": 3``
* ``LockfileConfig`` was renamed to ``Lockfile``
* ``ServiceConfigs [array]`` was moved to ``Services.Configs [array]``
* ``ProcessConfigs [array]`` was moved to ``Processes.Configs [array]``
* Moved ``CrashOnUpdateServiceNotFound`` to ``Processes.CrashOnUpdateServiceNotFound`` (default) and ``Processes.Configs[object].CrashOnUpdateServiceNotFound`` (override)
* Moved and renamed ``ServiceStartTimeout`` to ``Processes.StartTimeout`` (default) and ``Processes.Configs[object].StartTimeout`` (override)
* Moved ``ServiceProperlyStartedDelay`` to ``Processes.ProperlyStartedDelay``
* Moved ``ServiceShutdownDelay`` to ``Processes.ShutdownDelay``

### v2.1 
#### Changes
* Added new ``LockFileConfig`` property

### v2
<details>
<summary>
<b>Examples</b>
</summary>

``liveupdate.json``
```JSON
{
  "Version": 2,
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
  "Version": 2,
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
  "CrashOnUpdateServiceNotFound": true,
  "ServiceStartTimeout": "00:00:10",
  "ServiceProperlyStartedDelay": "00:00:00",
  "ServiceShutdownDelay": "00:00:01",
  "StayingOpenBeforeEnding": "00:00:00.5000000"
}
```

</details>

#### Changes
* Added version property<br>``  "Version": 2``
* ``ServiceConfig [single object]`` was replaced by ``ServiceConfigs [array]``
* ``ProcessConfig [single object]`` was replaced by ``ProcessConfigs [array]``
* The ProcessConfig now contains:<br>``Key [string]``<br>``Async [bool]``<br>``DependensOn [string array]``

### v1
<details>
<summary>
<b>Example</b>
</summary>

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

</details>
