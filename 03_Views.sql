/* =============================================================================
REPORTING & ANALYTICS: Database Views for Business Intelligence
=============================================================================
*/

-- 1. Displays all currently valid insurance policies by joining Policy, Owner, and Agency data.
-- Business Logic: Filters records where the EndDate is in the future relative to the current system date.
CREATE VIEW View_ActivePolicies AS
SELECT p.PolicyID, p.StartDate, p.EndDate, c.Name, c.Surname, agn.AgencyName
FROM Policy p
    INNER JOIN InsuranceOwner io ON p.InsuranceOwnerID = io.InsuranceOwnerID
    INNER JOIN Customer c ON p.InsuranceOwnerID = c.CustomerID
    INNER JOIN Agency agn ON p.AgencyID = agn.AgencyID 
WHERE p.EndDate > GETDATE();
GO

-- 2. Provides a directory of doctors and their associated medical institutions.
-- Business Logic: Links physicians to the Provider table to show workplace type and location.
CREATE VIEW View_Doctor_Workplaces AS
SELECT d.DoctorName, p.ProviderType, p.InstitutionName, p.Address
FROM Doctor d
JOIN Provider p ON d.CurrentProviderID = p.ProviderID;
GO

-- 3. Generates statistical insights regarding medical staff distribution across the network.
-- Business Logic: Aggregates total doctor counts per provider (Hospital, Clinic, or Lab) using a LEFT JOIN.
CREATE OR ALTER VIEW View_ProviderStats AS
SELECT 
    p.InstitutionName, 
    p.ProviderType,
    COUNT(d.DoctorID) AS [Total Doctors]
FROM Provider p
    LEFT JOIN Doctor d ON p.ProviderID = d.CurrentProviderID
WHERE p.ProviderType IN ('H', 'C', 'L') -- Excludes Pharmacies from medical staffing counts
GROUP BY p.InstitutionName, p.ProviderType;
GO

-- 4. Summarizes claim history and total financial payouts per customer.
-- Business Logic: Aggregates payments from ClaimPayment and handles zero-payment cases using ISNULL.
CREATE VIEW View_PendingClaims AS
SELECT cl.ClaimID, c.Name, c.Surname, cl.EventDate, ISNULL(SUM(cp.PaymentAmount),0) AS [Total Paid]
FROM Claim cl
JOIN Customer c ON cl.CustomerID = c.CustomerID
LEFT JOIN ClaimPayment cp ON cl.ClaimID = cp.ClaimID
GROUP BY cl.ClaimID, c.Name, c.Surname, cl.EventDate;
GO