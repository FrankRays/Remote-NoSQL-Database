BUILDING/GENERATING
Author: Rohit Sharma
-------------------------------------------------------------------------------------------
STEP1 - compile.bat // used to compile
STEP2 - run.bat	// will run the executive.(demonstrates the requirements)
-----------------------------------------------EOF-----------------------------------------

Notes: 

1) ProcessStarter is the main Test Executive for this project.
2) Persist xml file will be store in root folder.
3) Restore XML can be done on both int-string and string-list of string database.
4) There is one Main Reader on GUI which can be used to query the database manually. Apart from that, multiple readers and writers can be 
	started from console by providing the number of clients to be started.
5) Search Key and Pattern have to be specified in the XML after looking at the current records in DB otherwise status will be "No record found".
6) I have used <string, List<string> to demonstrate search queries.
7) I have used <int,string> to demonstrate insert queries.
8) Total number of records can be provided in reader client XML.
9) Number of records can be provided in writer xml separately for isertion, deletion, edit.