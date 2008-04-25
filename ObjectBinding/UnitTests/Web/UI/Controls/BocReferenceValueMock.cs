using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="BocReferenceValue"/> type. </summary>
[ToolboxItem (false)]
public class BocReferenceValueMock: BocReferenceValue
{
	public new void EvaluateWaiConformity ()
  {
    base.EvaluateWaiConformity ();
  }

  public new string InternalValue
  {
    get { return base.InternalValue; }
    set { base.InternalValue = value; }
  }

  public new bool HasOptionsMenu
  {
    get { return base.HasOptionsMenu; }
  }

  public new bool IsCommandEnabled (bool isReadOnly)
  {
    return base.IsCommandEnabled (isReadOnly);
  }
}

}
