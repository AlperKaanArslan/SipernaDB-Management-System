using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SipernaWeb.Pages
{
    public class AddAgencyModel : PageModel
    {
        public void OnGet() { }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            string name = Request.Form["AgencyName"];
            string tax = Request.Form["TaxNumber"];
            string phone = Request.Form["PhoneNumber"];
            string email = Request.Form["Email"];
            string start = Request.Form["StartDate"];
            string end = string.IsNullOrEmpty(Request.Form["EndDate"]) ? "NULL" : $"'{Request.Form["EndDate"]}'";

            string query = $@"INSERT INTO dbo.Agency (AgencyName, StartDate, EndDate, Email, TaxNumber, PhoneNumber) 
                             VALUES ('{name}', '{start}', {end}, '{email}', '{tax}', '{phone}')";

            db.execute(query);
            return RedirectToPage("AgencyList");
        }
    }
}