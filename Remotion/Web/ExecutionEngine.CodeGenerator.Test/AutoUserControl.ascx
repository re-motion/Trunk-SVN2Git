<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AutoUserControl.ascx.cs" Inherits="Test.AutoUserControl" %>
<%@ Register TagPrefix="remotion" Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" %>
<div>
<h1>Auto User Control</h1>
            <table>
            <tr><td>IsPostBack</td><td><asp:Label ID="IsPostBackLabel" runat="server" /></td></tr>
            <tr>
                <td>
                    In
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InArgField" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    InOut
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="InOutArgField" runat="server"></asp:TextBox></td>
            </tr>
            <tr>
                <td>
                    Out
                </td>
                <td style="width: 3px">
                    <asp:TextBox ID="OutArgField" runat="server"></asp:TextBox></td>
            </tr>
        </table>

<remotion:WebButton ID="Button1" runat="server" OnClick="Button1_Click" Text="Button" /></div>
