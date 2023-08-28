/*Create new login*/
IF NOT EXISTS 
    (SELECT name  
     FROM master.sys.server_principals
     WHERE name = 'bpapiuser')
BEGIN
    CREATE LOGIN bpapiuser WITH PASSWORD = 'Bpapipassword!'--CHANGE PASSWORD IF NEEDED (AND CORRESPONDINGLY IN APPSETTINGS.JSON
END
GO

/*Switch to a Blue Prism DB. IF YOUR BP DB IS NAMED DIFFERENTLY, CHANGE 'BluePrism' to that name*/
USE [BluePrism]
GO

/*Create 5 new views, each with a prefix cstm_*/
CREATE VIEW [dbo].[cstm_BPVAvailableProcesses]
AS
SELECT        dbo.BPAProcess.processid, dbo.BPAProcess.name AS ProcessName, dbo.BPAProcess.description AS ProcessDescription, LEFT(dbo.BPAProcess.processxml, 16384) AS ProcessInput, dbo.BPAGroup.name AS GroupName
FROM            dbo.BPAProcess INNER JOIN
                         dbo.BPAGroupProcess ON dbo.BPAProcess.processid = dbo.BPAGroupProcess.processid INNER JOIN
                         dbo.BPAGroup ON dbo.BPAGroupProcess.groupid = dbo.BPAGroup.id
WHERE        (dbo.BPAProcess.AttributeID = 2) AND (dbo.BPAProcess.ProcessType = 'P')
GO

CREATE VIEW [dbo].[cstm_BPVEnvironment]
AS
SELECT        dbo.BPASession.sessionid AS SessionId, dbo.BPASession.sessionnumber AS ID, dbo.BPAProcess.name AS Process, dbo.BPAResource.name AS 'Resource', dbo.BPAUser.username AS 'User', 
                         dbo.BPAStatus.description AS 'Status', dbo.BPASession.startdatetime AS StartTime, dbo.BPASession.enddatetime AS EndTime, dbo.BPASession.laststage AS LatestStage
FROM            dbo.BPASession INNER JOIN
                         dbo.BPAProcess ON dbo.BPAProcess.processid = dbo.BPASession.processid INNER JOIN
                         dbo.BPAResource ON dbo.BPAResource.resourceid = dbo.BPASession.runningresourceid INNER JOIN
                         dbo.BPAUser ON dbo.BPAUser.userid = dbo.BPASession.starteruserid INNER JOIN
                         dbo.BPAStatus ON dbo.BPAStatus.statusid = dbo.BPASession.statusid
GO

CREATE VIEW [dbo].[cstm_BPVQueueContents]
AS
SELECT        dbo.BPAWorkQueueItem.id AS QueueItemId, CASE WHEN dbo.BPACaseLock.locktime IS NOT NULL THEN 'Locked' WHEN [completed] IS NULL AND [exception] IS NULL THEN 'Pending' WHEN [completed] IS NOT NULL AND 
                         [exception] IS NULL THEN 'Completed' ELSE 'Exception' END AS State, dbo.BPAWorkQueueItem.queueid, dbo.BPAWorkQueueItem.keyvalue AS ItemKey, dbo.BPAWorkQueueItem.priority, dbo.BPAWorkQueueItem.status,
                             (SELECT        dbo.BPATag.tag + ';'
                               FROM            dbo.BPATag INNER JOIN
                                                         dbo.BPAWorkQueueItemTag ON dbo.BPAWorkQueueItemTag.tagid = dbo.BPATag.id
                               WHERE        (dbo.BPAWorkQueueItemTag.queueitemident = dbo.BPAWorkQueueItem.ident) FOR XML PATH('')) AS Tag, dbo.BPAResource.name AS Resource, dbo.BPAWorkQueueItem.attempt, 
                         dbo.BPAWorkQueueItem.loaded AS Created, dbo.BPAWorkQueueItem.lastupdated, dbo.BPAWorkQueueItem.deferred AS NextReview, dbo.BPAWorkQueueItem.completed, dbo.BPAWorkQueueItem.worktime AS TotalWorkTime, 
                         dbo.BPAWorkQueueItem.exception AS ExceptionDate, dbo.BPAWorkQueueItem.exceptionreason                                                                                                                                                                                                                                                         
 FROM            dbo.BPAWorkQueueItem INNER JOIN
                         dbo.BPASession ON dbo.BPASession.sessionid = dbo.BPAWorkQueueItem.sessionid INNER JOIN
                         dbo.BPAResource ON dbo.BPAResource.resourceid = dbo.BPASession.runningresourceid LEFT JOIN
                         dbo.BPACaseLock ON dbo.BPACaseLock.id = dbo.BPAWorkQueueItem.ident
GO

CREATE VIEW [dbo].[cstm_BPVQueueManagement]
AS
SELECT        dbo.BPAWorkQueue.id AS WorkQueueId, dbo.BPAWorkQueue.name AS WorkQueueName, dbo.BPAGroup.name AS WorkQueueGroupName, CASE WHEN [running] = 0 THEN 'Paused' ELSE 'Running' END AS Status,
                             (SELECT        COUNT(*) AS Expr1
                               FROM            dbo.BPAWorkQueueItem
                               WHERE        (queueid = dbo.BPAWorkQueue.id) AND (finished IS NOT NULL) AND (exception IS NULL)) AS WorkedItems,
                             (SELECT        COUNT(*) AS Expr1
                               FROM            dbo.BPAWorkQueueItem AS BPAWorkQueueItem_4
                               WHERE        (queueid = dbo.BPAWorkQueue.id) AND (finished IS NULL)) AS PendingItems,
                             (SELECT        COUNT(*) AS Expr1
                               FROM            dbo.BPAWorkQueueItem AS BPAWorkQueueItem_3
                               WHERE        (queueid = dbo.BPAWorkQueue.id) AND (exception IS NOT NULL)) AS ReferredItems,
                             (SELECT        COUNT(*) AS Expr1
                               FROM            dbo.BPAWorkQueueItem AS BPAWorkQueueItem_2
                               WHERE        (queueid = dbo.BPAWorkQueue.id)) AS TotalItems,
                             (SELECT        AVG(worktime) AS Expr1
                               FROM            dbo.BPAWorkQueueItem AS BPAWorkQueueItem_1
                               WHERE        (queueid = dbo.BPAWorkQueue.id)) AS AverageCaseDuration
FROM            dbo.BPAWorkQueue LEFT OUTER JOIN
                         dbo.BPAGroupQueue ON dbo.BPAGroupQueue.memberid = dbo.BPAWorkQueue.ident LEFT OUTER JOIN
                         dbo.BPAGroup ON dbo.BPAGroup.id = dbo.BPAGroupQueue.groupid
GO

CREATE VIEW [dbo].[cstm_BPVResources]
AS
SELECT        dbo.BPAResource.resourceid, dbo.BPAResource.name AS ResourceName, dbo.BPAGroup.name AS ResourceGroupName, CASE WHEN [DisplayStatus] IS NOT NULL AND 
                         [DisplayStatus] <> 'Private' THEN [DisplayStatus] WHEN [processesrunning] = 0 THEN 'Idle' WHEN [processesrunning] = 1 AND 
                         [actionsrunning] = 0 THEN 'Idle-Pending' ELSE 'Working' END AS ResourceStatus
FROM            dbo.BPAResource INNER JOIN
                         dbo.BPAGroupResource ON dbo.BPAGroupResource.memberid = dbo.BPAResource.resourceid INNER JOIN
                         dbo.BPAGroup ON dbo.BPAGroup.id = dbo.BPAGroupResource.groupid
WHERE        (dbo.BPAResource.DisplayStatus IS NOT NULL) AND (dbo.BPAResource.statusid <> 2)
GO

/*Create user in BP DB for a corresponding login*/
CREATE USER bpapiuser FOR LOGIN bpapiuser
GO

/*Grant readonly rights for 5 cstm_ views to created user*/
GRANT SELECT ON cstm_BPVAvailableProcesses TO bpapiuser
GO

GRANT SELECT ON cstm_BPVEnvironment TO bpapiuser
GO

GRANT SELECT ON cstm_BPVQueueContents TO bpapiuser
GO

GRANT SELECT ON cstm_BPVQueueManagement TO bpapiuser
GO

GRANT SELECT ON cstm_BPVResources TO bpapiuser
GO