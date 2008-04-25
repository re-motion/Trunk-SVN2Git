using System;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
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
      string url = "~/Images/" + ((BindableObjectClass) obj.BusinessObjectClass).TargetType.FullName + ".gif";
      return new IconInfo (url, Unit.Pixel (16), Unit.Pixel (16));
    }
  }

  public string GetToolTip (IBusinessObject obj)
  {
    if (obj == null)
      return "No ToolTip";
    else
      return "ToolTip: " + ((BindableObjectClass)obj.BusinessObjectClass).TargetType.FullName;
  }

}

}
