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
  public class TabbedMenuControlObject
      : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
  {
    public TabbedMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the tabbed menu's status text.
    /// </summary>
    public string GetStatusText ()
    {
      return Scope.FindCss ("td.tabbedMenuStatusCell").Text.Trim();
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SelectItem (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectItem().WithItemID (itemID, completionDetection, modalDialogHandler);
    }

    /// <summary>
    /// Gives access to the sub menu.
    /// </summary>
    public IControlObjectWithSelectableItems SubMenu
    {
      get { return new SubMenuItems (Context); }
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (
        string itemID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var menuItemScope = GetMainMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.ItemID, itemID);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (
        int index,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var menuItemScope = GetMainMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
      return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (
        string htmlID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var menuItemScope = Scope.FindId (htmlID);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (
        string displayText,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var menuItemScope = GetMainMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.Content, displayText);
      return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
    }

    private static UnspecifiedPageObject SelectMenuOrSubMenuItem (
        ControlObjectContext context,
        ElementScope menuItemScope,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var menuItemCommandScope = menuItemScope.FindLink();
      var menuItemCommand = new CommandControlObject (context.CloneForControl (menuItemCommandScope));
      return menuItemCommand.Click (completionDetection, modalDialogHandler);
    }

    private ElementScope GetMainMenuScope ()
    {
      return Scope.FindCss ("td.tabbedMainMenuCell");
    }

    private class SubMenuItems
        : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems, IFluentControlObjectWithSelectableItems
    {
      public SubMenuItems ([NotNull] ControlObjectContext context)
          : base (context)
      {
      }

      public IFluentControlObjectWithSelectableItems SelectItem ()
      {
        return this;
      }

      public UnspecifiedPageObject SelectItem (
          string itemID,
          ICompletionDetection completionDetection = null,
          IModalDialogHandler modalDialogHandler = null)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

        return SelectItem().WithItemID (itemID, completionDetection, modalDialogHandler);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithItemID (
          string itemID,
          ICompletionDetection completionDetection,
          IModalDialogHandler modalDialogHandler)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

        var menuItemScope = GetSubMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.ItemID, itemID);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithIndex (
          int index,
          ICompletionDetection completionDetection,
          IModalDialogHandler modalDialogHandler)
      {
        var menuItemScope = GetSubMenuScope().FindXPath (string.Format ("(.//li/span/span[2])[{0}]", index));
        return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithHtmlID (
          string htmlID,
          ICompletionDetection completionDetection,
          IModalDialogHandler modalDialogHandler)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

        var menuItemScope = Scope.FindId (htmlID);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
      }

      UnspecifiedPageObject IFluentControlObjectWithSelectableItems.WithDisplayText (
          string displayText,
          ICompletionDetection completionDetection,
          IModalDialogHandler modalDialogHandler)
      {
        ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

        var menuItemScope = GetSubMenuScope().FindTagWithAttribute ("span", DiagnosticMetadataAttributes.Content, displayText);
        return SelectMenuOrSubMenuItem (Context, menuItemScope, completionDetection, modalDialogHandler);
      }

      private ElementScope GetSubMenuScope ()
      {
        return Scope.FindCss ("td.tabbedSubMenuCell");
      }
    }
  }
}