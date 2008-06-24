using System;
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  /// <summary>
  /// Resolves <see cref="PropertyInfo"/> objects into property names and the other way around.
  /// </summary>
  public class ReflectionBasedNameResolver : IMappingNameResolver
  {
    /// <summary>
    /// Returns the mapping name for the given <paramref name="property"/>.
    /// </summary>
    /// <param name="property">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="property"/> as used internally by the mapping.</returns>
    public string GetPropertyName (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);
      Type originalDeclaringType = Remotion.Utilities.ReflectionUtility.GetOriginalDeclaringType (property);
      return GetPropertyName(originalDeclaringType, property.Name);
    }

    /// <summary>
    /// Returns the mapping name for a property with the given <paramref name="shortPropertyName"/> on the <paramref name="originalDeclaringType"/>.
    /// </summary>
    /// <param name="originalDeclaringType">The type on which the property was first declared.</param>
    /// <param name="shortPropertyName">The short property name of the property.</param>
    public string GetPropertyName (Type originalDeclaringType, string shortPropertyName)
    {
      ArgumentUtility.CheckNotNull ("originalDeclaringType", originalDeclaringType);
      ArgumentUtility.CheckNotNull ("shortPropertyName", shortPropertyName);
      if (originalDeclaringType.IsGenericType && !originalDeclaringType.IsGenericTypeDefinition)
        originalDeclaringType = originalDeclaringType.GetGenericTypeDefinition();

      return originalDeclaringType.FullName + "." + shortPropertyName;
    }

    /// <summary>
    /// Returns the property identified by the given mapping property name on the given type.
    /// </summary>
    /// <param name="concreteType">The type on which to search for the property. This can be the same type whose name is encoded in 
    /// <paramref name="propertyName"/> or a derived type or generic specialization.</param>
    /// <param name="propertyName">The long mapping property name of the property to be retrieved.</param>
    /// <returns>The <see cref="PropertyInfo"/> corresponding to the given mapping property.</returns>
    public PropertyInfo GetProperty (Type concreteType, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      int shortPropertyNameStart = propertyName.LastIndexOf ('.');
      string shortPropertyName = propertyName.Substring (shortPropertyNameStart + 1);

      return concreteType.GetProperty (shortPropertyName, PropertyFinderBase.PropertyBindingFlags);
    }
  }
}