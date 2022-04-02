# JiraHelper
.Net tool for downloading Jira issues/notify about issues changes with extension capabilities.

Have two modes: Console and as Web Application.

# Run as console application
TBD

# Run as a web application
TBD

# General architecture
Application working with the objects called "Strategy". Strategy is determined by it's Checker, Storage and Action. They are working in the corresponding order.
Checker is responsible for checking source (Jira/File/Etc.) and generating set of data.
Storage is responsible for storing data received from Checker in file/database/etc.
Action is responsible for any action which could be based on data - sending notifications, trigger state changes, etc.

# Extensions capabilities
TBD
