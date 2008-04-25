using System;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Reflection
{

public class ReflectionBusinessObjectWebUIService: IBusinessObjectWebUIService
{ 
  public IconInfo GetIcon (IBusinessObject obj)
  {
    if (obj == null)
    {
      string url = "~/Images/NullIcon.gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
    else
    {
      string url = "~/Images/" + obj.BusinessObjectClass.Identifier + ".gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
  }

  public string GetToolTip (IBusinessObject obj)
  {
    if (obj == null)
      return "No ToolTip";
    else
      return "ToolTip: " + obj.BusinessObjectClass.Identifier;
  }

}

}
