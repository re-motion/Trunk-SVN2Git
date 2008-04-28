using System;
using Remotion.Implementation;

namespace Remotion
{
  /// <summary>
  ///   Supplies an identifier that should remain constant even accross refactorings. Can be applied to reference types, properties and fields.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class | AttributeTargets.Property |AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class PermanentGuidAttribute : Attribute
  {
    private readonly Guid _value;

    /// <summary>
    ///   Initializes a new instance of the <see cref="PermanentGuidAttribute"/> class.
    /// </summary>
    /// <param name="value"> The <see cref="String"/> representation of a <see cref="Guid"/>. </param>
    public PermanentGuidAttribute (string value)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("value", value);

      _value = new Guid (value);
    }

    /// <summary>
    ///   Gets the <see cref="Guid"/> supplied during initialization.
    /// </summary>
    public Guid Value
    {
      get { return _value; }
    }
  }
}