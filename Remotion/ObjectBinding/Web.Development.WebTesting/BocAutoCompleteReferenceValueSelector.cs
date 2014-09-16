using System;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting
{
  /// <summary>
  /// Control object selector for <see cref="BocAutoCompleteReferenceValueControlObject"/>.
  /// </summary>
  public class BocAutoCompleteReferenceValueSelector : BocSelectorBase<BocAutoCompleteReferenceValueControlObject>
  {
    public override BocAutoCompleteReferenceValueControlObject FindTypedControl (
        TestObjectContext context,
        BocControlSelectionParameters selectionParameters)
    {
      var scope = FindScope (context, selectionParameters);
      return new BocAutoCompleteReferenceValueControlObject (scope.Id, context.CloneForScope (scope));
    }
  }
}