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
  /// Control object for <see cref="T:Remotion.Web.UI.Controls.ListMenu"/>.
  /// </summary>
  public class ListMenuControlObject : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithSelectableItems
  {
    public ListMenuControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

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

      return SelectItem().WithItemID (itemID, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithItemID (
        string itemID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var itemScope = Scope.FindTagWithAttribute ("span.listMenuItem", DiagnosticMetadataAttributes.ItemID, itemID);
      return ClickItem (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithIndex (
        int index,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var itemScope = Scope.FindChild ((index - 1).ToString());
      return ClickItem (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithHtmlID (
        string htmlID,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var itemScope = Scope.FindId (htmlID);
      return ClickItem (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithSelectableItems.WithText (
        string text,
        ICompletionDetection completionDetection,
        IModalDialogHandler modalDialogHandler)
    {
      var itemScope = Scope.FindTagWithAttribute ("span.listMenuItem", DiagnosticMetadataAttributes.Text, text);
      return ClickItem (itemScope, completionDetection, modalDialogHandler);
    }

    private UnspecifiedPageObject ClickItem (ElementScope itemScope, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      var itemCommandScope = itemScope.FindLink();
      var itemCommand = new CommandControlObject (Context.CloneForControl (itemCommandScope));
      return itemCommand.Click (completionDetection, modalDialogHandler);
    }
  }
}