# SWAPS [![Build](https://img.shields.io/github/workflow/status/litetex/SWAPS/Master%20CI/master)](https://github.com/litetex/SWAPS/actions?query=workflow%3A%22Master+CI%22) [![Latest Version](https://img.shields.io/github/v/release/litetex/SWAPS)](https://github.com/litetex/SWAPS/releases) ![Platform: Windows](https://img.shields.io/badge/windows-supported-5936b0.svg?logo=windows)
Start a program/process without a auto-starting service

### Requirements
* Windows
* User must be an administrator

### What it does
* Set service startup-type to demand
* Starts the service before launching the subprocess/program
* Stops the service after the subprocess/program has finished
* Create log files with ``-l``

### :wrench: Configuration 
#### :computer: CLI Args

| Arg | Description |
| --- | ----------- | 
| ``--genconf <path:optional>`` | Generates default config file in mentioned path  |
| | |
| ``-c <path:optional>`` ``--config <path:optional>`` | Path to the config file; if value not set: using defaults |
| ``-l`` ``--logfile`` | Writes logs as file(s) into ./logs |
| ``--startNotMin`` | Starts the process not minimized |
| ``-v`` ``--verbose`` | More logs (for debugging) |
| ``--showServerConsole`` | Shows the server console (for debugging) |
| ``--useUnencryptedCom`` | Uses no encryption for the communication between processes (for debugging; not recommended) |
| | |
| ``--update`` | Install the latest available update |
| ``--updatemode`` | Describes when updates are searched and installed; More info [here](docs/Updates.md) |

#### Config file
[Example config](example_config.json)

### Development [![Build Develop](https://img.shields.io/github/workflow/status/litetex/SWAPS/Check%20Build/develop?label=build%20develop)](https://github.com/litetex/SWAPS/actions?query=workflow%3A%22Check+Build%22+branch%3Adevelop)
