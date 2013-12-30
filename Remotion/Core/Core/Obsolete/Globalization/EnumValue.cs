using System;

// ReSharper disable once CheckNamespace
namespace Remotion.Utilities
{
  [Obsolete ("Dummy declaration for DependDB.", true)]
  internal struct EnumValue
  {
    public readonly Enum Value;

    public readonly string Description;

    public long NumericValue
    {
      get { return Convert.ToInt64 (Value); }
    }

    public EnumValue (Enum value, string description)
    {
      Value = value;
      Description = description;
    }
  }
}