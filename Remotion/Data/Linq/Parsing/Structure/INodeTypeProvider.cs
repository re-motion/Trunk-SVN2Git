using System;
using System.Linq.Expressions;
using System.Reflection;
using Remotion.Data.Linq.Parsing.Structure.IntermediateModel;

namespace Remotion.Data.Linq.Parsing.Structure
{
  /// <summary>
  /// Provides a common interface for classes mapping a <see cref="MethodInfo"/> to the respective <see cref="IExpressionNode"/>
  /// type. Implementations are used by <see cref="ExpressionTreeParser"/> when a <see cref="MethodCallExpression"/> is encountered to 
  /// instantiate the right <see cref="IExpressionNode"/> for the given method.
  /// </summary>
  public interface INodeTypeProvider
  {
    /// <summary>
    /// Determines whether a node type for the given <see cref="MethodInfo"/> can be returned by this 
    /// <see cref="INodeTypeProvider"/>.
    /// </summary>
    bool IsRegistered (MethodInfo method);

    /// <summary>
    /// Gets the type of <see cref="IExpressionNode"/> that matches the given <paramref name="method"/>, returning <see langword="null" /> 
    /// if none can be found.
    /// </summary>
    Type GetNodeType (MethodInfo method);
  }
}