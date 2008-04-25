using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  public class EnumerationTestBase : TestBase
  {
    protected void CheckEnumerationValueInfos (EnumerationValueInfo[] expected, IEnumerationValueInfo[] actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.Length, Is.EqualTo (expected.Length));
      for (int i = 0; i < expected.Length; i++)
        CheckEnumerationValueInfo (expected[i], actual[i]);
    }

    protected void CheckEnumerationValueInfo (EnumerationValueInfo expected, IEnumerationValueInfo actual)
    {
      ArgumentUtility.CheckNotNull ("expected", expected);

      Assert.That (actual, Is.InstanceOfType (expected.GetType()));
      Assert.That (actual.Value, Is.EqualTo (expected.Value));
      Assert.That (actual.Identifier, Is.EqualTo (expected.Identifier));
      Assert.That (actual.IsEnabled, Is.EqualTo (expected.IsEnabled));
      Assert.That (actual.DisplayName, Is.EqualTo (expected.DisplayName));
    }
  }
}