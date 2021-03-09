Send both PROJUNK.286 and PROJUNK.CFG in to the C drive of the affected store. 
Trigger the application by specifying the date as below.

PROJUNK <DATE in YYMMDD> eg: PROJUNK 150425. Which will fetch the correct DQ2 file
and auto corrects the affected transaction.Then the utility calls QCONSOLE to replay 
the transaction to SAP PI. Once successful program shows the status to the user.

Execution status  of the application can be found by checking either PROJUNK.ERR/PROJUNK.OK. 

Replayed transaction will be kept in file C:\PROJUNK.DAT.
