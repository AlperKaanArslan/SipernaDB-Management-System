using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;

namespace SipernaWeb.Pages
{
    public class ProviderListModel : PageModel
    {
        public DataTable ProviderData { get; set; }
        [BindProperty(SupportsGet = true)] public string Category { get; set; } = "H";

        public void OnGet()
        {
            DBConnection db = new DBConnection();

            string contactSubQueries = @"
                (SELECT STRING_AGG(EmailAddress, ', ') FROM dbo.ProviderEmails WHERE ProviderID = P.ProviderID) AS Emails,
                (SELECT STRING_AGG(GsmNumber, ', ') FROM dbo.ProviderPhones WHERE ProviderID = P.ProviderID) AS Phones";

            string query = "";
            switch (Category)
            {
                case "H":
                    query = $@"SELECT P.*, {contactSubQueries}, 'Hospital' AS TypeName, H.BedCapacity, H.DiscountRate 
                               FROM dbo.Provider P JOIN dbo.Hospital H ON P.ProviderID = H.HospitalID";
                    break;
                case "C":
                    query = $@"SELECT P.*, {contactSubQueries}, 'Clinic' AS TypeName, C.SpecializationArea 
                               FROM dbo.Provider P JOIN dbo.CLINIC C ON P.ProviderID = C.ClinicID";
                    break;
                case "L":
                    query = $@"SELECT P.*, {contactSubQueries}, 'Laboratory' AS TypeName, L.ResponseTimeHours, L.HasHomeSampling 
                               FROM dbo.Provider P JOIN dbo.LABAROTORY L ON P.ProviderID = L.LabarotoryID";
                    break;
                case "P":
                    query = $@"SELECT P.*, {contactSubQueries}, 'Pharmacy' AS TypeName, PH.PharmacistFullName 
                               FROM dbo.Provider P JOIN dbo.Pharmacy PH ON P.ProviderID = PH.PharmacyID";
                    break;
            }

            DataSet ds = db.getSelect(query);
            ProviderData = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
        }

        public IActionResult OnPostDelete(int id)
        {
            DBConnection db = new DBConnection();

            db.getSelect($"DELETE FROM dbo.ProviderEmails WHERE ProviderID = {id}");
            db.getSelect($"DELETE FROM dbo.ProviderPhones WHERE ProviderID = {id}");

            db.getSelect($"DELETE FROM dbo.Hospital WHERE HospitalID = {id}");
            db.getSelect($"DELETE FROM dbo.CLINIC WHERE ClinicID = {id}");
            db.getSelect($"DELETE FROM dbo.LABAROTORY WHERE LabarotoryID = {id}");
            db.getSelect($"DELETE FROM dbo.Pharmacy WHERE PharmacyID = {id}");

            db.getSelect($"DELETE FROM dbo.PolicyNetwork WHERE ProviderID = {id}");

            db.getSelect($"DELETE FROM dbo.Provider WHERE ProviderID = {id}");

            return RedirectToPage();
        }
    }
}