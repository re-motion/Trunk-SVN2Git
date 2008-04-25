using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{

public class BocReferenceValueDesigner: WebControlDesigner
{
  public override void OnComponentChanged(object sender, System.ComponentModel.Design.ComponentChangedEventArgs ce)
  {
    base.OnComponentChanged (sender, ce);
    if (ce.Member.Name == "Command")
    {
      PropertyDescriptor persistedCommand = TypeDescriptor.GetProperties (Component)["PersistedCommand"];
      RaiseComponentChanged (persistedCommand, null, ((BocReferenceValue) Component).PersistedCommand);
    }
  }
}

}
