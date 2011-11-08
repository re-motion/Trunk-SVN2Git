// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Reflection;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.VirtualObjectEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.InvalidObjects;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Creates all parts necessary to construct a <see cref="ClientTransaction"/> with root-transaction semantics.
  /// </summary>
  [Serializable]
  public class RootClientTransactionComponentFactory : ClientTransactionComponentFactoryBase
  {
    public static RootClientTransactionComponentFactory Create()
    {
      return ObjectFactory.Create<RootClientTransactionComponentFactory> (true, ParamList.Empty);
    }

    protected RootClientTransactionComponentFactory ()
    {
    }

    public override ClientTransaction GetParentTransaction (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return null;
    }

    public override Dictionary<Enum, object> CreateApplicationData (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new Dictionary<Enum, object> ();
    }

    public override IInvalidDomainObjectManager CreateInvalidDomainObjectManager (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return new RootInvalidDomainObjectManager ();
    }

    public override IPersistenceStrategy CreatePersistenceStrategy (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      return ObjectFactory.Create<RootPersistenceStrategy> (true, ParamList.Create (constructedTransaction.ID));
    }

    public override ClientTransactionExtensionCollection CreateExtensionCollection (ClientTransaction constructedTransaction)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);

      var extensions = base.CreateExtensionCollection (constructedTransaction);
      extensions.Insert (0, new CommitValidationClientTransactionExtension (tx => new MandatoryRelationValidator()));
      return extensions;
    }

    public override Func<ClientTransaction, ClientTransaction> CreateCloneFactory ()
    {
      return templateTransaction => (ClientTransaction) TypesafeActivator
          .CreateInstance (templateTransaction.GetType (), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
          .With (this);
    }

    protected override IRelationEndPointManager CreateRelationEndPointManager (
        ClientTransaction constructedTransaction,
        IRelationEndPointProvider endPointProvider,
        ILazyLoader lazyLoader)
    {
      ArgumentUtility.CheckNotNull ("constructedTransaction", constructedTransaction);
      ArgumentUtility.CheckNotNull ("endPointProvider", endPointProvider);
      ArgumentUtility.CheckNotNull ("lazyLoader", lazyLoader);

      var endPointChangeDetectionStrategy = new RootCollectionEndPointChangeDetectionStrategy();
      var collectionEndPointDataKeeperFactory = new CollectionEndPointDataKeeperFactory (constructedTransaction, endPointChangeDetectionStrategy);
      var virtualObjectEndPointDataKeeperFactory = new VirtualObjectEndPointDataKeeperFactory (constructedTransaction);

      var relationEndPointFactory = new RelationEndPointFactory (
          constructedTransaction,
          endPointProvider,
          lazyLoader,
          virtualObjectEndPointDataKeeperFactory,
          collectionEndPointDataKeeperFactory);
      var relationEndPointRegistrationAgent = new RootRelationEndPointRegistrationAgent (endPointProvider);
      return new RelationEndPointManager (constructedTransaction, lazyLoader, relationEndPointFactory, relationEndPointRegistrationAgent);
    }
  }
}