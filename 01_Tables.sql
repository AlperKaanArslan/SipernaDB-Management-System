/* =============================================================================
DATABASE SETUP: Siperna Health Insurance Management System
=============================================================================
*/

CREATE DATABASE SipernaDB;
GO

USE SipernaDB;
GO

-- 1. ADMINISTRATIVE DATA: Managing Insurance Agencies
CREATE TABLE Agency( 
    AgencyID INT IDENTITY(1,1) PRIMARY KEY,
    AgencyName NVARCHAR(100) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    Email NVARCHAR(100),
    TaxNumber CHAR(10) NOT NULL, -- Legal unique identifier for tax purposes
    PhoneNumber NVARCHAR(20),

    CONSTRAINT uq_agency_taxnum UNIQUE(TaxNumber)
);
GO

-- 2. PROVIDER NETWORK: Supertype Table for Hospitals, Clinics, Labs, and Pharmacies
CREATE TABLE Provider (
    ProviderID INT IDENTITY(1,1) PRIMARY KEY,
    InstitutionName NVARCHAR(150) NOT NULL,
    ProviderType CHAR(1) NOT NULL, -- H: Hospital, C: Clinic, L: Lab, P: Pharmacy
    Address NVARCHAR(250),
    ContractStatus BIT DEFAULT 1, -- 1: Active, 0: Terminated
    InsertDate DATETIME DEFAULT GETDATE()
);
GO

-- Handling multi-valued attributes for Providers (Emails and Phones)
CREATE TABLE ProviderEmails (
    ProviderID INT NOT NULL,
    EmailAddress NVARCHAR(100) NOT NULL,
    
    CONSTRAINT PK_ProviderEmails PRIMARY KEY (ProviderID, EmailAddress),
    CONSTRAINT FK_Emails_Provider FOREIGN KEY (ProviderID) REFERENCES Provider(ProviderID)
);
GO

CREATE TABLE ProviderPhones (
    ProviderID INT NOT NULL,
    GsmNumber NVARCHAR(15) NOT NULL,
    
    CONSTRAINT PK_ProviderPhones PRIMARY KEY (ProviderID, GsmNumber),
    CONSTRAINT FK_Phones_Provider FOREIGN KEY (ProviderID) REFERENCES Provider(ProviderID)
);
GO

-- 3. CUSTOMER MANAGEMENT: Base table for all insured individuals
CREATE TABLE Customer (
    CustomerID INT IDENTITY(1,1) PRIMARY KEY,
    CustomerType CHAR(1) NOT NULL CHECK (CustomerType IN ('O', 'F')), -- O: Owner, F: Family Member
    Name NVARCHAR(50) NOT NULL,
    Surname NVARCHAR(50) NOT NULL,
    Gender CHAR(1) CHECK (Gender IN ('M', 'F')),
    BirthDate DATE NOT NULL,
    Age AS DATEDIFF(YEAR, BirthDate, GETDATE()) -- Dynamically calculated age
);
GO

