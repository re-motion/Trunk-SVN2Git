using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithReferenceProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectReferenceProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService ();
      _mockRepository = new MockRepository ();
      _mockBusinessObject = _mockRepository.CreateMock<IBusinessObject> ();
      _mockProperty = _mockRepository.CreateMock<IBusinessObjectReferenceProperty> ();
    }

    [Test]
    public void Scalar_WithBusinessObjectValue ()
    {
      IBusinessObject mockValue = _mockRepository.CreateMock<IBusinessObject>();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (mockValue);
      Expect.Call (mockValue.DisplayName).Return ("ExpectedStringValue");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithBusinessObjectWithIdentityValue ()
    {
      IBusinessObjectWithIdentity mockValue = _mockRepository.CreateMock<IBusinessObjectWithIdentity> ();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (mockValue);
      Expect.Call (mockValue.DisplayName).Return ("ExpectedStringValue");
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.EqualTo ("ExpectedStringValue"));
    }

    [Test]
    public void Scalar_WithNull ()
    {
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (null);
      _mockRepository.ReplayAll ();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll ();
      Assert.That (actual, Is.Empty);
    }
  }
}