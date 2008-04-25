using System;
using System.ComponentModel;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls
{
[ToolboxItem (false)]
public class TabbedMenuMock: TabbedMenu
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}
}
