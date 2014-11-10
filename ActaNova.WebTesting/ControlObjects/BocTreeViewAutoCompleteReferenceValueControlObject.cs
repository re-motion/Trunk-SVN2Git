using System;
using System.Collections.Generic;
using System.Linq;
using ActaNova.WebTesting.ControlObjects.Selectors;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlObjects;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace ActaNova.WebTesting.ControlObjects
{
  /// <summary>
  /// Control object representing the ActaNova tree-view auto complete reference value.
  /// </summary>
  public class BocTreeViewAutoCompleteReferenceValueControlObject : ActaNovaAutoCompleteReferenceValueControlObject
  {
    public BocTreeViewAutoCompleteReferenceValueControlObject ([NotNull] ControlObjectContext context)
        : base (context)
    {
    }

    public UnspecifiedPageObject Select ([NotNull] params string[] treeNodes)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      return Select (treeNodes.AsEnumerable());
    }

    public UnspecifiedPageObject Select ([NotNull] IEnumerable<string> treeNodes, [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("treeNodes", treeNodes);

      var dropDownButtonScope = Scope.FindChild ("DropDownButton");
      dropDownButtonScope.Click();

      var scope = GetParentScope();
      var treePopupTable =
          scope.GetControl (new SingleControlSelectionCommand<ActaNovaTreePopupTableControlObject> (new ActaNovaTreePopupTableSelector()));
      var node = treePopupTable.GetNode (treeNodes);
      return node.Click (completionDetection);
    }

    private ScopeControlObject GetParentScope ()
    {
      var scope = Scope.FindXPath ("../..");
      return new ScopeControlObject (Context.CloneForControl (scope));
    }
  }
}