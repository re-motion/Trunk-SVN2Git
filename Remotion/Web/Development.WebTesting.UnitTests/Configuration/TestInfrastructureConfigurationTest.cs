// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Coypu;
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.Configuration;

namespace Remotion.Web.Development.WebTesting.UnitTests.Configuration
{
  [TestFixture]
  public class TestInfrastructureConfigurationTest
  {
    private readonly string _testRequestErrorDetectionStrategyAssemblyQualifiedName = typeof (TestRequestErrorDetectionStrategy).AssemblyQualifiedName;

    private readonly MethodInfo _webTestConfigurationFactoryPropertySetter = typeof (WebTestConfigurationSection)
        .GetProperties (BindingFlags.Instance | BindingFlags.NonPublic)
        .Single (
            pi => pi.Name == "Item"
                  && pi.GetIndexParameters().Length > 0
                  && pi.SetMethod.GetParameters().Any (p => p.ParameterType == typeof (string)))
        .GetSetMethod (true);

    [Test]
    public void CreateFromWebTestConfigurationSection ()
    {
      var webTestConfigurationSection = CreateWebTestConfigurationSection();
      _webTestConfigurationFactoryPropertySetter.Invoke (webTestConfigurationSection, new object[] { "webApplicationRoot", "http://some.url:1337/" });
      _webTestConfigurationFactoryPropertySetter.Invoke (webTestConfigurationSection, new object[] { "screenshotDirectory", @".\SomeScreenshotDirectory" });
      _webTestConfigurationFactoryPropertySetter.Invoke (webTestConfigurationSection, new object[] { "searchTimeout", TimeSpan.FromSeconds (43) });
      _webTestConfigurationFactoryPropertySetter.Invoke (webTestConfigurationSection, new object[] { "retryInterval", TimeSpan.FromMilliseconds (42) });
      _webTestConfigurationFactoryPropertySetter.Invoke (webTestConfigurationSection, new object[] { "closeBrowserWindowsOnSetUpAndTearDown", false });
      _webTestConfigurationFactoryPropertySetter.Invoke (
          webTestConfigurationSection,
          new object[] { "requestErrorDetectionStrategy", _testRequestErrorDetectionStrategyAssemblyQualifiedName });

      var testInfrastructureConfiguration = new TestInfrastructureConfiguration (webTestConfigurationSection);

      Assert.That (testInfrastructureConfiguration.WebApplicationRoot, Is.EqualTo ("http://some.url:1337/"));
      Assert.That (testInfrastructureConfiguration.ScreenshotDirectory, Is.EqualTo (Path.GetFullPath (@".\SomeScreenshotDirectory")));
      Assert.That (testInfrastructureConfiguration.SearchTimeout, Is.EqualTo (TimeSpan.FromSeconds (43)));
      Assert.That (testInfrastructureConfiguration.RetryInterval, Is.EqualTo (TimeSpan.FromMilliseconds (42)));
      Assert.That (testInfrastructureConfiguration.CloseBrowserWindowsOnSetUpAndTearDown, Is.EqualTo (false));
      Assert.That (testInfrastructureConfiguration.RequestErrorDetectionStrategy, Is.InstanceOf<TestRequestErrorDetectionStrategy>());
    }

    private WebTestConfigurationSection CreateWebTestConfigurationSection ()
    {
      return (WebTestConfigurationSection) Activator.CreateInstance (typeof (WebTestConfigurationSection), true);
    }

    private class TestRequestErrorDetectionStrategy : IRequestErrorDetectionStrategy
    {
      public void CheckPageForError (ElementScope scope) => throw new NotImplementedException();
    }
  }
}