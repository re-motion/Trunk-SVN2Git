using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{
public class PropertyPathBindingCollectionEditor: AdvancedCollectionEditor
{
  public PropertyPathBindingCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
      typeof (PropertyPathBinding)};
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    return EditValue (context, provider, value, 600, 400, 2);
  }
}

}