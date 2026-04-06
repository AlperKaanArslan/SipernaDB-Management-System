<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="student.aspx.cs" Inherits="CSE355BYS.student" MasterPageFile="~/template.Master" %>

<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1">
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    <br />
    <br />
    <asp:CheckBoxList ID="CheckBoxList1" runat="server">
    </asp:CheckBoxList>
    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Enroll" />
&nbsp;&nbsp;
    <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
    <br />
    <br />
    <br />
    <br />
</asp:Content>


