using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

/// <summary>
///   A <see cref="HyperLink"/> that provides integration into the <see cref="ISmartNavigablePage"/> framework by
///   automatically appending the navigation URL parameters to the rendered <see cref="HyperLink.NavigateUrl"/>.
/// </summary>
public class SmartHyperLink : HyperLink
{
	public SmartHyperLink()
	{
	}

  /// <summary> 
  ///   Uses <see cref="ISmartNavigablePage.AppendNavigationUrlParameters"/> to include the navigation URL parameters
  ///   with the rendered <see cref="HyperLink.NavigateUrl"/>.
  /// </summary>
  protected override void AddAttributesToRender(HtmlTextWriter writer)
  {
    string navigateUrlBackup = NavigateUrl;
    bool hasNavigateUrl = ! StringUtility.IsNullOrEmpty (NavigateUrl);
    bool isDesignMode = ControlHelper.IsDesignMode (this);

    if (! isDesignMode && Page is ISmartNavigablePage && hasNavigateUrl)
      NavigateUrl = ((ISmartNavigablePage) Page).AppendNavigationUrlParameters (NavigateUrl);

    base.AddAttributesToRender (writer);
    
    if (hasNavigateUrl)
      NavigateUrl = navigateUrlBackup;
  }
}

}
