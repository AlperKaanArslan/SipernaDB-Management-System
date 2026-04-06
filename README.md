🏥 Siperna: Enterprise Health Insurance Management System
Siperna is a comprehensive, full-stack web application and relational database system designed to automate the core operations of a Health Insurance Provider. This project demonstrates the integration of complex business logic with a robust SQL backend and a responsive ASP.NET frontend.

💼 Business Domain & Features
This system is built to manage the entire lifecycle of insurance products, ensuring data integrity and operational efficiency.

Customer Relationship Management (CRM): Complete management of policyholder profiles, including demographic data and coverage history.

Policy Administration: Dynamic creation and management of various insurance policies (Individual, Family, Corporate).

Agency & Broker Management: Tracking sales performance and hierarchical structures of insurance agencies.

Financial Transactions: Automated billing and payment tracking systems to ensure financial consistency.

Claims & Coverage: Logic-driven modules to handle outpatient/inpatient policy benefits and limitations.

Full CRUD Operations: Specialized interfaces for authorized users to Create, Read, Update, and Delete records across all insurance modules with real-time database synchronization.

🛠️ Technical Stack & Architecture
🗄️ Database Layer (SQL Server)
The heart of the project is a highly normalized relational database designed for high performance and data consistency.

Relational Design: 3rd Normal Form (3NF) compliant architecture (See ER_Diagram.png).

Advanced SQL Objects:

Stored Procedures: Optimized data retrieval and complex business transactions.

Triggers: Automated data validation and audit logging.

Views: Pre-computed reporting layers for business intelligence.

Indexes: Finely tuned for rapid query execution on large datasets.

🌐 Web Layer (ASP.NET Core / C#)
Architecture: Razor Pages with a clean separation of concerns.

Data Access: Secure database connectivity via SqlConnection and structured Connection Strings in appsettings.json.

Frontend: Responsive UI built with CSS/JS for an intuitive administrative experience.

📂 Repository Structure
/web: The core ASP.NET source code, including business logic and UI templates.

/sql: A structured deployment pipeline consisting of 6 sequential scripts:

01_Tables: Schema definition.

02-05: Optimization and logic (Indexes, Views, Procedures, Triggers).

06_Insertions: Comprehensive seed data for testing.

ER_Diagram.png: The visual blueprint of the insurance data model.

SipernaDB.bak: A production-ready database backup for instant environment restoration.

🚀 Quick Start & Deployment
Database Setup: Execute the SQL scripts in /sql chronologically OR restore the SipernaDB.bak file using SQL Server Management Studio (SSMS).

Configuration: Update the DefaultConnection string in web/appsettings.json with your server credentials.

Run: Open SipernaWeb.csproj in Visual Studio and press F5 to launch the local server.

Developer: Alper Kaan Arslan

Academic Background: Computer Engineering at Marmara University (International Exchange at Universidad de Málaga)
