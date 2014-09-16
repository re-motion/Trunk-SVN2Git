using System;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueControlObjectTest : IntegrationTest
  {
    [Test]
    public void TestInsertOfValidValue ()
    {
      var home = Start();

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { ID = "body_DataEditControl_PartnerField_Normal" }).FillWith ("Bla");

      Assert.That (home.Scope.FindId ("BOUINormalLabel").Text, Is.Empty);

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { ID = "body_DataEditControl_PartnerField_Normal_AlternativeRendering" }).FillWith ("Blubba");

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_Normal" }).FillWith ("D, A");

      Assert.That (home.Scope.FindId ("BOUINormalLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000009"));

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { DisplayName = "Partner" }).FillWith ("Bla");

      Assert.That (home.Scope.FindId ("BOUINormalLabel").Text, Is.Empty);

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { BoundProperty = "Partner" }).FillWith ("D, A");

      Assert.That (home.Scope.FindId ("BOUINormalLabel").Text, Is.EqualTo("00000000-0000-0000-0000-000000000009"));

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters
          {
              BoundType = "Remotion.ObjectBinding.Sample.Person, Remotion.ObjectBinding.Sample",
              BoundProperty = "Partner"
          }).FillWith ("Bla");

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_NoAutoPostBack" }).FillWith ("Blubba");

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_Normal" }).FillWith ("Foo");

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_Normal" }).FillWith ("Foo", WaitingStrategies.Null);

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_Normal" }).FillWith ("D, A");

      home.GetControl (
          new BocAutoCompleteReferenceValueSelector(),
          new BocControlSelectionParameters { LocalID = "PartnerField_Normal" }).ExecuteCommand();

      Assert.That (home.Scope.FindId ("ActionPerformedLabel").Text, Is.EqualTo("CommandClick"));
      Assert.That (home.Scope.FindId ("ActionPerformedParameterLabel").Text, Is.Empty);
      Assert.That (home.Scope.FindId ("ActionPerformedSenderLabel").Text, Is.EqualTo("PartnerField_Normal"));
    }
  }
}