using System;
using Remotion.Web.UI.Design;

namespace Remotion.ObjectBinding.Web.UI.Design
{

public class BocDataSourceDesigner: WebControlDesigner
{
  public override string GetDesignTimeHtml()
  {
    return base.CreatePlaceHolderDesignTimeHtml();
  }
}

}
