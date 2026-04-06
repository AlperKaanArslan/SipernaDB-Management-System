using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;

namespace SipernaWeb.Pages
{
    public class PolicyDetailsModel : PageModel
    {
        public DataRow PolicyRow { get; set; }
        public DataTable Beneficiaries { get; set; }
        public DataTable Network { get; set; }

        public void OnGet(int id)
        {
            DBConnection db = new DBConnection();

            string q1 = $@"SELECT P.*, (C.Name + ' ' + C.Surname) AS OwnerName FROM dbo.Policy P 
                   JOIN dbo.Customer C ON P.InsuranceOwnerID = C.CustomerID WHERE P.PolicyID = {id}";
            DataTable dt1 = db.getSelect(q1).Tables[0];
            if (dt1.Rows.Count > 0) PolicyRow = dt1.Rows[0];

            string q2 = $@"SELECT C.Name + ' ' + C.Surname AS FullName, C.Gender FROM dbo.PolicyBeneficiaries PB 
                   JOIN dbo.Customer C ON PB.FamilyMemberID = C.CustomerID WHERE PB.PolicyID = {id}";
            Beneficiaries = db.getSelect(q2).Tables[0];

               string q3 = $@"SELECT Pr.InstitutionName, Pr.Address, Pr.ProviderType FROM dbo.PolicyNetwork PN 
               JOIN dbo.Provider Pr ON PN.ProviderID = Pr.ProviderID 
               WHERE PN.PolicyID = {id}";

            Network = db.getSelect(q3).Tables[0];

            Network = db.getSelect(q3).Tables[0];

            Network = db.getSelect(q3).Tables[0];
        }
    }
}