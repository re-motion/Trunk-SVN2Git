/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Mixins;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Persistence
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
