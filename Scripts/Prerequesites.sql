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

/*Create 6 new views, each with a prefix cstm_*/
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

CREATE VIEW [dbo].[cstm_BPVLogs]
AS
SELECT 
ROW_NUMBER() OVER(order by sessionnumber desc) as LogId,
       sessionnumber as SessionNumber
      ,stagename as StageName
      ,CASE stagetype
	  WHEN 2 THEN 'Action'
	  WHEN 4 THEN 'Decision'
	  WHEN 8 THEN 'Calculation'
	  WHEN 64 THEN 'Process'
	  WHEN 128 THEN 'Page'
	  WHEN 1024 THEN 'Start'
	  WHEN 2048 THEN 'End'
	  WHEN 8192 THEN 'Note'
	  WHEN 16384 THEN 'Loop Start'
	  WHEN 32768 THEN 'Loop End'
	  WHEN 65536 THEN 'Read'
	  WHEN 131072 THEN 'Write'
	  WHEN 262144 THEN 'Navigate'
	  WHEN 524288 THEN 'Code'
	  WHEN 1048576 THEN 'Choice'
	  WHEN 4194304 THEN 'Wait'
	  WHEN 16777216 THEN 'Alert'
	  WHEN 33554432 THEN 'Exception'
	  WHEN 67108864 THEN 'Recover'
	  WHEN 134217728 THEN 'Resume'
	  WHEN 536870912 THEN 'Multicalculation'
	  WHEN 1073741824 THEN 'Skill'
	  ELSE 'Unknown' END as StageType
      ,ISNULL(processname, '') as 'Process'
      ,ISNULL(pagename, '') as 'Page'
      ,ISNULL(objectname, '') as 'Object'
      ,ISNULL(actionname, '') as 'Action'
      ,ISNULL(result, '') as Result
      ,startdatetime as ResourceStart
      ,enddatetime as ResourceEnd
      ,ISNULL(attributexml, '') as 'Parameters'
  FROM dbo.BPASessionLog_NonUnicode
GO

/*Create user in BP DB for a corresponding login*/
CREATE USER bpapiuser FOR LOGIN bpapiuser
GO

/*Grant readonly rights for 6 cstm_* views to created user*/
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

GRANT SELECT ON cstm_BPVLogs TO bpapiuser
GO