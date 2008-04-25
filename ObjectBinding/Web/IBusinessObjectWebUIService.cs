using System;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web
{

/// <summary>
///   Provides services for business object bound web applications
/// </summary>
public interface IBusinessObjectWebUIService: IBusinessObjectService
{
  IconInfo GetIcon (IBusinessObject obj);
  string GetToolTip (IBusinessObject obj);
}

}
