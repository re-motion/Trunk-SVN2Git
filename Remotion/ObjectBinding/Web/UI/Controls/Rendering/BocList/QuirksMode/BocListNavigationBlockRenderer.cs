// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode.Factories;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Rendering.BocList.QuirksMode
{
  /// <summary>
  /// Responsible for rendering the navigation block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListNavigationBlockRenderer : BocListRendererBase, IBocListNavigationBlockRenderer
  {
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

    private static readonly IDictionary<GoToOption, Controls.BocList.ResourceIdentifier> s_alternateTexts =
        new Dictionary
            <GoToOption, Controls.BocList.ResourceIdentifier>
        {
            { GoToOption.First, Controls.BocList.ResourceIdentifier.GoToFirstAlternateText },
            { GoToOption.Previous, Controls.BocList.ResourceIdentifier.GoToPreviousAlternateText },
            { GoToOption.Next, Controls.BocList.ResourceIdentifier.GoToNextAlternateText },
            { GoToOption.Last, Controls.BocList.ResourceIdentifier.GoToLastAlternateText }
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
    public BocListNavigationBlockRenderer (IHttpContext context, HtmlTextWriter writer, IBocList list, CssClassContainer cssClasses)
        : base (context, writer, list, cssClasses)
    {
    }

    /// <summary> 
    /// Renders the navigation bar consisting of the move buttons and the <see cref="Remotion.ObjectBinding.Web.UI.Controls.BocList.PageInfo"/>. 
    /// </summary>
    public void Render ()
    {
      bool isFirstPage = List.CurrentPage == 0;
      bool isLastPage = List.CurrentPage + 1 >= List.PageCount;

      Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Navigator);
      Writer.AddStyleAttribute ("position", "relative");
      Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Page info
      string pageInfo;
      if (StringUtility.IsNullOrEmpty (List.PageInfo))
        pageInfo = List.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.PageInfo);
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
      Writer.RenderEndTag();
    }

    /// <summary>Renders the appropriate icon for the given <paramref name="command"/>, depending on <paramref name="isInactive"/>.</summary>
    private void RenderNavigationIcon (bool isInactive, GoToOption command)
    {
      if (isInactive || List.EditModeController.IsRowEditModeActive)
      {
        string imageUrl = GetResolvedImageUrl(s_inactiveIcons[command]);
        RenderIcon (new IconInfo (imageUrl), null);
      }
      else
      {
        string imageUrl = GetResolvedImageUrl (s_activeIcons[command]);

        string argument = Controls.BocList.c_goToCommandPrefix + command;
        string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
        postBackEvent += "; return false;";
        Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        Writer.RenderBeginTag (HtmlTextWriterTag.A);
        RenderIcon (new IconInfo (imageUrl), s_alternateTexts[command]);
        Writer.RenderEndTag();
      }
      
      Writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    }

    private string GetResolvedImageUrl (string imageUrl)
    {
      imageUrl = ResourceUrlResolver.GetResourceUrl (List, Context, typeof (Controls.BocList), ResourceType.Image, imageUrl);
      return imageUrl;
    }
  }
}