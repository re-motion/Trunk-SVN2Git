using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Data.DomainObjects.Web.Test
{
  public partial class WxeUserControlTestPage : WxePage
  {
    protected void Page_Load (object sender, EventArgs e)
    {
      ClientTransactionLabel.Text = ClientTransaction.Current.ToString();
    }
  }
}
