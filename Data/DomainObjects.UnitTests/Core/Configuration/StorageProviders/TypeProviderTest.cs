using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration.StorageProviders;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.StorageProviders
{
  [TestFixture]
  public class TypeProviderTest: StandardMappingTest
  {
    private enum TestEnum { }

    [Test]
    public void CheckSupportedTypes()
    {
      TypeProvider typeProvider = new TypeProvider();
      Assert.That (typeProvider.IsTypeSupported (typeof (bool)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (byte)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (DateTime)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (decimal)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (double)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (TestEnum)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (Guid)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (short)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (int)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (long)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (float)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (string)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (ObjectID)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (byte[])), Is.True);

      Assert.That (typeProvider.IsTypeSupported (typeof (Enum)), Is.True);

      Assert.That (typeProvider.IsTypeSupported (typeof (object)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (char)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (char[])), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (DomainObject)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (DomainObjectCollection)), Is.False);
    }
  }
}