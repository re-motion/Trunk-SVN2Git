using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocDropDownMenu"/> type. </summary>
[ToolboxItem (false)]
public class BocDropDownMenuMock: BocDropDownMenu
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
