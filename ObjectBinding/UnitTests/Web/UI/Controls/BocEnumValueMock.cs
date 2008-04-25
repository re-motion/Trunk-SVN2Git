using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocEnumValue"/> type. </summary>
[ToolboxItem (false)]
public class BocEnumValueMock: BocEnumValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }

  public new void RefreshEnumList()
  {
    base.RefreshEnumList();
  }
}

}
