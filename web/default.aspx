<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CSE355BYS._default" MasterPageFile="~/template.Master" %>
<asp:Content ID="Content1" runat="server" contentplaceholderid="ContentPlaceHolder1">
    <p>
        &nbsp;</p>
    <p>
        Student Id&nbsp;
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
    </p>
    <p>
        Password&nbsp;
        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
    </p>
    <p>
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Log In" style="height: 26px" />
    </p>
    <p>
        &nbsp;</p>
    <p>
        &nbsp;</p>
</asp:Content>

