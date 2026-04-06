using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace SipernaWeb.Pages
{
    public class EditProviderModel : PageModel
    {
        [BindProperty] public int ProviderID { get; set; }
        [BindProperty] public string InstitutionName { get; set; }
        [BindProperty] public string Address { get; set; }
        [BindProperty] public bool ContractStatus { get; set; }
        [BindProperty] public string ProviderType { get; set; }

        // Listeler
        [BindProperty] public List<string> Emails { get; set; } = new List<string>();
        [BindProperty] public List<string> Phones { get; set; } = new List<string>();

        // Kategori Özellikleri
        [BindProperty] public int? BedCapacity { get; set; }
        [BindProperty] public decimal? DiscountRate { get; set; }
        [BindProperty] public string SpecializationArea { get; set; }
        [BindProperty] public int? ResponseTimeHours { get; set; }
        [BindProperty] public bool HasHomeSampling { get; set; }
        [BindProperty] public string PharmacistFullName { get; set; }

        public void OnGet(int id, string category)
        {
            DBConnection db = new DBConnection();
            ProviderType = category;
            string query = "";

            switch (category)
            {
                case "H": query = $"SELECT P.*, H.BedCapacity, H.DiscountRate FROM dbo.Provider P JOIN dbo.Hospital H ON P.ProviderID = H.HospitalID WHERE P.ProviderID = {id}"; break;
                case "C": query = $"SELECT P.*, C.SpecializationArea FROM dbo.Provider P JOIN dbo.CLINIC C ON P.ProviderID = C.ClinicID WHERE P.ProviderID = {id}"; break;
                case "L": query = $"SELECT P.*, L.ResponseTimeHours, L.HasHomeSampling FROM dbo.Provider P JOIN dbo.LABAROTORY L ON P.ProviderID = L.LabarotoryID WHERE P.ProviderID = {id}"; break;
                case "P": query = $"SELECT P.*, PH.PharmacistFullName FROM dbo.Provider P JOIN dbo.Pharmacy PH ON P.ProviderID = PH.PharmacyID WHERE P.ProviderID = {id}"; break;
            }

            DataTable dt = db.getSelect(query).Tables[0];
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                ProviderID = (int)row["ProviderID"];
                InstitutionName = row["InstitutionName"].ToString();
                Address = row["Address"].ToString();
                ContractStatus = Convert.ToBoolean(row["ContractStatus"]);

                if (category == "H") { BedCapacity = (int)row["BedCapacity"]; DiscountRate = (decimal)row["DiscountRate"]; }
                if (category == "C") { SpecializationArea = row["SpecializationArea"].ToString(); }
                if (category == "L") { ResponseTimeHours = (int)row["ResponseTimeHours"]; HasHomeSampling = Convert.ToBoolean(row["HasHomeSampling"]); }
                if (category == "P") { PharmacistFullName = row["PharmacistFullName"].ToString(); }
            }

            DataTable dtE = db.getSelect($"SELECT EmailAddress FROM dbo.ProviderEmails WHERE ProviderID = {id}").Tables[0];
            foreach (DataRow r in dtE.Rows) Emails.Add(r["EmailAddress"].ToString());

            DataTable dtP = db.getSelect($"SELECT GsmNumber FROM dbo.ProviderPhones WHERE ProviderID = {id}").Tables[0];
            foreach (DataRow r in dtP.Rows) Phones.Add(r["GsmNumber"].ToString());
        }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            int st = ContractStatus ? 1 : 0;


            db.getSelect($"UPDATE dbo.Provider SET InstitutionName = '{InstitutionName.Replace("'", "''")}', Address = '{Address.Replace("'", "''")}', ContractStatus = {st} WHERE ProviderID = {ProviderID}");
            db.getSelect($"DELETE FROM dbo.ProviderEmails WHERE ProviderID = {ProviderID}");
            db.getSelect($"DELETE FROM dbo.ProviderPhones WHERE ProviderID = {ProviderID}");

            if (Emails != null) foreach (var e in Emails.Where(x => !string.IsNullOrEmpty(x))) db.getSelect($"INSERT INTO dbo.ProviderEmails VALUES ({ProviderID}, '{e}')");
            if (Phones != null) foreach (var p in Phones.Where(x => !string.IsNullOrEmpty(x))) db.getSelect($"INSERT INTO dbo.ProviderPhones VALUES ({ProviderID}, '{p}')");

            // 3. Alt Tablo Güncelleme
            string subQ = "";
            switch (ProviderType)
            {
                case "H": subQ = $"UPDATE dbo.Hospital SET BedCapacity = {BedCapacity ?? 0}, DiscountRate = {(DiscountRate ?? 0).ToString(CultureInfo.InvariantCulture)} WHERE HospitalID = {ProviderID}"; break;
                case "C": subQ = $"UPDATE dbo.CLINIC SET SpecializationArea = '{SpecializationArea}' WHERE ClinicID = {ProviderID}"; break;
                case "L": subQ = $"UPDATE dbo.LABAROTORY SET ResponseTimeHours = {ResponseTimeHours ?? 0}, HasHomeSampling = {(HasHomeSampling ? 1 : 0)} WHERE LabarotoryID = {ProviderID}"; break;
                case "P": subQ = $"UPDATE dbo.Pharmacy SET PharmacistFullName = '{PharmacistFullName}' WHERE PharmacyID = {ProviderID}"; break;
            }
            db.getSelect(subQ);

            return RedirectToPage("ProviderListModel", new { category = ProviderType });
        }
    }
}