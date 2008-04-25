using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using TypeNameConverter=Remotion.Utilities.TypeNameConverter;

namespace Remotion.UnitTests.Configuration
{
  [TestFixture]
  public class TypeElementTest
  {
    [Test]
    public void Initialize()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      ConfigurationPropertyCollection properties = (ConfigurationPropertyCollection) PrivateInvoke.GetNonPublicProperty (typeElement, "Properties");
      Assert.IsNotNull (properties);
      ConfigurationProperty property = properties["type"];
      Assert.IsNotNull (property);
      Assert.IsNull (property.DefaultValue);
      Assert.IsInstanceOfType (typeof (TypeNameConverter), property.Converter);
      Assert.IsInstanceOfType (typeof (SubclassTypeValidator), property.Validator);
      Assert.IsTrue (property.IsRequired);
    }

    [Test]
    public void GetAndSetType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      typeElement.Type = typeof (DerivedSampleType);
      Assert.AreEqual (typeof (DerivedSampleType), typeElement.Type);
    }

    [Test]
    public void GetType_WithTypeNull()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      Assert.IsNull (typeElement.Type);
    }

    [Test]
    public void CreateInstance_WithType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();
      typeElement.Type = typeof (DerivedSampleType);

      Assert.IsInstanceOfType (typeof (DerivedSampleType), typeElement.CreateInstance());
    }

    [Test]
    public void CreateInstance_WithoutType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      Assert.IsNull (typeElement.CreateInstance());
    }

    [Test]
    public void Deserialize_WithValidType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      string xmlFragment = @"<theElement type=""Remotion.UnitTests::Configuration.SampleType"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Assert.AreEqual (typeof (SampleType), typeElement.Type);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException))]
    public void Deserialize_WithInvalidType()
    {
      TypeElement<SampleType> typeElement = new TypeElement<SampleType>();

      string xmlFragment = @"<theElement type=""System.Object, mscorlib"" />";
      ConfigurationHelper.DeserializeElement (typeElement, xmlFragment);

      Dev.Null = typeElement.Type;
    }
  }
}