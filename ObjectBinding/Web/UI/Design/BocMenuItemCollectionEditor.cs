using System;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{

public class BocMenuItemCollectionEditor: WebMenuItemCollectionEditor
{
  public BocMenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (BocMenuItem)};
  }
}

}
