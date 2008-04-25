using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BusinessObjectStringFormatterServiceTests
{
  [TestFixture]
  public class GetPropertyString_WithNotSupportedProperty
  {
    private BusinessObjectStringFormatterService _stringFormatterService;
    private MockRepository _mockRepository;
    private IBusinessObject _mockBusinessObject;
    private IBusinessObjectProperty _mockProperty;

    [SetUp]
    public void SetUp ()
    {
      _stringFormatterService = new BusinessObjectStringFormatterService ();
      _mockRepository = new MockRepository ();
      _mockBusinessObject = _mockRepository.CreateMock<IBusinessObject> ();
      _mockProperty = _mockRepository.CreateMock<IBusinessObjectProperty> ();
    }

    [Test]
    public void Scalar_WithValue ()
    {
      object value = new object();
      Expect.Call (_mockProperty.IsList).Return (false);
      Expect.Call (_mockBusinessObject.GetProperty (_mockProperty)).Return (value);
      _mockRepository.ReplayAll();

      string actual = _stringFormatterService.GetPropertyString (_mockBusinessObject, _mockProperty, null);

      _mockRepository.VerifyAll();
      Assert.That (actual, Is.EqualTo (value.ToString()));
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