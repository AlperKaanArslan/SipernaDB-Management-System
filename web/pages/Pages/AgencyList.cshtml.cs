using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class AgencyListModel : PageModel
    {
        public DataTable AgencyData { get; set; }

        public void OnGet()
        {
            DBConnection db = new DBConnection();
            string query = "SELECT * FROM dbo.Agency ORDER BY AgencyName";
            AgencyData = db.getSelect(query).Tables[0];
        }

        public IActionResult OnPostDelete(int id)
        {
            DBConnection db = new DBConnection();
            // Silme iþlemi
            string query = $"DELETE FROM dbo.Agency WHERE AgencyID = {id}";
            db.execute(query);
            return RedirectToPage();
        }
    }
}