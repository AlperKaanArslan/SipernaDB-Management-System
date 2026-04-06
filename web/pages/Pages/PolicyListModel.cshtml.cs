using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class PolicyListModel : PageModel
    {
        public DataTable PolicyData { get; set; }

        public void OnGet()
        {
            DBConnection db = new DBConnection();

            string query = @"
                SELECT P.*, 
                       (C.Name + ' ' + C.Surname) AS InsuredName,
                       (SELECT COUNT(*) FROM dbo.PolicyBeneficiaries WHERE PolicyID = P.PolicyID) AS BeneficiaryCount,
                       (SELECT COUNT(*) FROM dbo.PolicyNetwork WHERE PolicyID = P.PolicyID) AS NetworkCount
                FROM dbo.Policy P 
                JOIN dbo.Customer C ON P.InsuranceOwnerID = C.CustomerID 
                ORDER BY P.PolicyID ASC";

            DataSet ds = db.getSelect(query);
            PolicyData = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();
        }

        public IActionResult OnPostDelete(int id)
        {
            DBConnection db = new DBConnection();

            db.getSelect($"DELETE FROM dbo.PolicyBeneficiaries WHERE PolicyID = {id}");
            db.getSelect($"DELETE FROM dbo.PolicyNetwork WHERE PolicyID = {id}");
            db.getSelect($"DELETE FROM dbo.OutPatientPolicy WHERE PatientID = {id}");
            db.getSelect($"DELETE FROM dbo.Policy WHERE PolicyID = {id}");

            return RedirectToPage();
        }
    }
}