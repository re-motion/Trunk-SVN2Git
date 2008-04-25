using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{
public class WebMenuItemCollectionEditor: AdvancedCollectionEditor
{
  public WebMenuItemCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (WebMenuItem)};
  }
}

}