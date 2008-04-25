using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.ReferencePropertyTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void CreateIfNull ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      Assert.That (property.CreateIfNull, Is.False);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException),
        ExpectedMessage = "Create method is not supported by 'Remotion.ObjectBinding.BindableObject.Properties.ReferenceProperty'.")]
    public void Create ()
    {
      IBusinessObjectReferenceProperty property = CreateProperty ("Scalar");

      property.Create (null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithMissmatchedConcreteType ()
    {
      new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"), _businessObjectProvider),
          TypeFactory.GetConcreteType (typeof (ClassWithAllDataTypes)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void Initialize_WithConcreteTypeNotImplementingIBusinessObject ()
    {
      new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), "Scalar"), _businessObjectProvider),
          typeof (SimpleBusinessObjectClass));
    }

    private ReferenceProperty CreateProperty (string propertyName)
    {
      return new ReferenceProperty (
          GetPropertyParameters (GetPropertyInfo (typeof (ClassWithReferenceType<SimpleBusinessObjectClass>), propertyName), _businessObjectProvider),
          TypeFactory.GetConcreteType (typeof (SimpleBusinessObjectClass)));
    }
  }
}