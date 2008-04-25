using System;
using System.Web.UI.Design;

namespace Remotion.Web.UI.Design
{

public class FormGridManagerDesigner: ControlDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
