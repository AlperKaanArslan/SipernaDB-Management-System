using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace SipernaWeb.Pages
{
    public class CreatePolicyModel : PageModel
    {
        [BindProperty] public int InsuranceOwnerID { get; set; }
        [BindProperty] public int AgencyID { get; set; }
        [BindProperty] public string PolicyType { get; set; }
        [BindProperty] public DateTime StartDate { get; set; } = DateTime.Now;
        [BindProperty] public DateTime EndDate { get; set; } = DateTime.Now.AddYears(1);
        [BindProperty] public string RenewalInfo { get; set; }
        [BindProperty] public string PaymentFrequency { get; set; }

        public List<SelectListItem> OwnerList { get; set; } = new List<SelectListItem>();

        public void OnGet()
        {
            LoadOwners();
        }

        private void LoadOwners()
        {
            DBConnection db = new DBConnection();
            string query = "SELECT CustomerID, Name, Surname FROM dbo.Customer WHERE CustomerType = 'O'";
            DataSet ds = db.getSelect(query);

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    OwnerList.Add(new SelectListItem
                    {
                        Value = row["CustomerID"].ToString(),
                        Text = $"{row["CustomerID"]} - {row["Name"]} {row["Surname"]}"
                    });
                }
            }
        }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            string q = $@"INSERT INTO dbo.Policy (AgencyID, InsuranceOwnerID, PolicyType, StartDate, EndDate, RenewalInfo, PaymentFrequency) 
                          VALUES ({AgencyID}, {InsuranceOwnerID}, '{PolicyType}', '{StartDate:yyyy-MM-dd}', '{EndDate:yyyy-MM-dd}', 
                                  '{RenewalInfo?.Replace("'", "''")}', '{PaymentFrequency}')";

            db.getSelect(q);
            return RedirectToPage("PolicyListModel");
        }
    }
}