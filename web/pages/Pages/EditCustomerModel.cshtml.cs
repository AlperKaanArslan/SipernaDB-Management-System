using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class EditCustomerModel : PageModel
    {
        [BindProperty] public int CustomerID { get; set; }
        [BindProperty] public string Name { get; set; } = "";
        [BindProperty] public string Surname { get; set; } = "";
        [BindProperty] public string CustomerType { get; set; } = ""; // 'F' veya 'O'

        // Family Member Alanlarý (Veritabanýndaki isimlerle tam uyumlu)
        [BindProperty] public string? RelationType { get; set; }
        [BindProperty] public string? MaritalStatus { get; set; }

        // Insurance Owner Alanlarý
        [BindProperty] public string? Address { get; set; }
        [BindProperty] public string? Email { get; set; }

        public void OnGet(int id)
        {
            CustomerID = id;
            DBConnection db = new DBConnection();

            // Tüm tablolarý birleþtiren ana sorgu
            string query = $@"
                SELECT C.Name, C.Surname, C.CustomerType, 
                       F.RelationType, F.MaritalStatus,
                       O.Address, O.Email
                FROM dbo.Customer C
                LEFT JOIN dbo.FamilyMember F ON C.CustomerID = F.FamilyMemberID
                LEFT JOIN dbo.InsuranceOwner O ON C.CustomerID = O.InsuranceOwnerID
                WHERE C.CustomerID = {id}";

            DataSet ds = db.getSelect(query);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                Name = row["Name"].ToString() ?? "";
                Surname = row["Surname"].ToString() ?? "";

                // ÖNEMLÝ: char(1) olduðu için boþluðu temizle
                CustomerType = row["CustomerType"].ToString()?.Trim().ToUpper() ?? "";

                // ARTIK 'F' KONTROLÜ YAPIYORUZ
                if (CustomerType == "F")
                {
                    RelationType = row["RelationType"].ToString();
                    MaritalStatus = row["MaritalStatus"].ToString();
                }
                else if (CustomerType == "O") // Owner kontrolü
                {
                    Address = row["Address"].ToString();
                    Email = row["Email"].ToString();
                }
            }
        }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();

            db.getSelect($"UPDATE dbo.Customer SET Name = '{Name}', Surname = '{Surname}' WHERE CustomerID = {CustomerID}");

            string type = CustomerType.Trim().ToUpper();

            if (type == "F") 
            {
                db.getSelect($"UPDATE dbo.FamilyMember SET RelationType = '{RelationType}', MaritalStatus = '{MaritalStatus}' WHERE FamilyMemberID = {CustomerID}");
            }
            else if (type == "O") 
            {
                db.getSelect($"UPDATE dbo.InsuranceOwner SET Address = '{Address}', Email = '{Email}' WHERE InsuranceOwnerID = {CustomerID}");
            }
            return RedirectToPage("CustomerListModel", new { category = type });
        }
    }
}