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
using Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains.SampleTypes;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.MixedDomains
{
  [TestFixture]
  public class DomainObjectMixinBaseClassTest : ClientTransactionBaseTest
  {
    private ClassWithAllDataTypes _loadedClassWithAllDataTypes;
    private ClassWithAllDataTypes _newClassWithAllDataTypes;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _loadedClassWithAllDataTypesMixin;
    private MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes> _newClassWithAllDataTypesMixin;

    public override void SetUp ()
    {
      base.SetUp ();
      _loadedClassWithAllDataTypes = ClassWithAllDataTypes.GetObject (DomainObjectIDs.ClassWithAllDataTypes1);
      _newClassWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
      _loadedClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (_loadedClassWithAllDataTypes);
      _newClassWithAllDataTypesMixin = Mixin.Get<MixinWithAccessToDomainObjectProperties<ClassWithAllDataTypes>> (_newClassWithAllDataTypes);
    }

    [Test]
    public void MixinIsApplied ()
    {
      Assert.IsNotNull (_loadedClassWithAllDataTypesMixin);
      Assert.IsNotNull (_newClassWithAllDataTypesMixin);
    }

    [Test]
    [ExpectedException (typeof (ValidationException))]
    public void InvalidMixinConfiguration ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (ClassWithAllDataTypes)).Clear().AddMixins (typeof (MixinWithAccessToDomainObjectProperties<Official>)).EnterScope())
      {
        TargetClassDefinitionUtility.GetActiveConfiguration (typeof (ClassWithAllDataTypes));
      }
    }

    [Test]
    public void This ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes, _loadedClassWithAllDataTypesMixin.This);
      Assert.AreSame (_newClassWithAllDataTypes, _newClassWithAllDataTypesMixin.This);
    }

    [Test]
    public void ID ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes.ID, _loadedClassWithAllDataTypesMixin.ID);
      Assert.AreSame (_newClassWithAllDataTypes.ID, _newClassWithAllDataTypesMixin.ID);
    }

    [Test]
    public void GetPublicDomainObjectType ()
    {
      Assert.AreSame (_loadedClassWithAllDataTypes.GetPublicDomainObjectType (), _loadedClassWithAllDataTypesMixin.GetPublicDomainObjectType ());
      Assert.AreSame (_newClassWithAllDataTypes.GetPublicDomainObjectType (), _newClassWithAllDataTypesMixin.GetPublicDomainObjectType ());
    }

    [Test]
    public void State ()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);
      Assert.AreEqual (_newClassWithAllDataTypes.State, _newClassWithAllDataTypesMixin.State);

      ++_loadedClassWithAllDataTypes.Int32Property;
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);

      _loadedClassWithAllDataTypes.Delete ();
      Assert.AreEqual (_loadedClassWithAllDataTypes.State, _loadedClassWithAllDataTypesMixin.State);
    }

    [Test]
    public void IsDiscarded()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.IsDiscarded, _loadedClassWithAllDataTypesMixin.IsDiscarded);
      Assert.AreEqual (_newClassWithAllDataTypes.IsDiscarded, _newClassWithAllDataTypesMixin.IsDiscarded);

      _newClassWithAllDataTypes.Delete ();

      Assert.AreEqual (_newClassWithAllDataTypes.IsDiscarded, _newClassWithAllDataTypesMixin.IsDiscarded);
    }

    [Test]
    public void Properties ()
    {
      Assert.AreEqual (_loadedClassWithAllDataTypes.Properties, _loadedClassWithAllDataTypesMixin.Properties);
      Assert.AreEqual (_newClassWithAllDataTypes.Properties, _newClassWithAllDataTypesMixin.Properties);
    }

    [Test]
    public void OnDomainObjectCreated ()
    {
      Assert.IsFalse (_loadedClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled);
      Assert.IsTrue (_newClassWithAllDataTypesMixin.OnDomainObjectCreatedCalled);
    }

    [Test]
    public void OnDomainObjectLoaded ()
    {
      Assert.IsTrue (_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.WholeDomainObjectInitialized, _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode);

      _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled = false;
      using (ClientTransaction.NewRootTransaction ().EnterDiscardingScope ())
      {
        ClientTransaction.Current.EnlistDomainObject (_loadedClassWithAllDataTypes);
        ++_loadedClassWithAllDataTypes.Int32Property;
      }

      Assert.IsTrue (_loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
      Assert.AreEqual (LoadMode.DataContainerLoadedOnly, _loadedClassWithAllDataTypesMixin.OnDomainObjectLoadedLoadMode);

      Assert.IsFalse (_newClassWithAllDataTypesMixin.OnDomainObjectLoadedCalled);
    }

    [Test]
    public void Serializable ()
    {
      Serializer.SerializeAndDeserialize (_loadedClassWithAllDataTypesMixin);
      // no exception
    }
  }
}
