This solution contains several tools for logs :

- RegionGenerationLogAnalyser
    Put the log file with generation map information from the server with the one from the client in the folder "LogsToCompare".
    The files must be named with this patten "RegionName-[S/C]-Seed" with S for Server and C for Client.
    For instance "CH0301MadripoorRegion-C-1488502313.txt" and "CH0301MadripoorRegion-C-1488502313.txt"
    It will compare the files and generate a file "AnalyseResult.txt" in the bin folder of the project.
  
- DrawMapFromLog
    From a log file with generation map information, draw the cells created.
    Put the log to draw in the folder "LogsToDraw".
    TODO : More test and consider case when deallocation occured

- More projects could be added
