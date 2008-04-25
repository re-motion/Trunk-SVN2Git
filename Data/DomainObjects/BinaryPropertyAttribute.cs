using System;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Apply the <see cref="BinaryPropertyAttribute"/> to properties of type <see cref="byte"/> array.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public class BinaryPropertyAttribute : NullableLengthConstrainedPropertyAttribute
  {
    public BinaryPropertyAttribute ()
    {
    }
  }
}