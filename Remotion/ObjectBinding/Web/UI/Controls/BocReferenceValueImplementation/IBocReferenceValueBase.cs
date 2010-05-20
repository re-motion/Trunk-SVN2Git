using System;
using System.Web.UI.WebControls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation
{
  public interface IBocReferenceValueBase : IBusinessObjectBoundEditableWebControl, IBocRenderableControl
  {
    bool? HasValueEmbeddedInsideOptionsMenu { get; }
    bool HasOptionsMenu { get; }
    DropDownMenu OptionsMenu { get; }
    Unit OptionsMenuWidth { get; }
    BocCommand Command { get; }
    bool EnableIcon { get; }
    new IBusinessObjectReferenceProperty Property { get; }
    new IBusinessObjectWithIdentity Value { get; }
    bool IsCommandEnabled (bool readOnly);
    IconInfo GetIcon ();
    string GetLabelText ();
  }
}