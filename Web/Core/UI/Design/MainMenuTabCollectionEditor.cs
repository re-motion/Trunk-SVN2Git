using System;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UI.Design
{

public class MainMenuTabCollectionEditor: AdvancedCollectionEditor
{
  public MainMenuTabCollectionEditor (Type type)
    : base (type)
  {
  }

  protected override Type[] CreateNewItemTypes()
  {
    return new Type[] {typeof (MainMenuTab)};
  }
}

}