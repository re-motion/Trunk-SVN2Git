using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using System.Runtime.Serialization;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class SerializationHelperTest : ClientTransactionBaseTest
  {
    [Serializable]
    [DBTable]
    public class SerializableClass : DomainObject
    {
      private int _i;

      [StorageClassNone]
      public int I
      {
        get { return _i; }
        set { _i = value; }
      }
    }

    [Serializable]
    [DBTable]
    public class SerializableClassImplementingISerializable : DomainObject, ISerializable
    {
      public bool ISerializableCtorCalled = false;
      public int I;

      public SerializableClassImplementingISerializable ()
      {
      }

      protected SerializableClassImplementingISerializable (SerializationInfo info, StreamingContext context)
          : base (info, context)
      {
        ISerializableCtorCalled = true;
        I = info.GetInt32 ("I");
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        info.AddValue ("I", I);
        BaseGetObjectData (info, context);
      }
    }

    [Serializable]
    [DBTable]
    public class SerializableClassImplementingISerializableNotCallingBaseCtor : DomainObject, ISerializable
    {
      public SerializableClassImplementingISerializableNotCallingBaseCtor ()
      {
      }

      protected SerializableClassImplementingISerializableNotCallingBaseCtor (SerializationInfo info, StreamingContext context)
      {
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
        BaseGetObjectData (info, context);
      }
    }

    [Serializable]
    [DBTable]
    public class SerializableClassImplementingISerializableNotCallingBaseGetObjectData : DomainObject, ISerializable
    {
      public SerializableClassImplementingISerializableNotCallingBaseGetObjectData ()
      {
      }

      protected SerializableClassImplementingISerializableNotCallingBaseGetObjectData (SerializationInfo info, StreamingContext context)
        : base (info, context)
      {
      }

      public void GetObjectData (SerializationInfo info, StreamingContext context)
      {
      }
    }

    private SerializationInfo _info;
    private StreamingContext _context;
    private InterceptedDomainObjectFactory _factory;
    private SerializableClass _serializableInstance;
    private SerializableClassImplementingISerializable _serializableInstanceImplementingISerializable;
    private SerializableClassImplementingISerializableNotCallingBaseCtor _serializableInstanceImplementingISerializableNotCallingBaseCtor;
    private SerializableClassImplementingISerializableNotCallingBaseGetObjectData _serializableInstanceImplementingISerializableNotCallingGetObjectData;

    public override void SetUp ()
    {
      base.SetUp ();

      _info = new SerializationInfo (typeof (SerializableClass), new FormatterConverter ());
      _context = new StreamingContext();
      _factory = new InterceptedDomainObjectFactory (Environment.CurrentDirectory);
      
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (SerializableClass));
      _serializableInstance = _factory.GetTypesafeConstructorInvoker<SerializableClass> (concreteType).With();

      Type concreteTypeImplementingISerializable = _factory.GetConcreteDomainObjectType (typeof (SerializableClassImplementingISerializable));
      _serializableInstanceImplementingISerializable =
          _factory.GetTypesafeConstructorInvoker<SerializableClassImplementingISerializable> (concreteTypeImplementingISerializable).With();

      Type concreteTypeImplementingISerializableNotCallingBaseCtor =
          _factory.GetConcreteDomainObjectType (typeof (SerializableClassImplementingISerializableNotCallingBaseCtor));
      _serializableInstanceImplementingISerializableNotCallingBaseCtor =
          _factory.GetTypesafeConstructorInvoker<SerializableClassImplementingISerializableNotCallingBaseCtor> (
              concreteTypeImplementingISerializableNotCallingBaseCtor).With();
      Type concreteTypeImplementingISerializableNotCallingBaseGetObjectData =
          _factory.GetConcreteDomainObjectType (typeof (SerializableClassImplementingISerializableNotCallingBaseGetObjectData));
      _serializableInstanceImplementingISerializableNotCallingGetObjectData =
          _factory.GetTypesafeConstructorInvoker<SerializableClassImplementingISerializableNotCallingBaseGetObjectData> (
              concreteTypeImplementingISerializableNotCallingBaseGetObjectData).With ();
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
      object[] members = (object[]) _info.GetValue ("baseMemberValues", typeof (object[]));
      Assert.IsNotNull (members);
      Assert.Contains (0x400dd00d, members);
    }

    [Test]
    public void GetObjectDataForGeneratedTypesDoesntAddMembersIfToldTo ()
    {
      _serializableInstance.I = 0x400dd00d;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, false);
      object[] members = (object[]) _info.GetValue ("baseMemberValues", typeof (object[]));
      Assert.IsNull (members);
    }

    [Test]
    public void CtorCreatesUninitializedRealObjectOfRightType ()
    {
      _serializableInstance.I = 13;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      SerializationHelper helper = new SerializationHelper (_info, _context);
      SerializableClass realObject = (SerializableClass) helper.GetRealObject (_context);
      Assert.IsNotNull (realObject);
      Assert.AreEqual (13, _serializableInstance.I);
      Assert.AreNotEqual (13, realObject.I);
      Assert.IsTrue (_factory.WasCreatedByFactory (((object)realObject).GetType()));
      Assert.AreSame (typeof (SerializableClass), ((object) realObject).GetType ().BaseType);
    }

    [Test]
    public void CtorUsesISerializableBaseCtorIfNecessary ()
    {
      Assert.IsFalse (_serializableInstanceImplementingISerializable.ISerializableCtorCalled);
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializable, false);
      _serializableInstanceImplementingISerializable.GetObjectData (_info, _context);
      SerializationHelper helper = new SerializationHelper (_info, _context);
      SerializableClassImplementingISerializable realObject = (SerializableClassImplementingISerializable) helper.GetRealObject (_context);
      Assert.IsTrue (realObject.ISerializableCtorCalled);
    }

    [Test]
    public void OnDeserializationFixesUninitializedRealObjectIfNecessary ()
    {
      _serializableInstance.I = 13;
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      SerializationHelper helper = new SerializationHelper (_info, _context);
      SerializableClass realObject = (SerializableClass) helper.GetRealObject (_context);
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

      SerializationHelper helper = new SerializationHelper (_info, _context);
      SerializableClassImplementingISerializable realObject = (SerializableClassImplementingISerializable) helper.GetRealObject (_context);

      Assert.AreEqual (4, realObject.I);

      helper.OnDeserialization (null);
      Assert.IsTrue (realObject.ISerializableCtorCalled);
      Assert.AreEqual (4, realObject.I);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The deserialization constructor on type "
        + "Remotion.Data.DomainObjects.UnitTests.Interception.SerializationHelperTest+SerializableClassImplementingISerializableNotCallingBaseCtor "
        + "did not call DomainObject's base deserialization constructor.")]
    public void OnDeserializationThrowsIfISerializableBaseCtorNotCalled ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstanceImplementingISerializableNotCallingBaseCtor, false);
      _serializableInstanceImplementingISerializableNotCallingBaseCtor.GetObjectData (_info, _context);
      new SerializationHelper (_info, _context).OnDeserialization (null);
    }

    [Test]
    [ExpectedException (typeof (SerializationException), ExpectedMessage = "The GetObjectData method on type "
        + "Remotion.Data.DomainObjects.UnitTests.Interception.SerializationHelperTest+SerializableClassImplementingISerializableNotCallingBaseGetObjectData"
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
      SerializationHelper helper = new SerializationHelper (_info, _context);
      helper.GetObjectData (_info, _context);
    }

    [Test]
    public void DeserializerUsesConfigFactory ()
    {
      SerializationHelper.GetObjectDataForGeneratedTypes (_info, _context, _serializableInstance, true);
      SerializationHelper helper = new SerializationHelper (_info, _context);
      Assert.AreSame (DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory.GetConcreteDomainObjectType (typeof (SerializableClass)),
          helper.GetRealObject (_context).GetType ());
    }
  }
}
