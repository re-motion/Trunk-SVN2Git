// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.ObjectBinding.Web.UI.Controls.Factories;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using System.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the navigation block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  public class BocListNavigationBlockRenderer : IBocListNavigationBlockRenderer
  {
    private const string c_whiteSpace = "&nbsp;";
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
            { GoToOption.Previous, BocList.ResourceIdentifier.GoToPreviousAlternateText },
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

    private readonly HttpContextBase _context;
    private readonly IBocList _list;
    private readonly IResourceUrlFactory _resourceUrlFactory;
    private readonly CssClassContainer _cssClasses;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// <see cref="BocListRendererFactory"/> to obtain an instance of this class.
    /// </remarks>
    public BocListNavigationBlockRenderer (HttpContextBase context, IBocList list, IResourceUrlFactory resourceUrlFactory, CssClassContainer cssClasses)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      ArgumentUtility.CheckNotNull ("list", list);
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _context = context;
      _list = list;
      _resourceUrlFactory = resourceUrlFactory;
      _cssClasses = cssClasses;
    }

    public HttpContextBase Context
    {
      get { return _context; }
    }

    public IBocList List
    {
      get { return _list; }
    }

    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }

    public CssClassContainer CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary> 
    /// Renders the navigation bar consisting of the move buttons and the <see cref="BocList.PageInfo"/>. 
    /// </summary>
    public void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull ("writer", writer);

      bool isFirstPage = List.CurrentPage == 0;
      bool isLastPage = List.CurrentPage + 1 >= List.PageCount;

      writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.Navigator);
      writer.AddStyleAttribute ("position", "relative");
      writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Page info
      string pageInfo;
      if (StringUtility.IsNullOrEmpty (List.PageInfo))
        pageInfo = List.GetResourceManager().GetString (BocList.ResourceIdentifier.PageInfo);
      else
        pageInfo = List.PageInfo;

      string navigationText = string.Format (pageInfo, List.CurrentPage + 1, List.PageCount);
      // Do not HTML encode.
      writer.Write (navigationText);

      if (List.HasClientScript)
      {
        writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);

        RenderNavigationIcon (writer, isFirstPage, GoToOption.First);
        RenderNavigationIcon (writer, isFirstPage, GoToOption.Previous);
        RenderNavigationIcon (writer, isLastPage, GoToOption.Next);
        RenderNavigationIcon (writer, isLastPage, GoToOption.Last);
      }
      writer.RenderEndTag();
    }

    /// <summary>Renders the appropriate icon for the given <paramref name="command"/>, depending on <paramref name="isInactive"/>.</summary>
    private void RenderNavigationIcon (HtmlTextWriter writer, bool isInactive, GoToOption command)
    {
      if (isInactive || List.EditModeController.IsRowEditModeActive)
      {
        var imageUrl = GetResolvedImageUrl(s_inactiveIcons[command]);
        new IconInfo (imageUrl.GetUrl()).Render (writer);
      }
      else
      {
        var imageUrl = GetResolvedImageUrl (s_activeIcons[command]);

        string argument = BocList.GoToCommandPrefix + command;
        string postBackEvent = List.Page.ClientScript.GetPostBackEventReference (List, argument);
        writer.AddAttribute (HtmlTextWriterAttribute.Onclick, postBackEvent);
        writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        writer.RenderBeginTag (HtmlTextWriterTag.A);

        var icon = new IconInfo (imageUrl.GetUrl());
        icon.AlternateText = List.GetResourceManager().GetString (s_alternateTexts[command]);
        icon.Render (writer);

        writer.RenderEndTag();
      }

      writer.Write (c_whiteSpace + c_whiteSpace + c_whiteSpace);
    }

    private IResourceUrl GetResolvedImageUrl (string imageUrl)
    {
      return ResourceUrlFactory.CreateThemedResourceUrl(typeof (BocListNavigationBlockRenderer), ResourceType.Image, imageUrl);
    }
  }
}