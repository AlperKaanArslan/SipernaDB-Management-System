using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class AddCustomerModel : PageModel
    {
        [BindProperty] public string Name { get; set; } = "";
        [BindProperty] public string Surname { get; set; } = "";
        [BindProperty] public string Gender { get; set; } = "M";
        [BindProperty] public DateTime BirthDate { get; set; } = DateTime.Now.AddYears(-20);
        [BindProperty] public string CustomerType { get; set; } = "O";
        [BindProperty] public string? Address { get; set; }
        [BindProperty] public string? Email { get; set; }
        [BindProperty] public string? RelationType { get; set; }
        [BindProperty] public string? MaritalStatus { get; set; }

        [BindProperty] public List<string> PhoneNumbers { get; set; } = new List<string>();

        public void OnGet() { }

        public IActionResult OnPost()
        {
            DBConnection db = new DBConnection();
            string cleanType = CustomerType.Trim().ToUpper(); // 'O' or 'F'

            string q = $@"INSERT INTO dbo.Customer (CustomerType, Name, Surname, Gender, BirthDate) 
                  VALUES ('{cleanType}', '{Name}', '{Surname}', '{Gender}', '{BirthDate:yyyy-MM-dd}'); 
                  SELECT SCOPE_IDENTITY();";

            int newId = Convert.ToInt32(db.getSelect(q).Tables[0].Rows[0][0]);

            if (cleanType == "O") // Insurance Owner
            {
                string ownerQuery = $@"INSERT INTO dbo.InsuranceOwner (InsuranceOwnerID, Address, Email) 
                               VALUES ({newId}, '{Address}', '{Email}')";
                db.getSelect(ownerQuery);
            }
            else if (cleanType == "F") // Family Member
            {
                string familyQuery = $@"INSERT INTO dbo.FamilyMember (FamilyMemberID, RelationType, MaritalStatus) 
                                VALUES ({newId}, '{RelationType}', '{MaritalStatus}')";
                db.getSelect(familyQuery);
            }

            if (PhoneNumbers != null)
            {
                foreach (var p in PhoneNumbers.Where(x => !string.IsNullOrEmpty(x)))
                {
                    string phoneQuery = $"INSERT INTO dbo.CustomerPhones (CustomerID, PhoneNumber) VALUES ({newId}, '{p}')";
                    db.getSelect(phoneQuery);
                }
            }

            return RedirectToPage("CustomerListModel", new { category = CustomerType });
        }
    }
}