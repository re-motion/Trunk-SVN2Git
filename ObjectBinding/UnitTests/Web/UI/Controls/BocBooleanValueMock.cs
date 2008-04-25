using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocBooleanValue"/> type. </summary>
[ToolboxItem (false)]
public class BocBooleanValueMock: BocBooleanValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
