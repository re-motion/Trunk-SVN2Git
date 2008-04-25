using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{

public class SubMenuTabCollectionEditor: AdvancedCollectionEditor
{
  public SubMenuTabCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (SubMenuTab)};
  }
}

}