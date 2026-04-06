using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace SipernaWeb.Pages
{
    public class RegisterProviderModel : PageModel
    {
        [BindProperty] public string InstitutionName { get; set; }
        [BindProperty] public string Address { get; set; }
        [BindProperty] public string ProviderType { get; set; }
        [BindProperty] public bool ContractStatus { get; set; }
        [BindProperty] public List<string> Emails { get; set; } = new List<string>();
        [BindProperty] public List<string> Phones { get; set; } = new List<string>();
        [BindProperty] public int? BedCapacity { get; set; }
        [BindProperty] public decimal? DiscountRate { get; set; }
        [BindProperty] public string SpecializationArea { get; set; }
        [BindProperty] public int? ResponseTimeHours { get; set; }
        [BindProperty] public bool HasHomeSampling { get; set; }
        [BindProperty] public string PharmacistFullName { get; set; }

        public void OnGet() { }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            int status = ContractStatus ? 1 : 0;

            string baseQuery = $@"
                INSERT INTO dbo.Provider (InstitutionName, ProviderType, Address, ContractStatus, InsertDate) 
                VALUES ('{InstitutionName.Replace("'", "''")}', '{ProviderType}', '{Address.Replace("'", "''")}', {status}, GETDATE()); 
                SELECT SCOPE_IDENTITY();";

            DataSet ds = db.getSelect(baseQuery);
            int newId = Convert.ToInt32(ds.Tables[0].Rows[0][0]);

            if (Emails != null)
            {
                foreach (var email in Emails.Where(e => !string.IsNullOrWhiteSpace(e)))
                    db.getSelect($"INSERT INTO dbo.ProviderEmails (ProviderID, EmailAddress) VALUES ({newId}, '{email}')");
            }
            if (Phones != null)
            {
                foreach (var phone in Phones.Where(p => !string.IsNullOrWhiteSpace(p)))
                    db.getSelect($"INSERT INTO dbo.ProviderPhones (ProviderID, GsmNumber) VALUES ({newId}, '{phone}')");
            }

            string subQuery = "";
            switch (ProviderType)
            {
                case "H": subQuery = $"INSERT INTO dbo.Hospital (HospitalID, BedCapacity, DiscountRate) VALUES ({newId}, {BedCapacity ?? 0}, {(DiscountRate ?? 0).ToString(System.Globalization.CultureInfo.InvariantCulture)})"; break;
                case "C": subQuery = $"INSERT INTO dbo.CLINIC (ClinicID, SpecializationArea) VALUES ({newId}, '{SpecializationArea}')"; break;
                case "L": subQuery = $"INSERT INTO dbo.LABAROTORY (LabarotoryID, ResponseTimeHours, HasHomeSampling) VALUES ({newId}, {ResponseTimeHours ?? 0}, {(HasHomeSampling ? 1 : 0)})"; break;
                case "P": subQuery = $"INSERT INTO dbo.Pharmacy (PharmacyID, PharmacistFullName) VALUES ({newId}, '{PharmacistFullName}')"; break;
            }
            db.getSelect(subQuery);

            return RedirectToPage("ProviderListModel", new { category = ProviderType });
        }
    }
}