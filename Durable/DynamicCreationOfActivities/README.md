# Azure Function Dynamic Example
This shows an example of an orchestrator that calls an activity to determine which steps it will execute.

By default it will run job #4 (See ActivityGetExecutionSteps.cs for which activities will be called by job 4)

You can switch jobs by doing the following:
http://localhost:7071/api/JobRunner?jobId=1