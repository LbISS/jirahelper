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
  -c, --config    Path to config file. See [Config file](https://github.com/LbISS/jirahelper#config-file).

  --help          Display this help screen.

  --version       Display version information.
  ```

# Run as a web application
To run app as a web application you need to build & publish `JiraHelper.Web` project and run it using any web application server (IIS, Apache, etc.). For temp purposes the project could be started on IISExpress using debug command from the Visual Studio.

The application provides one simple controller `StrategiesController` which will act on the POST requests sent to the `/Strategies/**key**` endpoint, where **key** is the active strategy key from the config.

# Config file
The config file is describing final logic and composition of semantic blocks which will be run as well as general properties. 

The Jira section is requiring URI, username and password - for receiving data from Jira server.

The entry point for business logic is "Strategy" node, which is containing
- Mode - will the strategy work in background by timer (IBackgroundStrategy) or will be called using web endpoints (IActiveStrategy);
- Key - unique string key, should be alphanumeric without space/special chars/etc.;
- Type - Class name for execution with the assembly name;
- Parameters - all other parameters.
Parameters will be used to fill in parameters in strategy constructor. Most of the default strategies have Checker, Storage and Action parameters. In theirs turn they have "Type" and "Parameters" keys available.

If you want to see some examples of config please refer to [Sample scenarios](https://github.com/LbISS/jirahelper#sample-scenarios)

## Default strategies

### JiraHelper.Core.Business.Strategy.GetFromStorageStrategy
Gets data from the specified storage and returns to the endpoint.
### JiraHelper.Core.Business.Strategy.GetStrategy
Gets data from the specified checker and returns to the endpoint.
### JiraHelper.Core.Business.Strategy.NewAndClosedStrategy
Gets list of issues from the checker, stores it to storage and monitors changes - sending them to action.
### JiraHelper.Core.Business.Strategy.SaveStrategy
Saves data from the checker to the specified storage.

## Default checkers
### JiraHelper.Core.Business.Checkers.FilterChecker
Gets the list of issues from the Jira filter by filterId.

## Default storages
### JiraHelper.Core.Business.Storages.CsvFileStorage
Stores data in csv file.
### JiraHelper.Core.Business.Storages.JsonFileStorage
Stores data in json file.

## Default actions
### JiraHelper.Core.Business.Storages.MSTeamsSimpleNotifyAction
Action which notifies about changes in MSTeams channel using webhooks.

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

If you think that you've done great job and created new good strategy/action/etc. you could create a pull-request in Core project (if your code could be widely reused) or in Example project (if there some specifics).

## Sample scenarios

> The sample of config.file provided in the `JiraHelper.Console/config.sample.json`

Assembly JiraHelper.Example required to run sample scenarios. The JiraHelper.Example.dll should be placed in the folder /strategies/ at the working path of JiraHelper.Console.
config.sample.json should be renamed to config.json, Jira data and strategy properties should be filled in and the file should be passed to the utility as commandline parameter or placed in the working path.

### Scenario 1. Notifying about new high-priority jira issue

First example is checking periodically saved filter in Jira - here we will check for new high priority(blocker) tickets. If new issue will appear - it'll send the notification to the MSTeams webhook. If issue will dissapear from filter - it's considered closed and another notification will be sent.

So let's describe what's going on line-by-line:

``` "Mode": "JiraHelper.Core.Business.Strategy.IBackgroundStrategy",```

The strategy will be run periodically in the background.

``` "Type": "JiraHelper.Core.Business.Strategy.NewAndClosedStrategy",```

It will monitor new and closed issues and send perform action on them.

``` "Key": "blockers", ```

Some unique key.

``` 
"Checker": {
	"Type": "JiraHelper.Core.Business.Checkers.FilterChecker",
	"Parameters": {
		"filterId": "12345"
	}
},
```
For getting issue we will use FilterChecker with filter saved in jira under Id 12345.

``` 
"Storage": {
	"Type": "JiraHelper.Core.Business.Storage.JsonFileStorage",
	"Parameters": {
		"filePath": "data/blockers.json"
	}
},
```

After getting data from jira we will save it to "data/blockers.json".

```
"Action": {
	"Type": "JiraHelper.Example.Actions.MSTeamsNotifyBlockerAction",
	"Parameters": {
		"filePath": "data/blockers.json",
		"webhookUri": "https://webhook.office.com/webhookb2/someguid@somegiud/IncomingWebhook/@someId/@someguid",
		"jiraUri": "https://somejira.com/"
	}
}
```

Then strategy will compare data gathered from Checker with the data saved on previous run with storage and pass new and closed issues to action. Action will send it to MS Teams channel.

### Scenario 2. Creating endpoints for some UI based on jira data

Another simple example exposing two "endpoints" via two strategies as both of them implementing `IActiveStrategy`.

First one is `IterationSaveStrategy` which will gather data from provided filter with id 12346(using `FilterChecker`) and save it to the `data/list.json` file using(using `JsonFileStorage`).
So, the result of sending POST request to the endpoint `Strategies/**iterationSave**` (Strategies/**key**) will be issues gathered from Jira and saved on the disc.

The second one is `IterationGetFilterStrategy`. It'll simply return data (if any) from previously saved file.
Result of sending POST request to the endpoint `Strategies/iterationGetFromFile` will be issues gathered from disc.

If you'll create such config and run application as a web application you can easily create some ui on top of it showing list of issues from the disc with the "refresh" button which will refresh the issues from the Jira.