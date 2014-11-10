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
  public class ActaNovaTreePopupTableControlObject : RemotionControlObject
  {
    public ActaNovaTreePopupTableControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public ActaNovaTreePopupTableNodeControlObject GetNode ([NotNull] params string[] treeNodes)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      return GetNode (treeNodes.AsEnumerable());
    }

    public ActaNovaTreePopupTableNodeControlObject GetNode (
        [NotNull] IEnumerable<string> treeNodes,
        [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      var treeNodesAsList = treeNodes.ToList();

      var treeNodesToExpand = treeNodesAsList.Take (treeNodesAsList.Count - 1);
      ExpandTreeNodes(treeNodesToExpand);

      var lastTreeNode = treeNodesAsList.Last();
      var lastTreeNodeScope = Scope.FindXPath (string.Format (".//div[@class='NodeItem' and contains(a, '{0}')]", lastTreeNode));
      return new ActaNovaTreePopupTableNodeControlObject (Context.CloneForControl (lastTreeNodeScope));
    }

    private void ExpandTreeNodes (IEnumerable<string> treeNodesToExpand)
    {
      foreach (var node in treeNodesToExpand)
      {
        var nodeExpandLink = Scope.FindXPath (string.Format (".//a[@class='NodeLink' and contains(following-sibling::a, '{0}')]", node));
        if (nodeExpandLink["onclick"].Contains ("collapse"))
          continue;

        nodeExpandLink.FocusClick();
      }
    }
  }
}