using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocTextValue"/> type. </summary>
[ToolboxItem (false)]
public class BocTextValueMock: BocTextValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
