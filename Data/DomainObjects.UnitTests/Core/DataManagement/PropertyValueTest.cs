using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Core.Resources;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.DataManagement
{
  [TestFixture]
  public class PropertyValueTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false, new List<Type> ());
    }
    
    [Test]
    public void TestEquals ()
    {
      PropertyDefinition intDefinition = CreateIntPropertyDefinition ("test");
      PropertyValue propertyValue1 = new PropertyValue (intDefinition, 5);
      PropertyValue propertyValue2 = new PropertyValue (intDefinition, 5);
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "Initial values");

      propertyValue1.Value = 10;
      Assert.IsFalse (propertyValue1.Equals (propertyValue2), "After changing first value.");

      propertyValue1.Value = 5;
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing first value back to initial value.");

      propertyValue1.Value = 10;
      propertyValue2.Value = 10;
      Assert.IsTrue (propertyValue1.Equals (propertyValue2), "After changing both values.");

      PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
      propertyValue3.Value = 10;
      Assert.IsFalse (propertyValue1.Equals (propertyValue3), "Different original values.");
    }

    [Test]
    public void HashCode ()
    {
      PropertyValue propertyValue1 = CreateIntPropertyValue ("test", 5);
      PropertyValue propertyValue2 = CreateIntPropertyValue ("test", 5);
      Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "Initial values");

      propertyValue1.Value = 10;
      Assert.IsFalse (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing first value.");

      propertyValue1.Value = 5;
      Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing first value back to initial value.");

      propertyValue1.Value = 10;
      propertyValue2.Value = 10;
      Assert.IsTrue (propertyValue1.GetHashCode () == propertyValue2.GetHashCode (), "After changing both values.");

      PropertyValue propertyValue3 = CreateIntPropertyValue ("test", 10);
      Assert.IsFalse (propertyValue1.GetHashCode () == propertyValue3.GetHashCode (), "Different original values.");
    }

    [Test]
    public void IsRelationProperty_False ()
    {
      PropertyDefinition intDefinition = CreateIntPropertyDefinition ("test");
      PropertyValue propertyValue1 = new PropertyValue (intDefinition, 5);
      Assert.IsFalse (propertyValue1.IsRelationProperty);
    }

    [Test]
    public void IsRelationProperty_True ()
    {
      PropertyDefinition propertyDefinition = CreatePropertyDefinition ("test", typeof (ObjectID), null);
      PropertyValue propertyValue1 = new PropertyValue (propertyDefinition, null);
      Assert.IsTrue (propertyValue1.IsRelationProperty);
    }

    [Test]
    public void SettingOfValueForValueType ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("test", 5);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.AreEqual (5, propertyValue.Value, "Value after initialization");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = 5;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.AreEqual (5, propertyValue.Value, "Value after change #1");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual (10, propertyValue.Value, "Value after change #2");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = 20;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.AreEqual (20, propertyValue.Value, "Value after change #3");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");

      propertyValue.Value = 5;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #4");
      Assert.AreEqual (5, propertyValue.Value, "Value after change #4");
      Assert.AreEqual (5, propertyValue.OriginalValue, "OriginalValue after change #4");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #4");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #4");
    }

    [Test]
    public void SettingOfNullValueForNullableValueType ()
    {
      PropertyValue propertyValue = CreateNullableIntPropertyValue ("test", null);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.IsNull (propertyValue.Value, "Value after initialization");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.IsNull (propertyValue.Value, "Value after change #1");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = 10;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual (10, propertyValue.Value, "Value after change #2");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.IsNull (propertyValue.Value, "Value after change #3");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    public void SettingOfNullValueForString ()
    {
      PropertyValue propertyValue = CreateStringPropertyValue ("test", null);

      Assert.AreEqual ("test", propertyValue.Name, "Name after initialization");
      Assert.IsNull (propertyValue.Value, "Value after initialization");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after initialization");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after initialization");
      Assert.IsFalse (propertyValue.HasBeenTouched, "HasBeenTouched after initialization");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #1");
      Assert.IsNull (propertyValue.Value, "Value after change #1");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #1");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #1");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #1");

      propertyValue.Value = "Test Value";

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #2");
      Assert.AreEqual ("Test Value", propertyValue.Value, "Value after change #2");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #2");
      Assert.IsTrue (propertyValue.HasChanged, "HasChanged after change #2");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #2");

      propertyValue.Value = null;

      Assert.AreEqual ("test", propertyValue.Name, "Name after change #3");
      Assert.IsNull (propertyValue.Value, "Value after change #3");
      Assert.IsNull (propertyValue.OriginalValue, "OriginalValue after change #3");
      Assert.IsFalse (propertyValue.HasChanged, "HasChanged after change #3");
      Assert.IsTrue (propertyValue.HasBeenTouched, "HasBeenTouched after change #3");
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException))]
    public void MaxLengthCheck ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (string), 10);
      PropertyValue propertyValue = new PropertyValue (definition, "12345");
      propertyValue.Value = "12345678901";
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException))]
    public void MaxLengthCheckInConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (string), 10);
      new PropertyValue (definition, "12345678901");
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void TypeCheckInConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (string), 10);
      new PropertyValue (definition, 123);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void TypeCheck ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (string), 10);
      PropertyValue propertyValue = new PropertyValue (definition, "123");
      propertyValue.Value = 123;
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableStringToNull ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (string), false, 10);
      PropertyValue propertyValue = new PropertyValue (definition, string.Empty);

      propertyValue.Value = null;
    }

    [Test]
    public void SetNullableBinary ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), true);
      PropertyValue propertyValue = new PropertyValue (definition, null);
      Assert.IsNull (propertyValue.Value);
    }

    [Test]
    public void SetNotNullableBinary ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), false);

      PropertyValue propertyValue = new PropertyValue (definition, new byte[0]);
      ResourceManager.IsEmptyImage ((byte[]) propertyValue.Value);

      propertyValue.Value = ResourceManager.GetImage1 ();
      ResourceManager.IsEqualToImage1 ((byte[]) propertyValue.Value);
    }

    [Test]
    [ExpectedException (typeof (InvalidTypeException))]
    public void SetBinaryWithInvalidType ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), false);
      new PropertyValue (definition, new int[0]);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableBinaryToNullViaConstructor ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), false);
      new PropertyValue (definition, null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Property 'test' does not allow null values.")]
    public void SetNotNullableBinaryToNullViaProperty ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), false);
      PropertyValue propertyValue = new PropertyValue (definition, ResourceManager.GetImage1 ());
      propertyValue.Value = null;
    }

    [Test]
    [ExpectedException (typeof (ValueTooLongException), ExpectedMessage = "Value for property 'test' is too large. Maximum size: 1000000.")]
    public void SetBinaryLargerThanMaxLength ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (byte[]), true, 1000000);
      PropertyValue propertyValue = new PropertyValue (definition, new byte[0]);
      propertyValue.Value = ResourceManager.GetImageLarger1MB ();
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The relation property 'test' cannot be set directly.")]
    public void SetRelationPropertyDirectly ()
    {
      PropertyDefinition definition = ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, "test", "test", typeof (ObjectID), true);
      PropertyValue propertyValue = new PropertyValue (definition, null);

      propertyValue.Value = DomainObjectIDs.Customer1;
    }

    [Test]
    public void Commit ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("testProperty", 0);
      propertyValue.Value = 5;
      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsTrue (propertyValue.HasChanged);
      Assert.IsTrue (propertyValue.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "Commit");

      Assert.AreEqual (5, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsFalse (propertyValue.HasChanged);
      Assert.IsFalse (propertyValue.HasBeenTouched);
    }

    [Test]
    public void Rollback ()
    {
      PropertyValue propertyValue = CreateIntPropertyValue ("testProperty", 0);
      propertyValue.Value = 5;
      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (5, propertyValue.Value);
      Assert.IsTrue (propertyValue.HasChanged);
      Assert.IsTrue (propertyValue.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "Rollback");

      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (0, propertyValue.Value);
      Assert.IsFalse (propertyValue.HasChanged);
      Assert.IsFalse (propertyValue.HasBeenTouched);
    }

    [Test]
    public void AssumeSameState_FromUnchangedToChanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      propertyValue.Value = 6;
      
      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "AssumeSameState", propertyValue);

      Assert.AreEqual (1, propertyValue2.OriginalValue);
      Assert.AreEqual (6, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void AssumeSameState_FromUnchangedToUnchanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "AssumeSameState", propertyValue);

      Assert.AreEqual (1, propertyValue2.OriginalValue);
      Assert.AreEqual (1, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void AssumeSameState_FromChangedToChanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      propertyValue.Value = 6;
      propertyValue2.Value = 7;

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (7, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "AssumeSameState", propertyValue);

      Assert.AreEqual (1, propertyValue2.OriginalValue);
      Assert.AreEqual (6, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void AssumeSameState_FromChangedToUnchanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);
      
      propertyValue2.Value = 7;

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (7, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "AssumeSameState", propertyValue);

      Assert.AreEqual (1, propertyValue2.OriginalValue);
      Assert.AreEqual (1, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoUnchanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      propertyValue.Value = 6;

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "TakeOverCommittedData", propertyValue);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (6, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoUnchanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "TakeOverCommittedData", propertyValue);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (1, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_ChangedIntoChanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      propertyValue.Value = 6;
      propertyValue2.Value = 7;

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (7, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "TakeOverCommittedData", propertyValue);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (6, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoChanged ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 1);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      propertyValue2.Value = 7;

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (7, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "TakeOverCommittedData", propertyValue);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (1, propertyValue2.Value);
      Assert.IsTrue (propertyValue2.HasChanged);
      Assert.IsTrue (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void TakeOverCommittedData_UnchangedIntoEqual ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      PropertyValue propertyValue2 = new PropertyValue (definition, 0);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);

      PrivateInvoke.InvokeNonPublicMethod (propertyValue2, "TakeOverCommittedData", propertyValue);

      Assert.AreEqual (0, propertyValue2.OriginalValue);
      Assert.AreEqual (0, propertyValue2.Value);
      Assert.IsFalse (propertyValue2.HasChanged);
      Assert.IsFalse (propertyValue2.HasBeenTouched);
    }

    [Test]
    public void GetData_RestoreData_NonDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.Value = 17;

      Assert.AreEqual (0, propertyValue.OriginalValue);
      Assert.AreEqual (17, propertyValue.Value);
      Assert.IsTrue (propertyValue.HasBeenTouched);
      Assert.IsFalse (propertyValue.IsDiscarded);

      object[] data = propertyValue.GetData();

      PropertyValue restoredValue = new PropertyValue (definition, 1);

      Assert.AreNotEqual (0, restoredValue.OriginalValue);
      Assert.AreNotEqual (17, restoredValue.Value);
      Assert.IsFalse (restoredValue.HasBeenTouched);
      Assert.IsFalse (restoredValue.IsDiscarded);

      restoredValue.RestoreData (data);

      Assert.AreEqual (0, restoredValue.OriginalValue);
      Assert.AreEqual (17, restoredValue.Value);
      Assert.IsTrue (restoredValue.HasBeenTouched);
      Assert.IsFalse (restoredValue.IsDiscarded);
    }

    [Test]
    public void GetData_RestoreData_Discarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      PrivateInvoke.InvokeNonPublicMethod (propertyValue, "Discard");

      Assert.IsTrue (propertyValue.IsDiscarded);

      object[] data = propertyValue.GetData ();

      PropertyValue restoredValue = new PropertyValue (definition, 1);

      Assert.IsFalse (restoredValue.IsDiscarded);

      restoredValue.RestoreData (data);

      Assert.IsTrue (restoredValue.IsDiscarded);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter name: data", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataLength ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[2]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Parameter name: data[0]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem0_Type_Discarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { "foo" });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter name: data[0]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem0_Value_Discarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { false });
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Parameter name: data[0]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem0_Type_NotDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { "foo", 0, 0, true });
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter name: data[0]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem0_Value_NotDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { true, 0, 0, true });
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Parameter name: data[1]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem1_Type_NotDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { false, "foo", 1, true });
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Parameter name: data[2]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem2_Type_NotDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { false, 1, "foo", true });
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException), ExpectedMessage = "Parameter name: data[3]", MatchType = MessageMatch.Contains)]
    public void RestoreData_InvalidDataItem3_Type_NotDiscarded ()
    {
      PropertyDefinition definition = CreatePropertyDefinition ("testProperty2", typeof (int), null);
      PropertyValue propertyValue = new PropertyValue (definition, 0);
      propertyValue.RestoreData (new object[] { false, 1, 1, "foo" });
    }
    
    private PropertyValue CreateIntPropertyValue (string name, int intValue)
    {
      return CreatePropertyValue (name, typeof (int), null, intValue);
    }

    private PropertyValue CreateNullableIntPropertyValue (string name, int? intValue)
    {
      return CreatePropertyValue (name, typeof (int?), null, intValue);
    }

    private PropertyValue CreateStringPropertyValue (string name, string stringValue)
    {
      bool isNullable = (stringValue == null) ? true : false;
      return CreatePropertyValue (name, typeof (string), isNullable, stringValue);
    }

    private PropertyDefinition CreateIntPropertyDefinition (string name)
    {
      return CreatePropertyDefinition (name, typeof (int), null);
    }

    private PropertyDefinition CreatePropertyDefinition (string name, Type propertyType, bool? isNullable)
    {
      int? maxLength = (propertyType == typeof (string)) ? (int?) 100 : null;

      return ReflectionBasedPropertyDefinitionFactory.CreateReflectionBasedPropertyDefinition(_classDefinition, name, name, propertyType, isNullable, maxLength, true);
    }

    private PropertyValue CreatePropertyValue (string name, Type propertyType, bool? isNullable, object value)
    {
      return new PropertyValue (CreatePropertyDefinition (name, propertyType, isNullable), value);
    }
  }
}
