# SWAPS
Start programm without auto-starting service

### Requirements
* Windows
* User must be an administrator

### What it does
* Set service startup-type to demand
* Starts the service before launching the subprocess/program
* Stops the service after the subprocess/program has finished
* Create logs with ``-l``

### Example config
[example_config.json](example_config.json)
