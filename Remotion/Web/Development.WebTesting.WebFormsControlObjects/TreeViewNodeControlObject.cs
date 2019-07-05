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
using System.Linq;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebTestActions;

namespace Remotion.Web.Development.WebTesting.WebFormsControlObjects
{
  /// <summary>
  /// Control object representing a node within a <see cref="T:System.Web.UI.WebControls.TreeView"/>.
  /// </summary>
  public class TreeViewNodeControlObject
      : WebFormsControlObject,
          IControlObjectWithNodes<TreeViewNodeControlObject>,
          IControlObjectWithText
  {
    private class GetNodeImplementationForChildren : IFluentControlObjectWithNodes<TreeViewNodeControlObject>
    {
      private readonly TreeViewNodeControlObject _treeViewNode;

      public GetNodeImplementationForChildren (TreeViewNodeControlObject treeViewNode)
      {
        _treeViewNode = treeViewNode;
      }

      public TreeViewNodeControlObject WithItemID (string itemID)
      {
        throw new NotSupportedException ("The TreeViewNodeControlObject does not support node selection by item ID.");
      }

      public TreeViewNodeControlObject WithIndex (int oneBasedIndex)
      {
        var xpath = string.Format ("[{0}]", oneBasedIndex);
        return FindAndCreateNode (xpath);
      }

      public TreeViewNodeControlObject WithDisplayText (string displayText)
      {
        var xpath = string.Format ("[normalize-space(tbody/tr/td[last()]//*)={0}]", DomSelectorUtility.CreateMatchValueForXPath (displayText));
        return FindAndCreateNode (xpath);
      }

      public TreeViewNodeControlObject WithDisplayTextContains (string containsDisplayText)
      {
        var xpath = string.Format ("[contains(tbody/tr/td[last()]//*, {0})]", DomSelectorUtility.CreateMatchValueForXPath (containsDisplayText));
        return FindAndCreateNode (xpath);
      }

      private TreeViewNodeControlObject FindAndCreateNode (string xpathSuffix)
      {
        var nodeScope = GetChildrenScope (_treeViewNode.Scope).FindXPath ("./table" + xpathSuffix);
        return new TreeViewNodeControlObject (_treeViewNode.Context.CloneForControl (nodeScope));
      }
    }

    public TreeViewNodeControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <inheritdoc/>
    public string GetText ()
    {
      const string xpath = "./tbody/tr/td[last()]";
      return Scope.FindXPath (xpath).Text.Trim();
    }

    /// <summary>
    /// Returns whether the node is checked.
    /// </summary>
    public bool IsChecked ()
    {
      var checkedAttr = GetCheckboxScope()["checked"];
      return checkedAttr != null && checkedAttr == "true";
    }

    /// <summary>
    /// Returns whether the node is the currently selected node.
    /// </summary>
    public bool IsSelected ()
    {
      throw new NotSupportedException (
          "The ASP.NET TreeView control does not indicate which node is currently selected, therefore IsSelected() is not supported.");
    }

    /// <summary>
    /// Returns the number of child nodes.
    /// </summary>
    public int GetNumberOfChildren ()
    {
      return RetryUntilTimeout.Run (() => GetChildrenScope (Scope).FindAllXPath ("./table").Count());
    }

    /// <summary>
    /// Expands the node.
    /// </summary>
    public TreeViewNodeControlObject Expand ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = MergeWithDefaultActionOptions (Scope, actionOptions);

      const string xpath = "./tbody/tr/td/a[contains(@href,\"','t\")]";
      var expandLinkScope = Scope.FindXPath (xpath);
      ExecuteAction (new SimpleClickAction (this, expandLinkScope), actualCompletionDetector);
      return this;
    }

    /// <summary>
    /// Collapses the node.
    /// </summary>
    public TreeViewNodeControlObject Collapse ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      return Expand (actionOptions);
    }

    /// <summary>
    /// Checks the node's checkbox.
    /// </summary>
    public UnspecifiedPageObject Check ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = actionOptions ?? Opt.ContinueImmediately();
      ExecuteAction (new CheckAction (this, GetCheckboxScope()), actualCompletionDetector);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Unchecks the node's checkbox.
    /// </summary>
    public UnspecifiedPageObject Uncheck ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      var actualCompletionDetector = actionOptions ?? Opt.ContinueImmediately();
      ExecuteAction (new UncheckAction (this, GetCheckboxScope()), actualCompletionDetector);
      return UnspecifiedPage();
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the node.
    /// </summary>
    public TreeViewNodeControlObject Select ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ClickNode (actionOptions);
      return this;
    }

    /// <summary>
    /// Selects the node by clicking on it, returns the following page.
    /// </summary>
    public UnspecifiedPageObject Click ([CanBeNull] IWebTestActionOptions actionOptions = null)
    {
      ClickNode (actionOptions);
      return UnspecifiedPage();
    }

    private void ClickNode (IWebTestActionOptions actionOptions)
    {
      var actualCompletionDetector = MergeWithDefaultActionOptions (Scope, actionOptions);
      const string nodeClickScopeXpath = "./tbody/tr/td[a[contains(@onclick, 'TreeView_SelectNode')]][last()]/a[last()]";
      ExecuteAction (new ClickAction (this, Scope.FindXPath (nodeClickScopeXpath)), actualCompletionDetector);
    }

    private ElementScope GetCheckboxScope ()
    {
      const string xpath = "./tbody/tr/td[a[contains(@onclick, 'TreeView_SelectNode')]]/input[@type='checkbox']";
      return Scope.FindXPath (xpath);
    }

    /// <inheritdoc/>
    public IFluentControlObjectWithNodes<TreeViewNodeControlObject> GetNode ()
    {
      return new GetNodeImplementationForChildren (this);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemID", itemID);

      return GetNode().WithItemID (itemID);
    }

    /// <inheritdoc/>
    public TreeViewNodeControlObject GetNode (int oneBasedIndex)
    {
      return GetNode().WithIndex (oneBasedIndex);
    }

    private static ElementScope GetChildrenScope (ElementScope elementScope)
    {
      const string xpath = "./following-sibling::div[1]";
      return elementScope.FindXPath (xpath);
    }
  }
}