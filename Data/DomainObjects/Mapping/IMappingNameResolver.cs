using System;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Mapping
{
  public interface IMappingNameResolver
  {
    /// <summary>
    /// Returns the mapping name for the given <paramref name="property"/>.
    /// </summary>
    /// <param name="property">The property whose mapping name should be retrieved.</param>
    /// <returns>The name of the given <paramref name="property"/> as used internally by the mapping.</returns>
    string GetPropertyName (PropertyInfo property);

    /// <summary>
    /// Returns the mapping name for a property with the given <paramref name="shortPropertyName"/> on the <paramref name="originalDeclaringType"/>.
    /// </summary>
    /// <param name="originalDeclaringType">The type on which the property was first declared.</param>
    /// <param name="shortPropertyName">The short property name of the property.</param>
    string GetPropertyName (Type originalDeclaringType, string shortPropertyName);

    /// <summary>
    /// Returns the property identified by the given mapping property name on the given type.
    /// </summary>
    /// <param name="concreteType">The type on which to search for the property. This can be the same type whose name is encoded in 
    /// <paramref name="propertyName"/> or a derived type or generic specialization.</param>
    /// <param name="propertyName">The long mapping property name of the property to be retrieved.</param>
    /// <returns>The <see cref="PropertyInfo"/> corresponding to the given mapping property.</returns>
    PropertyInfo GetProperty (Type concreteType, string propertyName);
  }
}