using System;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// The <see cref="InheritanceHierarchyFilter"/> can be used to get all leaf classes within a deifned set of types passed into the 
  /// constructor.
  /// </summary>
  public class InheritanceHierarchyFilter
  {
    private readonly Type[] _types;

    public InheritanceHierarchyFilter (Type[] types)
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("types", types);
      _types = types;
    }

    public Type[] GetLeafTypes ()
    {
      Set<Type> baseTypes = new Set<Type>();
      foreach (Type type in _types)
      {
        baseTypes.Add (type.BaseType);
        if (type.BaseType.IsGenericType)
          baseTypes.Add (type.BaseType.GetGenericTypeDefinition());
      }

      return Array.FindAll (_types, delegate (Type type) { return !baseTypes.Contains (type); });
    }
  }
}