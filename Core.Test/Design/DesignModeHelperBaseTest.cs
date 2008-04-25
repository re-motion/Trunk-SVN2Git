using System;
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Design;
using Rhino.Mocks;

namespace Remotion.UnitTests.Design
{
  [TestFixture]
  public class DesignModeHelperBaseTest
  {
    private MockRepository _mockRepository;
    private IDesignerHost _mockDesignerHost;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _mockDesignerHost = _mockRepository.CreateMock<IDesignerHost>();
    }

    [Test]
    public void Initialize()
    {
      _mockRepository.ReplayAll();

      DesignModeHelperBase stubDesginerHelper = new StubDesignModeHelper (_mockDesignerHost);

      _mockRepository.VerifyAll();
      Assert.That (stubDesginerHelper.DesignerHost, Is.SameAs (_mockDesignerHost));
    }
  }
}