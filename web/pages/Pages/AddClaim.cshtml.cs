using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class AddClaimModel : PageModel
    {
        public DataTable Customers { get; set; }
        public DataTable Policies { get; set; }

        public void OnGet()
        {
            DBConnection db = new DBConnection();
            string query = @"SELECT P.PolicyID, P.InsuranceOwnerID, (C.Name + ' ' + C.Surname) AS InsuredName 
                     FROM dbo.Policy P 
                     JOIN dbo.Customer C ON P.InsuranceOwnerID = C.CustomerID";
            Policies = db.getSelect(query).Tables[0];
        }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            string pId = Request.Form["PolicyID"];
            string cId = Request.Form["CustomerID"]; 
            string eDate = Request.Form["EventDate"];
            string pDueDate = string.IsNullOrEmpty(Request.Form["PayDueDate"]) ? "NULL" : $"'{Request.Form["PayDueDate"]}'";
            string sgkFlag = Request.Form["SGKStatusFlag"] == "on" ? "1" : "0";
            string reason = string.IsNullOrEmpty(Request.Form["SGKExclusionReason"]) ? "NULL" : $"'{Request.Form["SGKExclusionReason"]}'";

            string query = $@"INSERT INTO dbo.Claim (PolicyID, CustomerID, EventDate, PayDueDate, SGKStatusFlag, SGKExclusionReason) 
                     VALUES ({pId}, {cId}, '{eDate}', {pDueDate}, {sgkFlag}, {reason})";

            db.execute(query);
            return RedirectToPage("ClaimList");
        }
    }
}