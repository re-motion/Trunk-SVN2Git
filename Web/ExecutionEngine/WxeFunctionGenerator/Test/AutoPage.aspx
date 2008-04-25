<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AutoPage.aspx.cs" Inherits="Test.AutoPage" %>
<%@ Register TagPrefix="remotion" Assembly="Remotion.Web" Namespace="Remotion.Web.UI.Controls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head>
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
            <table>
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
        <br />
    
        <asp:Button ID="ExecSelfButton" runat="server" Text="Execute self" OnClick="ExecSelfButton_Click" />
        <asp:Button ID="ExecCalledPageButton" runat="server" Text="Execute called page" OnClick="ExecCalledPageButton_Click" />
        <asp:Button ID="ReturnButton" runat="server" Text="Return" OnClick="ReturnButton_Click" />
        <asp:Button ID="NoOpButton" runat="server" Text="NoOp" />

    </div>
    </form>
</body>
</html>
