using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class ClaimDetailsModel : PageModel
    {
        public DataRow ClaimInfo { get; set; }
        public DataTable Payments { get; set; }

        public void OnGet(int id)
        {
            DBConnection db = new DBConnection();

            string q1 = $@"SELECT CL.ClaimID, CL.EventDate, CL.SGKStatusFlag, CL.SGKExclusionReason, 
                           (C.Name + ' ' + C.Surname) AS InsuredName 
                           FROM dbo.Claim CL JOIN dbo.Customer C ON CL.CustomerID = C.CustomerID 
                           WHERE CL.ClaimID = {id}";

            var dt = db.getSelect(q1).Tables[0];
            if (dt.Rows.Count > 0)
                ClaimInfo = dt.Rows[0];

            string q2 = $"SELECT * FROM dbo.ClaimPayment WHERE ClaimID = {id} ORDER BY PaymentSequenceNumber ASC";
            Payments = db.getSelect(q2).Tables[0];
        }

        // SGK Exclusion Reason Güncelleme Handler'ý
        public IActionResult OnPostUpdateExclusion(int id)
        {
            DBConnection db = new DBConnection();
            string reason = Request.Form["ExclusionReason"];

            string safeReason = reason?.Replace("'", "''");

            string query = $@"UPDATE dbo.Claim 
                             SET SGKExclusionReason = '{safeReason}' 
                             WHERE ClaimID = {id}";

            db.execute(query);

            return RedirectToPage(new { id = id });
        }

        public IActionResult OnPostAddPayment(int id)
        {
            DBConnection db = new DBConnection();
            string amount = Request.Form["PaymentAmount"];
            string pDate = Request.Form["PaymentDate"];

            string seqQuery = $"SELECT ISNULL(MAX(PaymentSequenceNumber), 0) + 1 FROM dbo.ClaimPayment WHERE ClaimID = {id}";
            int nextSeq = Convert.ToInt32(db.getSelect(seqQuery).Tables[0].Rows[0][0]);

            string query = $@"INSERT INTO dbo.ClaimPayment (ClaimID, PaymentSequenceNumber, PaymentAmount, PaymentDate) 
                             VALUES ({id}, {nextSeq}, {amount.Replace(",", ".")}, '{pDate}')";
            db.execute(query);

            return RedirectToPage(new { id = id });
        }
    }
}