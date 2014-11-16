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
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.WebTabStrip"/>.
  /// </summary>
  public class WebTabStripControlObject : WebFormsControlObjectWithDiagnosticMetadata, IControlObjectWithTabs
  {
    public WebTabStripControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public IControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    public UnspecifiedPageObject SwitchTo (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SwitchTo().WithItemID (itemID, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithItemID (string itemID, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var itemScope = Scope.FindTagWithAttribute ("span.tabStripTab", DiagnosticMetadataAttributes.ItemID, itemID);
      return SwitchTo (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithIndex (int index, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      var xPathSelector = string.Format (
          "(.//span{0})[{1}]",
          XPathUtils.CreateHasOneOfClassesCheck ("tabStripTab", "tabStripTabSelected"),
          index);
      var itemScope = Scope.FindXPath (xPathSelector);
      return SwitchTo (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithHtmlID (string htmlID, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      var itemScope = Scope.FindId (htmlID);
      return SwitchTo (itemScope, completionDetection, modalDialogHandler);
    }

    UnspecifiedPageObject IControlObjectWithTabs.WithText (string text, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);

      var itemScope = Scope.FindTagWithAttribute ("span.tabStripTab", DiagnosticMetadataAttributes.Text, text);
      return SwitchTo (itemScope, completionDetection, modalDialogHandler);
    }

    private UnspecifiedPageObject SwitchTo (ElementScope tabScope, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      var tabCommandScope = tabScope.FindLink();
      var tabCommand = new CommandControlObject (Context.CloneForControl (tabCommandScope));
      return tabCommand.Click (completionDetection, modalDialogHandler);
    }
  }
}