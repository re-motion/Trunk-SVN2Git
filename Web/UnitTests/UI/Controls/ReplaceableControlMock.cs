using System;
using System.Web.UI;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.ControlReplacing;

namespace Remotion.Web.UnitTests.UI.Controls
{
  public class ReplaceableControlMock : ControlMock, INamingContainer, IReplaceableControl
  {
    private readonly LazyInitializationContainer _lazyInitializationContainer = new LazyInitializationContainer();

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      EnsureLazyInitializationContainer();
    }

    public void EnsureLazyInitializationContainer ()
    {
      _lazyInitializationContainer.Ensure (base.Controls);
    }

    public override ControlCollection Controls
    {
      get { return _lazyInitializationContainer.GetControls (base.Controls); }
    }

    public bool IsInitialized
    {
      get { return _lazyInitializationContainer.IsInitialized; }
    }

    public ControlReplacer Replacer { get; set; }
  }
}