[![Build](https://img.shields.io/github/workflow/status/litetex/SWAPS/Master%20CI)](https://github.com/litetex/SWAPS/actions)
[![Latest Version](https://img.shields.io/github/v/release/litetex/SWAPS)](https://github.com/litetex/SWAPS/releases)
![Platform: Windows](https://img.shields.io/badge/platform-windows-0078d6.svg?logo=windows&labelColor=5936b0)]

[![Build Develop](https://dev.azure.com/litetex/SWAPS/_apis/build/status/Develop?label=build%20develop)](https://dev.azure.com/litetex/SWAPS/_build/latest?definitionId=1)

# SWAPS
Start a program/process without a auto-starting service

### Requirements
* Windows
* User must be an administrator

### What it does
* Set service startup-type to demand
* Starts the service before launching the subprocess/program
* Stops the service after the subprocess/program has finished
* Create logs with ``-l``

#### [Example config](example_config.json)
