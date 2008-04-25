using System;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies that only the date component of a <see cref="DateTime"/> property should be used by the <b>ObjectBinding</b> infrastructure.
  /// </summary>
  [AttributeUsage (AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class DatePropertyAttribute : Attribute
  {
    public DatePropertyAttribute ()
    {
    }
  }
}