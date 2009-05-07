using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Renderers
{
  /// <summary>
  /// Responsible for rendering the navigation block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListNavigatorRenderer : BocListBaseRenderer
  {
    protected const string c_goToCommandPrefix = "GoTo=";

    private const string c_goToFirstIcon = "MoveFirst.gif";
    private const string c_goToLastIcon = "MoveLast.gif";
    private const string c_goToPreviousIcon = "MovePrevious.gif";
    private const string c_goToNextIcon = "MoveNext.gif";
    private const string c_goToFirstInactiveIcon = "MoveFirstInactive.gif";
    private const string c_goToLastInactiveIcon = "MoveLastInactive.gif";
    private const string c_goToPreviousInactiveIcon = "MovePreviousInactive.gif";
    private const string c_goToNextInactiveIcon = "MoveNextInactive.gif";

    private static readonly IDictionary<GoToOption, string> s_activeIcons = new Dictionary<GoToOption, string>
                                                                            {
                                                                                { GoToOption.First, c_goToFirstIcon },
                                                                                { GoToOption.Previous, c_goToPreviousIcon },
                                                                                { GoToOption.Next, c_goToNextIcon },
                                                                                { GoToOption.Last, c_goToLastIcon }
                                                                            };
    private static readonly IDictionary<GoToOption, string> s_inactiveIcons = new Dictionary<GoToOption, string>
                                                                            {
                                                                                { GoToOption.First, c_goToFirstInactiveIcon },
                                                                                { GoToOption.Previous, c_goToPreviousInactiveIcon },
                                                                                { GoToOption.Next, c_goToNextInactiveIcon },
                                                                                { GoToOption.Last, c_goToLastInactiveIcon }
                                                                            };

    private static readonly IDictionary<GoToOption, BocList.ResourceIdentifier> s_alternateTexts =
                                                      new Dictionary
                                                          <GoToOption, BocList.ResourceIdentifier>
                                                      {
                                                          { GoToOption.First, BocList.ResourceIdentifier.GoToFirstAlternateText },
                                                          {GoToOption.Previous,BocList.ResourceIdentifier.GoToPreviousAlternateText },
                                                          { GoToOption.Next, BocList.ResourceIdentifier.GoToNextAlternateText },
                                                          { GoToOption.Last, BocList.ResourceIdentifier.GoToLastAlternateText }
                                                      };

    /// <summary> The possible directions for paging through the List. </summary>
    private enum GoToOption
    {
      /// <summary> Don't page. </summary>
      Undefined,
      /// <summary> Move to first page. </summary>
      First,
      /// <summary> Move to last page. </summary>
      Last,
      /// <summary> Move to previous page. </summary>
      Previous,
      /// <summary> Move to next page. </summary>
      Next
    }

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain an instance of this class.
    /// </remarks>
    public BocListNavigatorRenderer (BocList list, HtmlTextWriter writer)
      : base (list, writer)
    {
    }

    /// <summary> 
    /// Renders the navigation bar consisting of the move buttons and the <see cref="BocList.PageInfo"/>. 
    /// </summary>
    public void Render ()
    {
      bool isFirstPage = List.CurrentPage == 0;
      bool isLastPage = List.CurrentPage + 1 >= List.PageCount;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, List.CssClassNavigator);
      Writer.AddStyleAttribute ("position", "relative");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Page info
      string pageInfo;
      if (StringUtility.IsNullOrEmpty (List.PageInfo))
        pageInfo = List.GetResourceManager ().GetString (BocList.ResourceIdentifier.PageInfo);
      else
        pageInfo = List.PageInfo;

      string navigationText = string.Format (pageInfo, List.CurrentPage + 1, List.PageCount);
      // Do not HTML encode.
      Writer.Write (navigationText);

      if (List.HasClientScript)
      {
        Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

        RenderNavigationIcon (isFirstPage, GoToOption.First);
        RenderNavigationIcon (isFirstPage, GoToOption.Previous);
        RenderNavigationIcon (isLastPage, GoToOption.Next);
        RenderNavigationIcon (isLastPage, GoToOption.Last);
      }
      Writer.RenderEndTag ();
    }

    /// <summary>Renders the appropriate icon for the given <paramref name="command"/>, depending on <paramref name="isInactive"/>.</summary>
    private void RenderNavigationIcon (bool isInactive, GoToOption command)
    {
      string imageUrl;
      if (isInactive || List.IsRowEditModeActive)
        imageUrl = s_inactiveIcons[command];
      else
        imageUrl = s_activeIcons[command];
      imageUrl = ResourceUrlResolver.GetResourceUrl (List, HttpContext.Current, typeof (BocList), ResourceType.Image, imageUrl);
      if (isInactive || List.IsRowEditModeActive)
      {
        RenderIcon (new IconInfo (imageUrl), null);
      }
      else
      {
        string argument = c_goToCommandPrefix + command;
        string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
        postBackEvent += "; return false;";
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        Writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (new IconInfo (imageUrl), s_alternateTexts[command]);
        Writer.RenderEndTag ();
      }
      Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    }
  }
}
