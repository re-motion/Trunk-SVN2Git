using System;
using System.ComponentModel;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.WebButtonTests
{

/// <summary> Exposes non-public members of the <see cref="WebButton"/> type. </summary>
[ToolboxItem (false)]
public class TestWebButton: WebButton
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }

  public new bool IsLegacyButtonEnabled
  {
    get { return base.IsLegacyButtonEnabled; }
  }
}

}
