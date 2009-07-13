<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DropDownMenu.aspx.cs" Inherits="Remotion.Web.Test.DropDownMenu" %>
<% %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Register TagPrefix="remotion" Namespace="Remotion.Web.UI.Controls" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <remotion:HtmlHeadContents ID="HeadContents" runat="server" />
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
    <tr>
    <td><remotion:DropDownMenu ID="EditableEnabled" runat="server" TitleText="" /></td>
    </tr>
    <tr>
    <td>
    <span style="display:inline-block; border:solid 1px black"><a href="#"></a><span>X</span></span>
    </td>
    </tr>
    </table>
    </div>
    </form>
</body>
</html>
