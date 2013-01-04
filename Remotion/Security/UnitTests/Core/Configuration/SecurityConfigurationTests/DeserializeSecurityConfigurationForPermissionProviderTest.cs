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
using System.Configuration;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Configuration;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Configuration.SecurityConfigurationTests
{
  [TestFixture]
  public class DeserializeSecurityConfigurationForPermissionProviderTest: TestBase
  {
    [Test]
    public void Test_FallbackToDefaultWellKnownDefaultPermissionProvider()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.That (Configuration.PermissionProviders.Count, Is.EqualTo (1));
      Assert.IsInstanceOf (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.That (Configuration.PermissionProviders["Reflection"], Is.SameAs (Configuration.PermissionProvider));
    }

    [Test]
    public void Test_PermissionProviderIsAlwaysSameInstance()
    {
      string xmlFragment = @"<remotion.security />";
      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Assert.That (Configuration.PermissionProvider, Is.SameAs (Configuration.PermissionProvider));
    }

    [Test]
    public void Test_CustomPermissionProvider()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Custom"">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.IsInstanceOf (typeof (PermissionProviderMock), Configuration.PermissionProvider);
      Assert.That (Configuration.PermissionProviders["Custom"], Is.SameAs (Configuration.PermissionProvider));
    }

    [Test]
    public void Test_WithPermissionProvidersAndFallbackToDefaultWellKnownDefaultPermissionProvider()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Assert.That (Configuration.PermissionProviders.Count, Is.EqualTo (2));
      Assert.IsInstanceOf (typeof (PermissionProviderMock), Configuration.PermissionProviders["Custom"]);
      Assert.IsInstanceOf (typeof (PermissionReflector), Configuration.PermissionProvider);
      Assert.That (Configuration.PermissionProviders["Reflection"], Is.SameAs (Configuration.PermissionProvider));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The provider 'Invalid' specified for the defaultPermissionProvider does not exist in the providers collection.")]
    public void Test_WithCustomPermissionProviderAndInvalidName()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Invalid"">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The name of the entry 'Reflection' identifies a well known provider and cannot be reused for custom providers.")]
    public void Test_DuplicateWellKnownPermissionProviderForPermissionReflector()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider=""Reflection"">
            <permissionProviders>
              <add name=""Reflection"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationErrorsException),
        ExpectedMessage = "The value for the property 'defaultPermissionProvider' is not valid. The error is: The string must be at least 1 characters long.")]
    public void Test_WithCustomPermissionProviderNameEmpty()
    {
      string xmlFragment =
          @"
          <remotion.security defaultPermissionProvider="""">
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException))]
    public void Test_WithPermissionProvidersReadOnly()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PermissionProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);
      Configuration.PermissionProviders.Clear();
    }

    [Test]
    [ExpectedExceptionAttribute (typeof (ConfigurationErrorsException),
        ExpectedMessage = "Provider must implement the interface 'Remotion.Security.Metadata.IPermissionProvider'.")]
    public void InstantiateProvider_WithTypeNotImplementingRequiredInterface()
    {
      string xmlFragment =
          @"
          <remotion.security>
            <permissionProviders>
              <add name=""Custom"" type=""Remotion.Security.UnitTests::Core.Configuration.PrincipalProviderMock"" />
            </permissionProviders>
          </remotion.security>";

      ConfigurationHelper.DeserializeSection (Configuration, xmlFragment);

      Dev.Null = Configuration.PermissionProvider;
    }
  }
}
