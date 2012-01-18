using NUnit.Framework;
using Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests.WxeFunctions;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Web.WxeTransactedFunctionIntegrationTests
{
  [TestFixture]
  public class SerializationTest : WxeTransactedFunctionIntegrationTestBase
  {
    [Test]
    public void Serialization ()
    {
      var function = new SerializationTestTransactedFunction();
      function.Execute (Context);
      Assert.That (function.FirstStepExecuted, Is.True);
      Assert.That (function.SecondStepExecuted, Is.True);

      var deserializedFunction =
          (SerializationTestTransactedFunction) Serializer.Deserialize (function.SerializedSelf);
      Assert.That (deserializedFunction.FirstStepExecuted, Is.True);
      Assert.That (deserializedFunction.SecondStepExecuted, Is.False);

      deserializedFunction.Execute (Context);

      Assert.That (deserializedFunction.FirstStepExecuted, Is.True);
      Assert.That (deserializedFunction.SecondStepExecuted, Is.True);
    }
  }
}