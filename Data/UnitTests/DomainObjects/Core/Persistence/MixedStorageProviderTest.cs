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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence
{
  [TestFixture]
  public class MixedStorageProviderTest
  {
    [Test]
    public void StorageProvidersCanBeMixed ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        Assert.IsNotNull (Mixin.Get<StorageProviderWithFixedGuidMixin> (provider));
      }
    }

    [Test]
    public void MixinsCanOverrideStorageProviderMethods ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        ObjectID id1 = provider.CreateNewObjectID (orderDefinition);
        ObjectID id2 = provider.CreateNewObjectID (orderDefinition);
        Assert.AreEqual (id1, id2);
      }
    }

    [Test]
    public void MixinsCanIntroduceStorageProviderInterfaces ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (StorageProvider)).Clear().AddMixins (typeof (StorageProviderWithFixedGuidMixin)).EnterScope())
      {
        ClassDefinition orderDefinition = MappingConfiguration.Current.ClassDefinitions[typeof (Order)];
        StorageProvider provider = new StorageProviderManager ()[orderDefinition.StorageProviderID];
        
        Guid fixedGuid = Guid.NewGuid ();
        ((IStorageProviderWithFixedGuid) provider).FixedGuid = fixedGuid;

        ObjectID id = provider.CreateNewObjectID (orderDefinition);
        Assert.AreEqual (fixedGuid, id.Value);
      }
    }
  }
}
