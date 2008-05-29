using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectWithIdentityMixinTest : TestBase
  {
    [Test]
    public void InstantiateMixedType ()
    {
      Assert.That (
          ObjectFactory.Create<ClassWithIdentity>().With ("TheUniqueIdentifier"),
          Is.InstanceOfType (typeof (IBusinessObjectWithIdentity)));
    }

    [Test]
    public void GetUniqueIdentifier ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin> (ObjectFactory.Create<ClassWithIdentity>().With ("TheUniqueIdentifier"));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That (businessObjectWithIdentity.UniqueIdentifier, Is.SameAs ("TheUniqueIdentifier"));
    }

    [Test]
    public void DisplayName ()
    {
      BindableObjectWithIdentityMixin mixin =
          Mixin.Get<BindableObjectWithIdentityMixin> (ObjectFactory.Create<ClassWithIdentityAndDisplayName> ().With ("TheUniqueIdentifier"));
      IBusinessObjectWithIdentity businessObjectWithIdentity = mixin;

      Assert.That (businessObjectWithIdentity.DisplayName, Is.SameAs ("TheUniqueIdentifier"));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> ().With ();
      value.String = "TheString";
      ClassWithIdentity deserialized = Serializer.SerializeAndDeserialize (value);

      Assert.That (deserialized.String, Is.EqualTo ("TheString"));
      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void SerializeAndDeserialize_WithNewBindableObjectProvider ()
    {
      ClassWithIdentity value = ObjectFactory.Create<ClassWithIdentity> ().With ();
      byte[] serialized = Serializer.Serialize (value);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
      ClassWithIdentity deserialized = (ClassWithIdentity) Serializer.Deserialize (serialized);

      Assert.That (((IBusinessObject) deserialized).BusinessObjectClass, Is.Not.SameAs (((IBusinessObject) value).BusinessObjectClass));
    }

    [Test]
    public void HasMixin ()
    {
      Assert.IsTrue (Mixins.TypeUtility.HasMixin (typeof (ClassWithIdentity), typeof (BindableObjectWithIdentityMixin)));
      Assert.IsFalse (Mixins.TypeUtility.HasMixin (typeof (ClassWithAllDataTypes), typeof (BindableObjectWithIdentityMixin)));
      Assert.IsFalse (Mixins.TypeUtility.HasMixin (typeof (object), typeof (BindableObjectWithIdentityMixin)));
    }
  }
}