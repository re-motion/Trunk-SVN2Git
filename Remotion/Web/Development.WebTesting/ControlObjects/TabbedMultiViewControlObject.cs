﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object for form grids created with <see cref="T:Remotion.Web.UI.Controls.TabbedMultiView"/>.
  /// </summary>
  public class TabbedMultiViewControlObject
      : WebFormsControlObjectWithDiagnosticMetadata, IControlHost, IControlObjectWithTabs, IFluentControlObjectWithTabs
  {
    public TabbedMultiViewControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Returns the top control scope.
    /// </summary>
    public ScopeControlObject GetTopControls ()
    {
      var scope = Scope.FindChild ("TopControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    /// <summary>
    /// Returns the tabbed multi view's active view.
    /// </summary>
    public ScopeControlObject GetActiveView ()
    {
      var scope = Scope.FindChild ("ActiveView");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    /// <summary>
    /// Returns the bottom control scope.
    /// </summary>
    public ScopeControlObject GetBottomControls ()
    {
      var scope = Scope.FindChild ("BottomControl");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithTabs SwitchTo ()
    {
      return this;
    }

    /// <inheritdoc/>
    public UnspecifiedPageObject SwitchTo (
        string itemID,
        ICompletionDetection completionDetection = null,
        IModalDialogHandler modalDialogHandler = null)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return SwitchTo().WithItemID (itemID, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithItemID (string itemID, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetTabStrip().SwitchTo (itemID + "_Tab", completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithIndex (int index, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      return GetTabStrip().SwitchTo().WithIndex (index, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithHtmlID (string htmlID, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("htmlID", htmlID);

      return GetTabStrip().SwitchTo().WithHtmlID (htmlID, completionDetection, modalDialogHandler);
    }

    /// <inheritdoc/>
    UnspecifiedPageObject IFluentControlObjectWithTabs.WithDisplayText (string displayText, ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      return GetTabStrip().SwitchTo().WithDisplayText (displayText, completionDetection, modalDialogHandler);
    }

    private WebTabStripControlObject GetTabStrip ()
    {
      var scope = Scope.FindChild ("TabStrip");
      return new WebTabStripControlObject (Context.CloneForControl (scope));
    }

    /// <inheritdoc/>
    public TControlObject GetControl<TControlObject> (IControlSelectionCommand<TControlObject> controlSelectionCommand)
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("controlSelectionCommand", controlSelectionCommand);

      return Children.GetControl (controlSelectionCommand);
    }
  }
}