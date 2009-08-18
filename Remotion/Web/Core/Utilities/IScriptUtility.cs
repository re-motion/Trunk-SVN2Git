using System;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Utilities
{
  public interface IScriptUtility
  {
    void RegisterElementForBorderSpans (HtmlHeadAppender htmlHeadAppender, IControl control, string jQuerySelectorForBorderSpanTarget);
    /// <summary>
    /// Registers the <paramref name="eventHandler"/> for the element identified by the <paramref name="jquerySelector"/>. 
    /// </summary>
    /// <param name="control">The <see cref="IControl"/> for which the <paramref name="eventHandler"/> is registered.</param>
    /// <param name="jquerySelector">The element-selector in jquery-syntax.</param>
    /// <param name="eventHandler">The eventhandler, with the following signatur: <c>function (element)</c>.</param>
    void RegisterResizeOnElement (IControl control, string jquerySelector, string eventHandler);
  }
}