// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception
{
  [TestFixture]
  public class SerializationHelperTest : ClientTransactionBaseTest
  {
    private SerializationInfo _info;
    private StreamingContext _context;
    
    private SerializableClass _serializableInstance;
    private SerializableClassImplementingISerializable _serializableInstanceImplementingISerializable;
    private SerializableClassImplementingISerializableNotCallingBaseCtor _serializableInstanceImplementingISerializableNotCallingBaseCtor;
    private SerializableClassImplementingISerializableNotCallingBaseGetObjectData _serializableInstanceImplementingISerializableNotCallingGetObjectData;

    public InterceptedDomainObjectTypeFactory Factory
    {
      get { return SetUpFixture.Factory; }
    }

    public override void SetUp ()
    {
      base.SetUp ();
      
      _info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter ());
      _context = new StreamingContext ();

      _serializableInstance = CreateInstance<SerializableClass>();
      _serializableInstanceImplementingISerializable = CreateInstance<SerializableClassImplementingISerializable> ();
      _serializableInstanceImplementingISerializableNotCallingBaseCtor = CreateInstance<SerializableClassImplementingISerializableNotCallingBaseCtor> ();
      _serializableInstanceImplementingISerializableNotCallingGetObjectData = CreateInstance<SerializableClassImplementingISerializableNotCallingBaseGetObjectData> ();
    }

    private T CreateInstance<T> ()
    {
      Type concreteType = Factory.GetConcreteDomainObjectType (typeof (T));
      var constructorLookupInfo = new DomainObjectConstructorLookupInfo (typeof (T), concreteType, BindingFlags.Public | BindingFlags.Instance);
      return ObjectLifetimeAgentTestHelper.CallWithInitializationContext (
          TestableClientTransaction,
          new ObjectID (typeof (T), Guid.NewGuid ()), 
          () => (T) ParamList.Empty.InvokeConstructor (constructorLookupInfo));
    }

    [Test]
    public void GetObjectDataForGeneratedTypesSetsType ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);

      Assert.AreEqual (typeof (SerializationHelper).Assembly.FullName, _info.AssemblyName);
      Assert.AreEqual (typeof (SerializationHelper).FullName, _info.FullTypeName);
    }

    [Test]
    public void GetObjectDataForGeneratedTypesAddsBaseTypeInfo ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);

      Assert.AreEqual (typeof (SerializableClass).AssemblyQualifiedName, _info.GetString ("PublicDomainObjectType.AssemblyQualifiedName"));
    }

    [Test]
    public void GetObjectDataForGeneratedTypesAddsMembersIfToldTo ()
    {
      _serializableInstance.I = 0x400dd00d;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      var members = (object[]) _info.GetValue ("baseMemberValues", typeof (object[]));
      Assert.IsNotNull (members);
      Assert.Contains (0x400dd00d, members);
    }

    [Test]
    public void GetObjectDataForGeneratedTypesDoesntAddMembersIfToldTo ()
    {
      _serializableInstance.I = 0x400dd00d;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, false);
      var members = (object[]) _info.GetValue ("baseMemberValues", typeof (object[]));
      Assert.IsNull (members);
    }

    [Test]
    public void CtorCreatesUninitializedRealObjectOfRightType ()
    {
      _serializableInstance.I = 13;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      var helper = new SerializationHelper (_info, _context);
      var realObject = (SerializableClass) helper.GetRealObject (_context);
      Assert.IsNotNull (realObject);
      Assert.AreEqual (13, _serializableInstance.I);
      Assert.AreNotEqual (13, realObject.I);
      Assert.IsTrue (Factory.WasCreatedByFactory (((object)realObject).GetType()));
      Assert.AreSame (typeof (SerializableClass), ((object) realObject).GetType ().BaseType);
    }

    [Test]
    public void CtorUsesISerializableBaseCtorIfNecessary ()
    {
      Assert.IsFalse (_serializableInstanceImplementingISerializable.ISerializableCtorCalled);
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializable, false);
      _serializableInstanceImplementingISerializable.GetObjectData (_info, _context);
      var helper = new SerializationHelper (_info, _context);
      var realObject = (SerializableClassImplementingISerializable) helper.GetRealObject (_context);
      Assert.IsTrue (realObject.ISerializableCtorCalled);
    }

    [Test]
    public void OnDeserializationFixesUninitializedRealObjectIfNecessary ()
    {
      _serializableInstance.I = 13;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      var helper = new SerializationHelper (_info, _context);
      var realObject = (SerializableClass) helper.GetRealObject (_context);
      helper.OnDeserialization (null);

      Assert.AreEqual (13, realObject.I);
    }

    [Test]
    public void OnDeserializationDoesNothingIfObjectImplementsISerializableCorrectly ()
    {
      _serializableInstanceImplementingISerializable.I = 4;
      Assert.IsFalse (_serializableInstanceImplementingISerializable.ISerializableCtorCalled);
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializable, false);
      _serializableInstanceImplementingISerializable.GetObjectData (_info, _context);

      var helper = new SerializationHelper (_info, _context);
      var realObject = (SerializableClassImplementingISerializable) helper.GetRealObject (_context);

      Assert.AreEqual (4, realObject.I);

      helper.OnDeserialization (null);
      Assert.IsTrue (realObject.ISerializableCtorCalled);
      Assert.AreEqual (4, realObject.I);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The deserialization constructor on type "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.SerializableClassImplementingISerializableNotCallingBaseCtor "
        + "did not call DomainObject's base deserialization constructor.")]
    public void OnDeserializationThrowsIfISerializableBaseCtorNotCalled ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializableNotCallingBaseCtor, false);
      _serializableInstanceImplementingISerializableNotCallingBaseCtor.GetObjectData (_info, _context);
      ObjectLifetimeAgentTestHelper.CallWithInitializationContext (
          TestableClientTransaction, DomainObjectIDs.Order1, () => new SerializationHelper (_info, _context).OnDeserialization (null));
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The GetObjectData method on type "
        + "Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.Interception.TestDomain.SerializableClassImplementingISerializableNotCallingBaseGetObjectData"
        +" did not call DomainObject's BaseGetObjectData method.")]
    public void DeserializationCtorThrowsIfBaseGetObjectDataNotCalled ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializableNotCallingGetObjectData, false);
      _serializableInstanceImplementingISerializableNotCallingGetObjectData.GetObjectData (_info, _context);
      try
      {
        new SerializationHelper (_info, _context);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException), ExpectedMessage = "This method should never be called.")]
    public void GetObjectDataThrows ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      var helper = new SerializationHelper (_info, _context);
      helper.GetObjectData (_info, _context);
    }

    [Test]
    public void DeserializerUsesConfigFactory ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      var helper = new SerializationHelper (_info, _context);
      Assert.AreSame (InterceptedDomainObjectCreator.Instance.Factory.GetConcreteDomainObjectType (typeof (SerializableClass)),
          helper.GetRealObject (_context).GetType ());
    }
  }
}
