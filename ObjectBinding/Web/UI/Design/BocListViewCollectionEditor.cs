using System;
using System.ComponentModel;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{
public class BocListViewCollectionEditor: AdvancedCollectionEditor
{
  public BocListViewCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {
        typeof (BocListView)
    };
  }

  public override object EditValue (ITypeDescriptorContext context, IServiceProvider provider, object value)
  {
    return EditValue (context, provider, value, 600, 400, 2);
  }
}

}