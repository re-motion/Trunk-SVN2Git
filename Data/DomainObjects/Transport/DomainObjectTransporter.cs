// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Transport
{
  /// <summary>
  /// Collects domain objects to be transported to another system.
  /// </summary>
  public class DomainObjectTransporter
  {
    /// <summary>
    /// Loads the data transported from another system into a <see cref="TransportedDomainObjects"/> container using the <see cref="BinaryImportStrategy"/>.
    /// </summary>
    /// <param name="data">The transported data to be loaded.</param>
    /// <returns>A container holding the objects loaded from the given data.</returns>
    /// <exception cref="ObjectNotFoundException">A referenced related object is not part of the transported data and does not exist on the
    /// target system.</exception>
    /// <remarks>
    /// Given a <see cref="DomainObjectTransporter"/>, the binary data can be retrieved from <see cref="GetBinaryTransportData()"/>.
    /// </remarks>
    public static TransportedDomainObjects LoadTransportData (byte[] data)
    {
      BinaryImportStrategy strategy = BinaryImportStrategy.Instance;
      return LoadTransportData(data, strategy);
    }

    /// <summary>
    /// Loads the data transported from another system into a <see cref="TransportedDomainObjects"/> container.
    /// </summary>
    /// <param name="data">The transported data to be loaded.</param>
    /// <param name="strategy">The strategy to use when importing data. This must match the strategy being used with <see cref="GetBinaryTransportData(IExportStrategy)"/>.</param>
    /// <returns>A container holding the objects loaded from the given data.</returns>
    /// <exception cref="ObjectNotFoundException">A referenced related object is not part of the transported data and does not exist on the
    /// target system.</exception>
    /// <remarks>
    /// Given a <see cref="DomainObjectTransporter"/>, the binary data can be retrieved from <see cref="GetBinaryTransportData(IExportStrategy)"/>.
    /// </remarks>
    public static TransportedDomainObjects LoadTransportData (byte[] data, IImportStrategy strategy)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("data", data);
      ArgumentUtility.CheckNotNull ("strategy", strategy);
      return new DomainObjectImporter (data, strategy).GetImportedObjects ();
    }

    private readonly ClientTransaction _transportTransaction;
    private readonly Set<ObjectID> _transportedObjects = new Set<ObjectID>();

    public DomainObjectTransporter ()
    {
      _transportTransaction = ClientTransaction.CreateBindingTransaction();
      _transportTransaction.AddListener (new TransportTransactionListener (this));
    }

    /// <summary>
    /// Gets the IDs of the objects loaded into this transporter.
    /// </summary>
    /// <value>The IDs of the loaded objects.</value>
    public ReadOnlyCollection<ObjectID> ObjectIDs
    {
      get { return new ReadOnlyCollection<ObjectID> (_transportedObjects.ToArray()); }
    }

    /// <summary>
    /// Determines whether the specified <paramref name="objectID"/> has been loaded for transportation.
    /// </summary>
    /// <param name="objectID">The object ID to check.</param>
    /// <returns>
    /// True if the specified object ID has been loaded; otherwise, false.
    /// </returns>
    public bool IsLoaded (ObjectID objectID)
    {
      return _transportedObjects.Contains (objectID);
    }

    /// <summary>
    /// Loads a new instance of a domain object for transportation.
    /// </summary>
    /// <param name="type">The domain object type to instantiate. The type must have a parameterless constructor.</param>
    /// <returns>A new instance of <paramref name="type"/> prepared for transport.</returns>
    public DomainObject LoadNew (Type type)
    {
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        DomainObject domainObject = RepositoryAccessor.NewObject (type).With();
        Load (domainObject.ID);
        return domainObject;
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> into the transporter.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object to load.</param>
    /// <returns>The loaded object, whose properties can be manipulated before it is transported.</returns>
    /// <remarks>
    /// <para>
    /// This method loads exactly the object with the given ID, it will not load any related objects.
    /// </para>
    /// <para>
    /// If an object has the foreign key side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will still be transported. The related object must exist at the target system, otherwise an exception is thrown in
    /// <see cref="LoadTransportData"/>.
    /// </para>
    /// <para>
    /// If an object has the virtual side of a relationship and the related object is not loaded into this transporter, the relationship
    /// will not be transported. Its status after <see cref="LoadTransportData"/> depends on the objects at the target system. This
    /// also applies to the 1-side of a 1-to-n relationship because the n-side is the foreign key side.
    /// </para>
    /// </remarks>
    public DomainObject Load (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      DomainObject domainObject = _transportTransaction.GetObject (objectID, false);
      _transportedObjects.Add (objectID);
      return domainObject;
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly referenced by it into the transporter.
    /// Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="PropertyIndexer.GetAllRelatedObjects"/>
    public IEnumerable<DomainObject> LoadWithRelatedObjects (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return EnumerableUtility.ToArray (LazyLoadWithRelatedObjects(objectID));
    }

    private IEnumerable<DomainObject> LazyLoadWithRelatedObjects (ObjectID objectID)
    {
      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      yield return Load (sourceObject.ID);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        IEnumerable<DomainObject> relatedObjects = sourceObject.Properties.GetAllRelatedObjects ();
        foreach (DomainObject domainObject in relatedObjects)
          yield return Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction
      }
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly or indirectly referenced by it into the
    /// transporter. Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="DomainObjectGraphTraverser.GetFlattenedRelatedObjectGraph"/>
    public IEnumerable<DomainObject> LoadRecursive (ObjectID objectID)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      return LoadRecursive (objectID, FullGraphTraversalStrategy.Instance);
    }

    /// <summary>
    /// Loads the object with the specified <see cref="ObjectID"/> plus all objects directly or indirectly referenced by it into the
    /// transporter, as specified by the <see cref="IGraphTraversalStrategy"/>. Each object behaves as if it were loaded via <see cref="Load"/>.
    /// </summary>
    /// <param name="objectID">The <see cref="ObjectID"/> of the object which is to be loaded together with its related objects.</param>
    /// <param name="strategy">An <see cref="IGraphTraversalStrategy"/> instance defining which related object links to follow and which
    /// objects to include in the set of transported objects.</param>
    /// <returns>The loaded objects, whose properties can be manipulated before they are transported.</returns>
    /// <seealso cref="DomainObjectGraphTraverser.GetFlattenedRelatedObjectGraph"/>
    public IEnumerable<DomainObject> LoadRecursive (ObjectID objectID, IGraphTraversalStrategy strategy)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("strategy", strategy);

      DomainObject sourceObject = _transportTransaction.GetObject (objectID, false);
      using (_transportTransaction.EnterNonDiscardingScope ())
      {
        Set<DomainObject> graph = new DomainObjectGraphTraverser (sourceObject, strategy).GetFlattenedRelatedObjectGraph ();
        foreach (DomainObject domainObject in graph)
          Load (domainObject.ID); // explicitly call load rather than just implicitly loading it into the transaction for consistency
        return graph;
      }
    }

    /// <summary>
    /// Retrieves a loaded object so that it can be manipulated prior to it being transported.
    /// </summary>
    /// <param name="loadedObjectID">The object ID of the object to be retrieved.</param>
    /// <returns>A <see cref="DomainObject"/> representing an object to be transported. Properties of this object can be manipulated.</returns>
    public DomainObject GetTransportedObject (ObjectID loadedObjectID)
    {
      ArgumentUtility.CheckNotNull ("loadedObjectID", loadedObjectID);
      if (!IsLoaded (loadedObjectID))
      {
        string message = string.Format ("Object '{0}' cannot be retrieved, it hasn't been loaded yet. Load it first, then retrieve it for editing.",
            loadedObjectID);
        throw new ArgumentException (message, "loadedObjectID");
      }
      return _transportTransaction.GetObject (loadedObjectID, false);
    }

    /// <summary>
    /// Gets a the objects loaded into this transporter (including their contents) in a binary format for transport to another system using <see cref="BinaryExportStrategy"/>.
    /// At the target system, the data can be loaded via <see cref="LoadTransportData(byte[])"/>.
    /// </summary>
    /// <returns>The loaded objects in a binary format.</returns>
    public byte[] GetBinaryTransportData ()
    {
      return GetBinaryTransportData (BinaryExportStrategy.Instance);
    }

    /// <summary>
    /// Gets a the objects loaded into this transporter (including their contents) in a binary format for transport to another system.
    /// At the target system, the data can be loaded via <see cref="LoadTransportData(byte[],IImportStrategy)"/>.
    /// </summary>
    /// <param name="strategy">The strategy to be used for exporting data. This must match the strategy used with <see cref="LoadTransportData(byte[],IImportStrategy)"/>.</param>
    /// <returns>The loaded objects in a binary format.</returns>
    public byte[] GetBinaryTransportData (IExportStrategy strategy)
    {
      IEnumerable<DataContainer> transportedContainers = GetTransportedContainers();
      TransportItem[] transportItems = EnumerableUtility.ToArray (TransportItem.PackageDataContainers (transportedContainers));
      return strategy.Export (transportItems);
    }

    private IEnumerable<DataContainer> GetTransportedContainers ()
    {
      foreach (ObjectID id in _transportedObjects)
        yield return _transportTransaction.DataManager.DataContainerMap[id];
    }
  }
}
