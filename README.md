# JiraHelper
.Net tool for downloading Jira issues/notify about issues changes with extension capabilities.

Have two modes: Console and as Web Application.

# Run as console application
Example:
```
JiraHelper.console.exe -c config.json
```

Parameters:
```
  -c, --config    Path to config file. See TODOLINK Config file

  --help          Display this help screen.

  --version       Display version information.
  ```

# Run as a web application
TODO

# Config file
TODO: Jira section

The config file is describing final logic and composition of semantic blocks which will be run. The entry point is "Strategy" node, which is containing
- Mode - will the strategy work in background by timer (IBackgroundStrategy) or will be called once/using web endpoints (IActiveStrategy)
- Key - unique string key
- Type - Class name for execution
- Parameters - all other parameters.
Parameters will be used to fill in parameters in strategy constructor. Most of the default strategies have Checker, Storage and Action parameters. In theirs turn they have "Type" and "Parameters" keys available.

The sample of config.file provided in the `JiraHelper.Console/config.sample.json`. See TODOLINK Sample scenarios

## Default strategies

- TODO
- TODO

## Default checkers

- TODO
- TODO

## Default storages

- TODO
- TODO

## Default actions

- TODO
- TODO

# General architecture
Application working with the objects called "Strategy". Strategy is determined by it's Checker, Storage and Action. They are working in the corresponding order.

Checker is responsible for checking source (Jira/File/Etc.) and generating set of data.

Storage is responsible for storing data received from Checker in file/database/etc.

Action is responsible for any action which could be based on data - sending notifications, trigger state changes, etc.

# Extensions capabilities
The software supports extending capabilities. If you find this tool useful, but you need more, than default strategies you can write your own. 

You will need to produce a CLR-compatible library with .dll extension and place it inside `/strategies/` folder at the working path of main executable.

The library could introduce implementations of core interfaces such as `IBackgroundStrategy`, `IActiveStrategy`, `IChecker`, `IStorage`, `IAction` and then use this implementation in the config file.

The library could implement any additional classes and extend abstract and specific classes of JiraHelper.Core library.

In the JiraHelper.Example project some custom strategies are introduced and could be used as an example.

## Sample scenarios

Assembly JiraHelper.Example required to run sample scenarios. The JiraHelper.Example.dll should be placed in the folder /strategies/ at the working path of JiraHelper.Console.
config.sample.json.


### Scenario 1. Notifying about new high-priority jira issue

First example is checking periodically saved filter in Jira. If new issue will appear - it'll send the notification to the MSTeams webhook. If issue will dissapear from filter - it's considered closed and another notification will be sent.

TODO: Go line by line

