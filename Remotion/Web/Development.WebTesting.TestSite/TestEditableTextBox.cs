using System;
using System.Web.UI.WebControls;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public class TestEditableTextBox : TextBox, IEditableControl
  {
    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      this.TextChanged += OnTextChanged;
    }

    private void OnTextChanged (object sender, EventArgs eventArgs)
    {
      IsDirty = true;
    }

    public new IPage Page
    {
      get { return PageWrapper.CastOrCreate (base.Page); }
    }

    public bool IsDirty { get; private set; }

    public string[] GetTrackedClientIDs ()
    {
      return new[] { ClientID };
    }
  }
}