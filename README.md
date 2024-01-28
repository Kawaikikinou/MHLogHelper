This solution contains several tools to work with Marvel Heroes logs :<br>
<br>
- <b>RegionGenerationLogAnalyser</b>:<br>
    Put the log file with generation map information from the server with the one from the client in the folder "LogsToCompare".<br>
    The files must be named with this patten "RegionName-[S/C]-Seed" with S for Server and C for Client.<br>
    For instance "CH0301MadripoorRegion-C-1488502313.txt" and "CH0301MadripoorRegion-C-1488502313.txt"<br>
    It will compare the files and generate a file "AnalyseResult.txt" in the bin folder of the project.<br>

- <b>DrawMapFromLog</b> :<br>
    From a log file with generation map information, draw the cells created.<br>
    Put the log to draw in the folder "LogsToDraw".<br>
    TODO : More test and consider case when deallocation occured<br>
    
- More projects could be added
