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
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;

namespace Remotion.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:Remotion.Web.UI.Controls.WebTreeView"/>.
  /// </summary>
  public class WebTreeViewNodeControlObject : WebFormsControlObject, IControlObjectWithNodes<WebTreeViewNodeControlObject>, IControlObjectWithText
  {
    public WebTreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      return Scope[DiagnosticMetadataAttributes.Content];
    }

    /// <summary>
    /// Returns whether the node is currently selected.
    /// </summary>
    public bool IsSelected ()
    {
      return Scope[DiagnosticMetadataAttributes.WebTreeViewIsSelectedNode] != null;
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      return int.Parse (Scope[DiagnosticMetadataAttributes.WebTreeViewNumberOfChildren]);
    }

    /// <inheritdoc/>
    public IControlObjectWithNodes<WebTreeViewNodeControlObject> GetNode ()
    {
      return this;
    }

    /// <inheritdoc/>
    public WebTreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithItemID (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.ItemID, itemID);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithIndex (int index)
    {
      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.IndexInCollection, index.ToString());
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <inheritdoc/>
    WebTreeViewNodeControlObject IControlObjectWithNodes<WebTreeViewNodeControlObject>.WithDisplayText (string displayText)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      var nodeScope = Scope.FindTagWithAttribute ("ul li", DiagnosticMetadataAttributes.Content, displayText);
      return new WebTreeViewNodeControlObject (Context.CloneForControl (nodeScope));
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public WebTreeViewNodeControlObject Expand ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var expandAnchorScope = Scope.FindTagWithAttribute (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownExpandAnchor);
      expandAnchorScope.ClickAndWait (Context, actualCompletionDetector, null);
      return this;
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public WebTreeViewNodeControlObject Collapse ()
    {
      var actualCompletionDetector = GetActualCompletionDetector (null);

      var collapseAnchorScope = Scope.FindTagWithAttribute (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownCollapseAnchor);
      collapseAnchorScope.ClickAndWait (Context, actualCompletionDetector, null);
      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public WebTreeViewNodeControlObject Select (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ClickNode (completionDetection, modalDialogHandler);
      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    public UnspecifiedPageObject Click (
        [CanBeNull] ICompletionDetection completionDetection = null,
        [CanBeNull] IModalDialogHandler modalDialogHandler = null)
    {
      ClickNode (completionDetection, modalDialogHandler);
      return UnspecifiedPage();
    }

    private void ClickNode (ICompletionDetection completionDetection, IModalDialogHandler modalDialogHandler)
    {
      var actualCompletionDetector = GetActualCompletionDetector (completionDetection);

      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      selectAnchorScope.ClickAndWait (Context, actualCompletionDetector, modalDialogHandler);
    }

    public ContextMenuControlObject OpenContextMenu ()
    {
      var selectAnchorScope = GetWellKnownSelectAnchorScope();
      return new ContextMenuControlObject (Context.CloneForControl (selectAnchorScope));
    }

    private ElementScope GetWellKnownSelectAnchorScope ()
    {
      return Scope.FindTagWithAttribute (
          "span a",
          DiagnosticMetadataAttributes.WebTreeViewWellKnownAnchor,
          DiagnosticMetadataAttributeValues.WebTreeViewWellKnownSelectAnchor);
    }

    /// <inheritdoc/>
    protected override ICompletionDetection GetDefaultCompletionDetection (ElementScope scope)
    {
      return Continue.When (Wxe.PostBackCompleted);
    }
  }
}