using System;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.Cloning
{
  // TODO: Change to use ordinary property setters => event handling easier
  ///// <summary>
  ///// Assists in cloning <see cref="DomainObject"/> instances.
  ///// </summary>
  //public class DomainObjectCloner
  //{
  //  /// <summary>
  //  /// Creates a new <see cref="DomainObject"/> instance of the same type and with the same property values as the given <paramref name="source"/>.
  //  /// Relations are not cloned, foreign key properties default to null.
  //  /// </summary>
  //  /// <typeparam name="T">The static <see cref="DomainObject"/> type to be cloned. Note that the actual (dynamic) type of the cloned object
  //  /// is the type defined by <paramref name="source"/>'s <see cref="ClassDefinition"/>.</typeparam>
  //  /// <param name="source">The <see cref="DomainObject"/> to be cloned.</param>
  //  /// <remarks>
  //  /// The clone is created in the current transaction. No constructor is called on the clone object, and no property or relation set events are raised.
  //  /// </remarks>
  //  public T CreateValueClone<T> (T source)
  //      where T: DomainObject
  //  {
  //    ClientTransaction sourceTransaction = source.ClientTransaction;
  //    ClientTransaction cloneTransaction = ClientTransactionScope.CurrentTransaction;
  //    DataContainer sourceDataContainer = source.GetDataContainerForTransaction (sourceTransaction);

  //    ObjectID cloneID = cloneTransaction.CreateNewObjectID (source.ID.ClassDefinition);
  //    DataContainer dataContainerClone = DataContainer.CreateNew (cloneID);
  //    dataContainerClone.AssumeSamePropertyValues (sourceDataContainer);
  //    ClearForeignKeys (dataContainerClone);
  //    cloneTransaction.RegisterNewDataContainer (dataContainerClone);
  //    return (T) RepositoryAccessor.NewObjectFromDataContainer (dataContainerClone);
  //  }

  //  private void ClearForeignKeys (DataContainer dataContainer)
  //  {
  //    foreach (PropertyValue propertyValue in dataContainer.PropertyValues)
  //    {
  //      if (propertyValue.IsRelationProperty)
  //        propertyValue.TakeOverCommittedData (null); // TODO: stop events!
  //    }
  //  }
  //}
}