using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{

public class WebTreeNodeCollectionEditor: AdvancedCollectionEditor
{
  public WebTreeNodeCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (WebTreeNode)};
  }
}

}