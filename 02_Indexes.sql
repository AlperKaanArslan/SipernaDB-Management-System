/* =============================================================================
PERFORMANCE OPTIMIZATION: Database Indexes
=============================================================================
*/

-- 1. DOCTOR TABLE: Clustered Index on SSN
-- Purpose: Physically sorts the data rows based on SSN.
-- Use Case: Optimizes high-frequency lookups using the primary national identifier.
CREATE INDEX IX_DOCTOR_SSN_CLUSTERED on Doctor(Ssn);
GO

-- 2. CUSTOMER TABLE: Non-Clustered Composite Index on Name & Surname
-- Purpose: Speeds up administrative searches by full name.
-- Use Case: Efficiently handles "Index Seeks" for finding specific policyholders.
CREATE INDEX idx_Customer_Name_Surname ON Customer(Name, Surname);
GO

-- 3. POLICY TABLE: Non-Clustered Index on StartDate
-- Purpose: Optimizes range queries and sorting for policy commencement.
-- Use Case: Essential for generating monthly/annual reports on new registrations.
CREATE INDEX idx_Policy_StartDate ON Policy(StartDate);
GO