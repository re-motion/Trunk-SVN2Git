// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.Samples.Tutorial.T01_Configuration;
using Remotion.Mixins.Samples.Tutorial.T01_Configuration.UsesSamples;
using Remotion.Reflection;

namespace Remotion.Mixins.Samples.UnitTests.Tutorial.T01_Configuration
{
  [TestFixture]
  public class UsesSamplesTest
  {
    [Test]
    public void MyCloneableClass_IsICloneable ()
    {
      var instance = ObjectFactory.Create<MyCloneableClass> (ParamList.Empty);
      Assert.That (instance, Is.InstanceOfType (typeof (ICloneable)));
    }
  }
}