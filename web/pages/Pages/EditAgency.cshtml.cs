using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class EditAgencyModel : PageModel
    {
        public DataRow AgencyRow { get; set; }

        public void OnGet(int id)
        {
            DBConnection db = new DBConnection();
            string query = $"SELECT * FROM dbo.Agency WHERE AgencyID = {id}";
            AgencyData = db.getSelect(query).Tables[0];
            if (AgencyData.Rows.Count > 0) AgencyRow = AgencyData.Rows[0];
        }

        public IActionResult OnPost(int id)
        {
            DBConnection db = new DBConnection();
            string name = Request.Form["AgencyName"];
            string tax = Request.Form["TaxNumber"];
            string phone = Request.Form["PhoneNumber"];
            string email = Request.Form["Email"];
            string start = Request.Form["StartDate"];
            string end = string.IsNullOrEmpty(Request.Form["EndDate"]) ? "NULL" : $"'{Request.Form["EndDate"]}'";

            string query = $@"UPDATE dbo.Agency SET 
                            AgencyName = '{name}', TaxNumber = '{tax}', PhoneNumber = '{phone}', 
                            Email = '{email}', StartDate = '{start}', EndDate = {end}
                            WHERE AgencyID = {id}";

            db.execute(query);
            return RedirectToPage("AgencyList");
        }

        private DataTable AgencyData;
    }
}