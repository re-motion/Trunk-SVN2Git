using System;
using Remotion.SecurityManager.Clients.Web.Classes;
using Remotion.SecurityManager.Clients.Web.Globalization.UI.OrganizationalStructure;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Globalization;

namespace Remotion.SecurityManager.Clients.Web.UI.OrganizationalStructure
{
  [WebMultiLingualResources (typeof (SearchGroupFormResources))]
  public partial class SearchGroupForm : BasePage
  {

    // types

    // static members and constants

    // member fields

    // construction and disposing

    // methods and properties

    protected void CancelButton_Click (object sender, EventArgs e)
    {
      throw new WxeUserCancelException ();
    }
  }
}
