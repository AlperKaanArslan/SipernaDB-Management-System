/* =============================================================================
PROGRAMMABILITY: Stored Procedures for Business Operations
=============================================================================
*/

-- 1. Adds a new insured individual (owner or dependent) to the database[cite: 156].
CREATE PROCEDURE sp_AddCustomer
@Type CHAR(1), @name nvarchar(50), @surname nvarchar(50), @gender char(1), @birthdate date
AS
BEGIN
    INSERT INTO Customer(CustomerType, Name, Surname, Gender, BirthDate)
    VALUES (@Type, @name, @surname, @gender, @birthdate);
END;
GO

-- 2. Automatically extends a specific policy's end date by exactly one year[cite: 163].
CREATE PROCEDURE sp_RenewPolicy @PolicyID INT
AS
BEGIN
    UPDATE Policy SET EndDate = DATEADD(YEAR, 1, EndDate) WHERE PolicyID = @PolicyID;
END;
GO

-- 3. Manages provider participation by toggling active/inactive contract status[cite: 170].
CREATE PROCEDURE sp_ToggleProviderContract @ProviderID int, @status bit
AS
BEGIN
    UPDATE Provider
    SET ContractStatus = @status WHERE ProviderID = @ProviderID;
END;
GO

-- 4. Creates a new claim record linked to a policy, customer, and payment deadline[cite: 178].
CREATE PROCEDURE sp_CreateClaim @PolicyID INT, @CustomerID INT, @DueDate DATE
AS
BEGIN
    INSERT INTO Claim (PolicyID, CustomerID, PayDueDate) VALUES (@PolicyID, @CustomerID, @DueDate);
END;
GO

-- 5. Transfers a doctor to a new medical institution (Provider) within the network[cite: 185].
CREATE PROCEDURE sp_ReassignDoctor @DoctorID INT, @NewProviderID INT
AS
BEGIN
    UPDATE Doctor SET CurrentProviderID = @NewProviderID WHERE DoctorID = @DoctorID;
END;
GO

-- 6. Records a financial transaction and sequence number for a specific claim[cite: 192].
CREATE PROCEDURE sp_MakePayment @ClaimID INT, @SeqNum INT, @Amount DECIMAL(10,2)
AS
BEGIN
    INSERT INTO ClaimPayment (ClaimID, PaymentSequenceNumber, PaymentAmount) 
    VALUES (@ClaimID, @SeqNum, @Amount);
END;
GO

-- 7. Retrieval procedure to list all insurance policies managed by a specific agency[cite: 199].
CREATE PROCEDURE sp_GetAgencyPolicies @AgencyID INT
AS
BEGIN
    SELECT * FROM Policy WHERE AgencyID = @AgencyID;
END;
GO

-- 8. Updates a specific contact number for a customer to ensure data accuracy[cite: 207].
CREATE PROCEDURE sp_UpdateCustomerPhone @CustomerID INT, @OldPhone NVARCHAR(15), @NewPhone NVARCHAR(15)
AS
BEGIN
    UPDATE CustomerPhones SET PhoneNumber = @NewPhone 
    WHERE CustomerID = @CustomerID AND PhoneNumber = @OldPhone;
END;
GO

