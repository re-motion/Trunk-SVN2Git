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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;
using NUnit.Framework.Internal.Commands;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ExecutionEngine.PageObjects;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.Utilities;
using Remotion.Web.Development.WebTesting.WebDriver;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.Chrome;
using Remotion.Web.Development.WebTesting.WebDriver.Configuration.InternetExplorer;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests
{
  /// <summary>
  /// Base class for all integration tests.
  /// </summary>
  [RetryTest]
  public abstract class IntegrationTest
  {
    public static WebTestHelper Current => _webTestHelper;
    private static Lazy<Uri> s_webApplicationRoot;

    private static WebTestHelper _webTestHelper;

    protected virtual bool MaximizeMainBrowserSession
    {
      get { return true; }
    }

    protected WebTestHelper Helper
    {
      get { return _webTestHelper; }
    }

    [OneTimeSetUp]
    public void IntegrationTestOneTimeSetUp ()
    {
      _webTestHelper = WebTestHelper.CreateFromConfiguration<CustomWebTestConfigurationFactory>();

      _webTestHelper.OnFixtureSetUp (MaximizeMainBrowserSession);
      s_webApplicationRoot = new Lazy<Uri> (
          () =>
          {
            var uri = new Uri (_webTestHelper.TestInfrastructureConfiguration.WebApplicationRoot);

            // RM-7401: Edge loads pages slower due to repeated hostname resolution.
            if (_webTestHelper.BrowserConfiguration.IsEdge())
              return HostnameResolveHelper.ResolveHostname (uri);

            return uri;
          });
    }

    [SetUp]
    public void IntegrationTestSetUp ()
    {
      _webTestHelper.OnSetUp (GetType().Name + "_" + TestContext.CurrentContext.Test.Name);

      // Prevent failing IE tests due to topmost windows
      if (_webTestHelper.BrowserConfiguration.IsInternetExplorer())
        KillAnyExistingWindowsErrorReportingProcesses();
    }

    [AttributeUsage (AttributeTargets.All)]
    public class RetryTestAttribute : NUnitAttribute, IFixtureBuilder2
    {
      private bool _isIE;

      public RetryTestAttribute ()
      {
        if (IntegrationTest.Current.BrowserConfiguration is InternetExplorerConfiguration)
        {
          _isIE = true;
        }
      }
      public IEnumerable<TestSuite> BuildFrom (ITypeInfo typeInfo, IPreFilter filter)
      {
        if (_isIE)
          return new[] { new NUnitTestFixtureBuilder().BuildFrom (new TypeInfoDecorator (typeInfo), filter) };

        return new[] { new NUnitTestFixtureBuilder().BuildFrom (typeInfo, filter) };
      }

      public IEnumerable<TestSuite> BuildFrom (ITypeInfo typeInfo)
      {
        if (_isIE)
          return new[] { new NUnitTestFixtureBuilder().BuildFrom (new TypeInfoDecorator (typeInfo), new SimplePreFilter()) };

        return new[] { new NUnitTestFixtureBuilder().BuildFrom (typeInfo, new SimplePreFilter()) };
      }
    }

    public class MyRetryAttribute : NUnitAttribute, IWrapSetUpTearDown
    {
      public TestCommand Wrap (TestCommand command)
      {
        return new RetryAttribute (2).Wrap (command);
      }
    }

    public class SimplePreFilter : IPreFilter
    {
      public bool IsMatch (Type type) => true;

      public bool IsMatch (Type type, MethodInfo method) => true;
    }

    public class TypeInfoDecorator : ITypeInfo
    {
      private ITypeInfo _typeInfoImplementation;

      public TypeInfoDecorator (ITypeInfo typeInfoImplementation)
      {
        _typeInfoImplementation = typeInfoImplementation;
      }

      public T[] GetCustomAttributes<T> (bool inherit)
          where T : class
      {
        return _typeInfoImplementation.GetCustomAttributes<T> (inherit).Where(s => !(s is TestFixtureAttribute)).ToArray();
      }

      public bool IsDefined<T> (bool inherit)
          where T : class
      {
        return _typeInfoImplementation.IsDefined<T> (inherit);
      }

      public bool IsType (Type type)
      {
        return _typeInfoImplementation.IsType (type);
      }

      public string GetDisplayName ()
      {
        return _typeInfoImplementation.GetDisplayName();
      }

      public string GetDisplayName (object[] args)
      {
        return _typeInfoImplementation.GetDisplayName (args);
      }

      public Type GetGenericTypeDefinition ()
      {
        return _typeInfoImplementation.GetGenericTypeDefinition();
      }

      public ITypeInfo MakeGenericType (Type[] typeArgs)
      {
        return _typeInfoImplementation.MakeGenericType (typeArgs);
      }

      public bool HasMethodWithAttribute (Type attrType)
      {
        return _typeInfoImplementation.HasMethodWithAttribute (attrType);
      }

      public IMethodInfo[] GetMethods (BindingFlags flags)
      {
        return _typeInfoImplementation.GetMethods (flags).Select (m => new AttributeProvidingMethodInfoDecorator (m, new MyRetryAttribute())).ToArray();
      }

      public ConstructorInfo GetConstructor (Type[] argTypes)
      {
        return _typeInfoImplementation.GetConstructor (argTypes);
      }

      public bool HasConstructor (Type[] argTypes)
      {
        return _typeInfoImplementation.HasConstructor (argTypes);
      }

      public object Construct (object[] args)
      {
        return _typeInfoImplementation.Construct (args);
      }

      public Type Type => _typeInfoImplementation.Type;

      public ITypeInfo BaseType => _typeInfoImplementation.BaseType;

      public string Name => _typeInfoImplementation.Name;

      public string FullName => _typeInfoImplementation.FullName;

      public Assembly Assembly => _typeInfoImplementation.Assembly;

      public string Namespace => _typeInfoImplementation.Namespace;

      public bool IsAbstract => _typeInfoImplementation.IsAbstract;

      public bool IsGenericType => _typeInfoImplementation.IsGenericType;

      public bool ContainsGenericParameters => _typeInfoImplementation.ContainsGenericParameters;

      public bool IsGenericTypeDefinition => _typeInfoImplementation.IsGenericTypeDefinition;

      public bool IsSealed => _typeInfoImplementation.IsSealed;

      public bool IsStaticClass => _typeInfoImplementation.IsStaticClass;
    }

    public class AttributeProvidingMethodInfoDecorator : IMethodInfo
    {
      private readonly IMethodInfo _innerMethodInfo;
      private readonly object[] _myAttributes;

      public AttributeProvidingMethodInfoDecorator (IMethodInfo innerMethodInfo, params object[] myAttributes)
      {
        _innerMethodInfo = innerMethodInfo;
        _myAttributes = myAttributes;
      }

      public T[] GetCustomAttributes<T> (bool inherit)
          where T : class
      {
        var matchingTypes = _myAttributes.Where (a => a is T).Cast<T>().ToArray();
        return _innerMethodInfo.GetCustomAttributes<T> (inherit).Concat (matchingTypes).ToArray();
      }

      public bool IsDefined<T> (bool inherit)
          where T : class
      {
        return _innerMethodInfo.IsDefined<T> (inherit);
      }

      public IParameterInfo[] GetParameters ()
      {
        return _innerMethodInfo.GetParameters();
      }

      public Type[] GetGenericArguments ()
      {
        return _innerMethodInfo.GetGenericArguments();
      }

      public IMethodInfo MakeGenericMethod (params Type[] typeArguments)
      {
        return _innerMethodInfo.MakeGenericMethod (typeArguments);
      }

      public object Invoke (object fixture, params object[] args)
      {
        return _innerMethodInfo.Invoke (fixture, args);
      }

      public ITypeInfo TypeInfo => _innerMethodInfo.TypeInfo;
      public MethodInfo MethodInfo => _innerMethodInfo.MethodInfo;
      public string Name => _innerMethodInfo.Name;
      public bool IsAbstract => _innerMethodInfo.IsAbstract;
      public bool IsPublic => _innerMethodInfo.IsPublic;
      public bool ContainsGenericParameters => _innerMethodInfo.ContainsGenericParameters;
      public bool IsGenericMethod => _innerMethodInfo.IsGenericMethod;
      public bool IsGenericMethodDefinition => _innerMethodInfo.IsGenericMethodDefinition;
      public ITypeInfo ReturnType => _innerMethodInfo.ReturnType;
    }


    [TearDown]
    public void IntegrationTestTearDown ()
    {
      var hasSucceeded = TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed;

      _webTestHelper.OnTearDown (hasSucceeded);
    }

    [OneTimeTearDown]
    public void IntegrationTestTestFixtureTearDown ()
    {
      _webTestHelper.OnFixtureTearDown();
    }

    protected WxePageObject Start (string userControl)
    {
      var userControlUrl = string.Format ("Controls/{0}UserControl.ascx", userControl);

      var url = string.Format ("{0}ControlTest.wxe?UserControl={1}", s_webApplicationRoot.Value, userControlUrl);
      _webTestHelper.MainBrowserSession.Window.Visit (url);
      _webTestHelper.AcceptPossibleModalDialog();

      return _webTestHelper.CreateInitialPageObject<WxePageObject> (_webTestHelper.MainBrowserSession);
    }

    private static void KillAnyExistingWindowsErrorReportingProcesses ()
    {
      ProcessUtils.KillAllProcessesWithName ("WerFault");
    }
  }
}