CREATE TABLE CustomerPhones (
    CustomerID INT NOT NULL,
    PhoneNumber NVARCHAR(15) NOT NULL,

    CONSTRAINT PK_CustomerPhones PRIMARY KEY (CustomerID, PhoneNumber),
    CONSTRAINT FK_Phone_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
GO

-- 4. PROVIDER ISA HIERARCHY: Specific attributes for each provider type
CREATE TABLE HOSPITAL(
    HospitalID INT PRIMARY KEY,
    BedCapacity INT CHECK (BedCapacity >= 0),
    DiscountRate DECIMAL(5,2) DEFAULT 0.00,
    CONSTRAINT FK_Hospital_Provider FOREIGN KEY (HospitalID) REFERENCES Provider(ProviderID)
);
GO

CREATE TABLE CLINIC(
    ClinicID INT PRIMARY KEY,
    SpecializationArea NVARCHAR(100),
    CONSTRAINT FK_Clinic_Provider FOREIGN KEY (ClinicID) References Provider(ProviderID)
);
GO

CREATE TABLE LABAROTORY(
    LabarotoryID INT PRIMARY KEY,
    HasHomeSampling BIT DEFAULT 0, -- Option for lab tests at home
    ResponseTimeHours INT,

    CONSTRAINT FK_Labarotory_Provider FOREIGN KEY (LabarotoryID) REFERENCES Provider(ProviderID)
);
GO

CREATE TABLE Pharmacy (
    PharmacyID INT PRIMARY KEY, 
    PharmacistFullName NVARCHAR(100), 
    CONSTRAINT FK_Pharmacy_Provider FOREIGN KEY (PharmacyID) REFERENCES Provider(ProviderID)
);
GO

CREATE TABLE PharmacyDrugs (
    PharmacyID INT NOT NULL,
    DrugName NVARCHAR(100) NOT NULL,
   
    CONSTRAINT PK_PharmacyDrugs PRIMARY KEY (PharmacyID, DrugName),
    CONSTRAINT FK_Drugs_Pharmacy FOREIGN KEY (PharmacyID) REFERENCES Pharmacy(PharmacyID)
);
GO

-- 5. MEDICAL STAFF: Doctor management and specialties
CREATE TABLE dbo.Doctor (
    DoctorID int IDENTITY(1,1) NOT NULL, 
    SSN char(11) NOT NULL,               
    DoctorName nvarchar(100) NOT NULL,
    BirthDate date NULL,
    CurrentProviderID int NULL,

    CONSTRAINT PK_Doctor_ID PRIMARY KEY NONCLUSTERED (DoctorID)
);
GO
CREATE UNIQUE CLUSTERED INDEX IX_Doctor_SSN_Clustered ON dbo.Doctor (SSN);
GO

CREATE TABLE DoctorSpecialties (
    DoctorID INT NOT NULL,
    SpecialtyName NVARCHAR(50) NOT NULL,

    CONSTRAINT PK_DoctorSpecialties PRIMARY KEY (DoctorID, SpecialtyName),
    CONSTRAINT FK_Specialty_Doctor FOREIGN KEY (DoctorID) REFERENCES Doctor(DoctorID)
);
GO

-- 6. MEMBERSHIP ROLES: Specialized Customer Subtypes (Owner vs. Dependent)
CREATE TABLE InsuranceOwner(
    InsuranceOwnerID int primary key, -- References CustomerID
    Address nvarchar(200),
    Email NVARCHAR(100),

    CONSTRAINT FK_Owner_Customer FOREIGN KEY (InsuranceOwnerID) references CUSTOMER(CustomerID)
);
GO

CREATE TABLE FamilyMember(
    FamilyMemberID INT PRIMARY KEY, 
    RelationType NVARCHAR(20) NOT NULL, -- e.g., Spouse, Child
    MaritalStatus NVARCHAR(20),

    CONSTRAINT FK_Family_Customer FOREIGN KEY (FamilyMemberID) REFERENCES Customer(CustomerID),
);
GO

-- 7. POLICY MANAGEMENT: Insurance contracts and network coverage
CREATE TABLE Policy (
    PolicyID INT IDENTITY(1,1) PRIMARY KEY,
    AgencyID INT NOT NULL,
    InsuranceOwnerID INT NOT NULL,
    PolicyType CHAR(1) NOT NULL CHECK (PolicyType IN ('I', 'O')), -- I: Individual, O: Other/Group
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    RenewalInfo NVARCHAR(MAX),
    PaymentFrequency NVARCHAR(20),
    
    CONSTRAINT FK_Policy_Agency FOREIGN KEY (AgencyID) REFERENCES Agency(AgencyID),
    CONSTRAINT FK_InsuranceOwner_Policy FOREIGN KEY (InsuranceOwnerID) REFERENCES InsuranceOwner(InsuranceOwnerID),
    CONSTRAINT CK_PolicyDate CHECK (EndDate > StartDate) -- Ensures valid date range
);
GO

-- Junction tables for Many-to-Many relationships
CREATE TABLE PolicyNetwork (
    PolicyID INT NOT NULL,
    ProviderID INT NOT NULL,
    
    CONSTRAINT PK_PolicyNetwork PRIMARY KEY (PolicyID, ProviderID),
    CONSTRAINT FK_PN_Policy FOREIGN KEY (PolicyID) REFERENCES Policy(PolicyID),
    CONSTRAINT FK_PN_Provider FOREIGN KEY (ProviderID) REFERENCES Provider(ProviderID)
);
GO

CREATE TABLE PolicyBeneficiaries (
    PolicyID INT NOT NULL,
    FamilyMemberID INT NOT NULL,
    
    CONSTRAINT PK_PolicyBeneficiaries PRIMARY KEY (PolicyID, FamilyMemberID),
    CONSTRAINT FK_PB_Policy FOREIGN KEY (PolicyID) REFERENCES Policy(PolicyID),
    CONSTRAINT FK_PB_Family FOREIGN KEY (FamilyMemberID) REFERENCES FamilyMember(FamilyMemberID)
);
GO

-- 8. OPERATIONAL LOGS: Treatment and Pharmacy transaction history
CREATE TABLE DoctorPatients (
    DoctorID INT NOT NULL,
    CustomerID INT NOT NULL,
    TreatmentDate DATE DEFAULT GETDATE(),
    
    CONSTRAINT PK_DoctorPatients PRIMARY KEY (DoctorID, CustomerID, TreatmentDate),
    CONSTRAINT FK_DP_Doctor FOREIGN KEY (DoctorID) REFERENCES Doctor(DoctorID),
    CONSTRAINT FK_DP_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
GO

CREATE TABLE PharmacyCustomers (
    PharmacyID INT NOT NULL,
    CustomerID INT NOT NULL,
    RegistrationDate DATE DEFAULT GETDATE(),
    
    CONSTRAINT PK_PharmacyCustomers PRIMARY KEY (PharmacyID, CustomerID),
    CONSTRAINT FK_PC_Pharmacy FOREIGN KEY (PharmacyID) REFERENCES Pharmacy(PharmacyID),
    CONSTRAINT FK_PC_Customer FOREIGN KEY (CustomerID) REFERENCES Customer(CustomerID)
);
GO

-- 9. POLICY ISA HIERARCHY: Detailed coverage rules for In/Out Patient plans
CREATE TABLE OutPatientPolicy(
    PatientID INT Primary key references Policy(PolicyID),
    AnnualDoctorVisit INT,
    DrugCostLimit decimal(10,2) default 0.00
);
GO

CREATE TABLE InPatientPolicy(
    PatientID INT Primary Key references Policy(PolicyID),
    IcuMaxDays INT,
    PreApprovalRequired CHAR(1) default 1
);
GO

-- 10. CLAIMS AND PAYMENTS: Managing medical expenses and financial processing
CREATE TABLE Claim(
    ClaimID INT IDENTITY(1,1) PRIMARY KEY,
    PolicyID INT NOT NULL References Policy(PolicyID),
    CustomerID INT NOT NULL References Customer(CustomerID),
    EventDate DATE NOT NULL DEFAULT GETDATE(),
    PayDueDate DATE,
    SGKStatusFlag BIT DEFAULT 0, -- Social Security integration flag
    SGKExclusionReason NVARCHAR(250)
);
GO

CREATE TABLE ClaimPayment(
    ClaimID int not null,
    PaymentSequenceNumber INT NOT NULL,
    PaymentAmount DECIMAL(10,2) NOT NULL,
    PaymentDate DATE DEFAULT GETDATE(),

    CONSTRAINT PK_ClaimPayment PRIMARY KEY (ClaimID, PaymentSequenceNumber),
    CONSTRAINT FK_Payment_Claim FOREIGN KEY (ClaimID) REFERENCES Claim(ClaimID)
);
GO
