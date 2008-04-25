using System;

namespace Remotion.Data.DomainObjects.Web.ExecutionEngine
{
/// <summary>
/// Indicates the behavior of a <see cref="WxeTransactedFunction"/>.
/// </summary>
public enum WxeTransactionMode
{
  /// <summary>Create a new transaction.</summary>
  CreateRoot,
  /// <summary>Create a new child transaction.</summary>
  CreateChildIfParent,
  /// <summary>Never create a transaction.</summary>
  None
}
}
