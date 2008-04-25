using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocDateTimeValue"/> type. </summary>
[ToolboxItem (false)]
public class BocDateTimeValueMock: BocDateTimeValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }
}

}
