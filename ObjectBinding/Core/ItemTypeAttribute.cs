using System;
using System.Collections;
using Remotion.Utilities;

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// Specifies the type of items for properties returning an <see cref="IList"/>.
  /// </summary>
  [AttributeUsage (AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
  public sealed class ItemTypeAttribute : Attribute
  {
    private Type _itemType;

    /// <summary>
    /// Instantiates a new object.
    /// </summary>
    /// <param name="itemType">The type of items returned by the property. Must not be <see langword="null"/>.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="itemType"/> is <see langword="null"/>.</exception>
    public ItemTypeAttribute (Type itemType)
    {
      ArgumentUtility.CheckNotNull ("itemType", itemType);

      _itemType = itemType;
    }

    /// <summary>
    /// Gets the type of items returned by the property.
    /// </summary>
    public Type ItemType
    {
      get { return _itemType; }
    }
  }
}