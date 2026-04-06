using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class EditPolicyModel : PageModel
    {
        [BindProperty] public int PolicyID { get; set; }
        [BindProperty] public int AgencyID { get; set; }
        [BindProperty] public int InsuranceOwnerID { get; set; }
        [BindProperty] public string PolicyType { get; set; }
        [BindProperty] public DateTime StartDate { get; set; }
        [BindProperty] public DateTime EndDate { get; set; }
        [BindProperty] public string RenewalInfo { get; set; }
        [BindProperty] public string PaymentFrequency { get; set; }

        public void OnGet(int id)
        {
            DBConnection db = new DBConnection();
            DataTable dt = db.getSelect($"SELECT * FROM dbo.Policy WHERE PolicyID = {id}").Tables[0];

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                PolicyID = (int)row["PolicyID"];
                AgencyID = (int)row["AgencyID"];
                InsuranceOwnerID = (int)row["InsuranceOwnerID"];
                PolicyType = row["PolicyType"].ToString();
                StartDate = Convert.ToDateTime(row["StartDate"]);
                EndDate = Convert.ToDateTime(row["EndDate"]);
                RenewalInfo = row["RenewalInfo"]?.ToString();
                PaymentFrequency = row["PaymentFrequency"].ToString();
            }
        }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            string query = $@"
                UPDATE dbo.Policy SET 
                AgencyID = {AgencyID}, 
                InsuranceOwnerID = {InsuranceOwnerID}, 
                PolicyType = '{PolicyType}', 
                StartDate = '{StartDate:yyyy-MM-dd}', 
                EndDate = '{EndDate:yyyy-MM-dd}', 
                RenewalInfo = '{RenewalInfo?.Replace("'", "''")}', 
                PaymentFrequency = '{PaymentFrequency}' 
                WHERE PolicyID = {PolicyID}";

            db.getSelect(query);
            return RedirectToPage("PolicyListModel");
        }
    }
}