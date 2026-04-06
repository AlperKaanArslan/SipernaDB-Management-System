using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class CustomerListModel : PageModel
    {
        public DataTable CustomerData { get; set; } = new DataTable();

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = "O";

        public void OnGet()
        {
            DBConnection db = new DBConnection();
            string query = "";
            string selectedCategory = Category?.Trim().ToUpper() ?? "O";

            switch (selectedCategory)
            {
                case "O":
                    query = @"SELECT C.CustomerID, C.Name, C.Surname, C.Gender, C.Age, O.Address, O.Email 
                              FROM dbo.Customer C 
                              INNER JOIN dbo.InsuranceOwner O ON C.CustomerID = O.InsuranceOwnerID";
                    break;
                case "F":
                    query = @"SELECT C.CustomerID, C.Name, C.Surname, C.Gender, C.Age, F.RelationType, F.MaritalStatus 
                              FROM dbo.Customer C 
                              INNER JOIN dbo.FamilyMember F ON C.CustomerID = F.FamilyMemberID";
                    break;
                default:
                    query = @"SELECT C.CustomerID, C.Name, C.Surname, C.Gender, C.Age, O.Address, O.Email 
                              FROM dbo.Customer C 
                              INNER JOIN dbo.InsuranceOwner O ON C.CustomerID = O.InsuranceOwnerID";
                    break;
            }

            DataSet ds = db.getSelect(query);
            if (ds != null && ds.Tables.Count > 0)
            {
                CustomerData = ds.Tables[0];
            }
        }

        public IActionResult OnPostDelete(int id)
        {
            DBConnection db = new DBConnection();

            db.getSelect($"DELETE FROM dbo.CustomerPhones WHERE CustomerID = {id}");

            db.getSelect($"DELETE FROM dbo.FamilyMember WHERE FamilyMemberID = {id}");

            db.getSelect($"DELETE FROM dbo.InsuranceOwner WHERE InsuranceOwnerID = {id}");

            db.getSelect($"DELETE FROM dbo.Customer WHERE CustomerID = {id}");

            return RedirectToPage("CustomerListModel", new { category = Category });
        }
    }
}