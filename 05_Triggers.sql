/* =============================================================================
DATA SECURITY & AUDITING: Database Triggers and Logs
=============================================================================
*/

-- 1. Data Protection Trigger: Prevents any manual deletion of policy records.
-- Business Logic: Ensures a permanent record of all insurance contracts for auditing purposes.
CREATE TRIGGER trg_PreventPolicyDelete
ON Policy
FOR DELETE
AS
BEGIN
    PRINT 'Policies in the system cannot be deleted to maintain historical integrity.';
    ROLLBACK TRANSACTION; -- Cancels the delete operation automatically
END;
GO

-- 2. Audit Infrastructure: Table to store a history of modifications.
CREATE TABLE TableAuditLog (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    TableName NVARCHAR(50),
    Operation NVARCHAR(10), -- Stores type of change: INSERT, UPDATE, or DELETE
    LogDate DATETIME DEFAULT GETDATE(), -- Captures exact time of operation
    LogUser NVARCHAR(100) DEFAULT CURRENT_USER -- Captures which database user made the change
);
GO

-- 3. Operational Audit Trigger: Tracks lifecycle changes for the Customer table.
-- Logic: Automatically identifies the DML operation type and records it in TableAuditLog.
CREATE TRIGGER trg_CustomerAudit
ON Customer
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    DECLARE @operation NVARCHAR(10);

    -- Determining the operation type based on inserted/deleted virtual tables
    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
        SET @operation = 'UPDATE';
    ELSE IF EXISTS (SELECT * FROM inserted)
        SET @operation = 'INSERT';
    ELSE IF EXISTS (SELECT * FROM deleted)
        SET @operation = 'DELETE';

    -- Logging the event for security monitoring
    INSERT INTO TableAuditLog (TableName, Operation)
    VALUES ('Customer', @operation);
END;
GO