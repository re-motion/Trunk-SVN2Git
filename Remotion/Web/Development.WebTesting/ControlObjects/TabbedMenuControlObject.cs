// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMenu"/>.
  /// </summary>
  [UsedImplicitly]
  public class TabbedMenuControlObject : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IControlObjectWithSelectableSubItems
  {
    // Todo RM-6297: Replace IControlObjectWithSelectableSubItems with a sub menu control object, it is a cleaner approach.

    public TabbedMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public string GetStatusText ()
    {
      return Scope.FindCss ("td.tabbedMenuStatusCell").Text.Trim();
    }

    public IControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    public UnspecifiedPageObject SelectItem (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectItem().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithIndex (int index, ICompletionDetection completionDetection)
    {
      var menuItemScope = GetMainMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithHtmlID (string htmlID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithText (string text, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetMainMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    public IControlObjectWithSelectableSubItems SelectSubItem ()
    {
      return this;
    }

    public UnspecifiedPageObject SelectSubItem (string itemID, ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectSubItem().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableSubItems.WithItemID (string itemID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableSubItems.WithIndex (int index, ICompletionDetection completionDetection)
    {
      var menuItemScope = GetSubMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableSubItems.WithHtmlID (string htmlID, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableSubItems.WithText (string text, ICompletionDetection completionDetection)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var menuItemScope = GetSubMenuScope().FindDMA ("span", DiagnosticMetadataAttributes.Text, text);
      return SelectMenuOrSubMenuItem (menuItemScope, completionDetection);
    }

    private UnspecifiedPageObject SelectMenuOrSubMenuItem (ElementScope menuItemScope, ICompletionDetection completionDetection)
    {
      var menuItemCommandScope = menuItemScope.FindLink();
      var menuItemCommand = new CommandControlObject (Context.CloneForControl (menuItemCommandScope));
      return menuItemCommand.Click (completionDetection);
    }

    private ElementScope GetMainMenuScope ()
    {
      return Scope.FindCss ("td.tabbedMainMenuCell");
    }

    private ElementScope GetSubMenuScope ()
    {
      return Scope.FindCss ("td.tabbedSubMenuCell");
    }
  }
}