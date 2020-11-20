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

#### [Example config](example_config.json)

### Development [![Build Develop](https://img.shields.io/github/workflow/status/litetex/SWAPS/Check%20Build/develop?label=build%20develop)](https://github.com/litetex/SWAPS/actions?query=workflow%3A%22Check+Build%22+branch%3Adevelop)
