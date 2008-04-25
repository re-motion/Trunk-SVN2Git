using System;
using System.Web.UI;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   This interface contains all public members of System.Web.UI.TemplateControl. It is used to 
///   derive interfaces that will be implemented by deriving from System.Web.UI.TemplateControl.
/// </summary>
/// <remarks>
///   The reason for providing this interface is that derived interfaces do not need to be casted 
///   to System.Web.UI.TemplateControl.
/// </remarks>
public interface ITemplateControl: IControl, INamingContainer
{
  event EventHandler AbortTransaction;
  event EventHandler CommitTransaction;
  event EventHandler Error;

  Control LoadControl(string virtualPath);
  ITemplate LoadTemplate(string virtualPath);
  Control ParseControl(string content);
}

}
