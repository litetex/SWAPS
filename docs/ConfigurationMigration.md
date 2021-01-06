# Configuration Migration
Guide about what to update on a new configuration version

## Configuration-Versions

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
