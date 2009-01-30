// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Samples.Tutorial.T01_Configuration.ExtendsSamples;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.Tutorial.T01_Configuration
{
  [TestFixture]
  public class ExtendsSamplesTest
  {
    [Test]
    public void File_IsNumbered ()
    {
      var numberedFile = (INumberedFile) ObjectFactory.Create<File> (ParamList.Empty);
      Console.WriteLine (numberedFile.GetFileNumber ());
      Assert.That (numberedFile.GetFileNumber (), NUnit.Framework.SyntaxHelpers.Text.Matches ("........-....-....-....-............"));
    }
  }
}