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
  /// Base class for all control objects representing a <see cref="T:Remotion.Web.UI.Controls.DropDownMenu"/>.
  /// </summary>
  public abstract class DropDownMenuControlObjectBase : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems
  {
    protected DropDownMenuControlObjectBase ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    protected abstract void OpenDropDownMenu ();

    public IControlObjectWithSelectableItems SelectItem ()
    {
      return this;
    }

    public UnspecifiedPageObject SelectItem (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SelectItem().WithItemID (itemID, completionDetection);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithItemID (
        string itemID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute ("li.DropDownMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (scope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithIndex (
        int index,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindXPath (string.Format ("li[{0}]", index));
      return ClickItem (scope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithHtmlID (
        string htmlID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindId (htmlID);
      return ClickItem (scope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithText (
        string text,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var dropDownMenuScope = GetDropDownMenuScope();
      var scope = dropDownMenuScope.FindTagWithAttribute ("li.DropDownMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (scope, completionDetection, modalDialogHandler);
    }

    private ElementScope GetDropDownMenuScope ()
    {
      OpenDropDownMenu();

      var dropDownMenuOptionsScope = Context.RootScope.FindCss ("ul.DropDownMenuOptions");
      return dropDownMenuOptionsScope;
    }

    private UnspecifiedPageObject ClickItem (ElementScope item, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      var actualCompletionDetector = GetActualCompletionDetector (item, completionDetection);

      var anchorScope = item.FindLink();
      anchorScope.ClickAndWait (Context, actualCompletionDetector, modalDialogHandler);
      return UnspecifiedPage();
    }
  }
}