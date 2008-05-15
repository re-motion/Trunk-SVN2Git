using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  /// <summary>
  /// Assists in cloning <see cref="DomainObject"/> instances.
  /// </summary>
  public class DomainObjectCloner
  {
    /// <summary>
    /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
    /// Relations are not cloned, foreign key properties default to null.
    /// </summary>
    /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. Note that the actual (dynamic) type of the cloned object
    /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
    /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
    /// <remarks>
    /// The clone is created in the current transaction. No constructor is called on the clone object; property or relation get and set events are 
    /// raised as needed by the cloner.
    /// </remarks>
    public T CreateValueClone<T> (T source)
        where T : DomainObject
    {
      ObjectID sourceID = source.ID;
      ClientTransaction sourceTransaction = source.GetNonNullClientTransaction();
      ClientTransaction cloneTransaction = ClientTransactionScope.CurrentTransaction;

      DataContainer cloneDataContainer = cloneTransaction.CreateNewDataContainer (sourceID.ClassDefinition.ClassType);
      T clone = (T) RepositoryAccessor.NewObjectFromDataContainer (cloneDataContainer);

      IEnumerable<PropertyAccessor> cloneProperties = EnumerableUtility.Where (clone.Properties, 
          delegate (PropertyAccessor property) { return property.Kind == PropertyKind.PropertyValue; });
      CopyProperties (cloneProperties, cloneTransaction, source.Properties, sourceTransaction);
      return clone;
    }

    private void CopyProperties (IEnumerable<PropertyAccessor> cloneProperties, ClientTransaction cloneTransaction, PropertyIndexer sourceProperties, ClientTransaction sourceTransaction)
    {
      foreach (PropertyAccessor cloneProperty in cloneProperties)
      {
        PropertyAccessor sourceProperty = sourceProperties[cloneProperty.PropertyIdentifier];
        object sourceValue = sourceProperty.GetValueWithoutTypeCheckTx (sourceTransaction);
        cloneProperty.SetValueWithoutTypeCheckTx (cloneTransaction, sourceValue);
      }
    }
  }
}