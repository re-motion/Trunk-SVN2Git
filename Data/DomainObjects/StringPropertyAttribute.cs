using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="StringPropertyAttribute"/> to properties of type <see cref="string"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class StringPropertyAttribute: NullableLengthConstrainedPropertyAttribute
  {
    public StringPropertyAttribute()
    {
    }
  }
}