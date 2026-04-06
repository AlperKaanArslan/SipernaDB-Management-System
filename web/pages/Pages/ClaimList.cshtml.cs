using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class ClaimListModel : PageModel
    {
        public DataTable ClaimData { get; set; }

        public void OnGet()
        {
            DBConnection db = new DBConnection();
            // Sorguya ihtiyacýmýz olan TÜM sütunlarý ekledik
            string query = @"SELECT CL.ClaimID, CL.EventDate, CL.PolicyID, CL.SGKStatusFlag, CL.PayDueDate,
                   (C.Name + ' ' + C.Surname) AS InsuredName,
                   (SELECT SUM(PaymentAmount) FROM dbo.ClaimPayment WHERE ClaimID = CL.ClaimID) AS TotalPaid
                   FROM dbo.Claim CL
                   JOIN dbo.Customer C ON CL.CustomerID = C.CustomerID
                   ORDER BY CL.EventDate DESC";
            ClaimData = db.getSelect(query).Tables[0];
        }

        public IActionResult OnPostDelete(int id)
        {
            DBConnection db = new DBConnection();
            db.execute($"DELETE FROM dbo.ClaimPayment WHERE ClaimID = {id}");
            db.execute($"DELETE FROM dbo.Claim WHERE ClaimID = {id}");
            return RedirectToPage();
        }
    }
}