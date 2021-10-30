Command | Payload | Produced | Consumed
------- | -------- | -------- |---------------
Create popug | {PopugId} | Popug Manager | Tasks, Reporting
Assign role  | {PopugId, Role} | Popug Manager | Tasks, Reporting 
Create task | {TaskId, Text} | Tasks | Accounting, Reporting 
Assign task | {TaskId, PopugId} | Tasks | Accounting, Reporting  
Close task | {PopugId, TaskId} | Tasks | Accounting, Reporting   
Withdrawn price | {PopugId, TaskId, Ammount} | Accounting | Reporting  
Payed reward | {PopugId, TaskId, Ammount} | Accounting | Reporting 
Create report | {} | Scheduler |  Reporting
Send report | {PopugId} | Reporting | Reporting 
