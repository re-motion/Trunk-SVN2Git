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
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Mixins.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains
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
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
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
