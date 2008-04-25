using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  /// <summary>
  /// Declares a relation as bidirectional.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BidirectionalRelationAttribute: Attribute, IMappingAttribute
  {
    private readonly string _oppositeProperty;

    /// <summary>
    /// Initializes a new instance of the <see cref="BidirectionalRelationAttribute"/> class with the name of the oppsite property.
    /// </summary>
    /// <param name="oppositeProperty">The name of the opposite property. Must not be <see langword="null" /> or empty.</param>
    public BidirectionalRelationAttribute (string oppositeProperty)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("oppositeProperty", oppositeProperty);

      _oppositeProperty = oppositeProperty;
    }

    public string OppositeProperty
    {
      get { return _oppositeProperty; }
    }
  }
}