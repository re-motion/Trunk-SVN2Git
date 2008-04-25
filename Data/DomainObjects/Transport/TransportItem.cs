using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Represents a <see cref="DomainObject"/> instance when it is to be transported by <see cref="DomainObjectTransporter"/>.
  /// </summary>
  /// <remarks>
  /// Values of this type hold the data stored by a <see cref="DomainObject"/> in the context of a certain <see cref="ClientTransaction"/> without
  /// having a dependency on the transaction infrastructure, which makes it easier to manipulate, export, and import them.
  /// </remarks>
  public struct TransportItem
  {
    /// <summary>
    /// Packages the data held by a <see cref="DataContainer"/> into a <see cref="TransportItem"/>. This method is only meant for internal purposes.
    /// </summary>
    /// <param name="container">The container to package.</param>
    /// <returns>A <see cref="TransportItem"/> holding the same data as <paramref name="container"/>.</returns>
    public static TransportItem PackageDataContainer (DataContainer container)
    {
      ArgumentUtility.CheckNotNull ("container", container);

      TransportItem item = new TransportItem (container.ID);
      foreach (PropertyValue propertyValue in container.PropertyValues)
        item.Properties.Add (propertyValue.Name, propertyValue.Value);
      return item;
    }

    /// <summary>
    /// Packages the data held by a stream of <see cref="DataContainer"/> instances into a stream of <see cref="TransportItem"/> values. This method 
    /// is only meant for internal purposes.
    /// </summary>
    /// <param name="containers">The containers to package.</param>
    /// <returns>A stream of <see cref="TransportItem"/> instances holding the same data as <paramref name="containers"/>.</returns>
    public static IEnumerable<TransportItem> PackageDataContainers (IEnumerable<DataContainer> containers)
    {
      ArgumentUtility.CheckNotNull ("containers", containers);

      foreach (DataContainer container in containers)
        yield return PackageDataContainer (container);
    }

    private readonly ObjectID _id;
    private readonly Dictionary<string, object> _properties;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransportItem"/> class.
    /// </summary>
    /// <param name="id">The id of the <see cref="DomainObject"/> represented by this item.</param>
    public TransportItem (ObjectID id)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      _id = id;
      _properties = new Dictionary<string, object>();
    }

    internal TransportItem (ObjectID id, Dictionary<string, object> properties)
    {
      ArgumentUtility.CheckNotNull ("id", id);
      ArgumentUtility.CheckNotNull ("properties", properties);

      _id = id;
      _properties = properties;
    }

    /// <summary>
    /// Gets the <see cref="ObjectID"/> of the <see cref="DomainObject"/> represented by this item.
    /// </summary>
    /// <value>The <see cref="ObjectID"/> of this item.</value>
    public ObjectID ID
    {
      get { return _id; }
    }

    /// <summary>
    /// Gets the properties to be transported by this item.
    /// </summary>
    /// <value>The properties to be transported.</value>
    public Dictionary<string, object> Properties
    {
      get { return _properties; }
    }
  }
}