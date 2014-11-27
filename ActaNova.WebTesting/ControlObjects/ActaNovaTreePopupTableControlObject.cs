using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing an ActaNova popup table control, filled with a tree.
  /// </summary>
  public class ActaNovaTreePopupTableControlObject : WebFormsControlObject
  {
    public ActaNovaTreePopupTableControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    /// <summary>
    /// Retrieves a node in the popup tree given by its path (nodes are referenced by display text) <paramref name="treeNodes"/>.
    /// </summary>
    public ActaNovaTreePopupTableNodeControlObject GetNode ([NotNull] params string[] treeNodes)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      return GetNode (treeNodes.AsEnumerable());
    }

    /// <summary>
    /// Retrieves a node in the popup tree given by its path (nodes are referenced by display text) <paramref name="treeNodes"/>.
    /// </summary>
    public ActaNovaTreePopupTableNodeControlObject GetNode ([NotNull] IEnumerable<string> treeNodes)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      var treeNodesAsList = treeNodes.ToList();

      var treeNodesToExpand = treeNodesAsList.Take (treeNodesAsList.Count - 1);
      ExpandTreeNodes (treeNodesToExpand);

      var lastTreeNode = treeNodesAsList.Last();
      var lastTreeNodeScope = Scope.FindXPath (string.Format (".//div[@class='NodeItem' and .//span[normalize-space(.)='{0}']]", lastTreeNode));
      return new ActaNovaTreePopupTableNodeControlObject (Context.CloneForControl (lastTreeNodeScope));
    }

    private void ExpandTreeNodes (IEnumerable<string> treeNodesToExpand)
    {
      foreach (var node in treeNodesToExpand)
      {
        var nodeExpandLinkScope =
            Scope.FindXPath (string.Format (".//a[@class='NodeLink' and following-sibling::a//span[normalize-space(.)='{0}']]", node));
        if (nodeExpandLinkScope["onclick"].Contains ("collapse"))
          continue;

        nodeExpandLinkScope.FocusClick();
      }
    }
  }
}