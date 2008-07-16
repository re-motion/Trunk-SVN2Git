/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Context;
using Remotion.Design;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.BridgeImplementations;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject.BindableObjectDataSourceTests
{
  [TestFixture]
  public class DesignTime : TestBase
  {
    private BindableObjectDataSource _dataSource;
    private MockRepository _mockRepository;
    private ISite _stubSite;
    private IDesignerHost _mockDesignerHost;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _dataSource = new BindableObjectDataSource();

      _mockRepository = new MockRepository();
      _stubSite = _mockRepository.Stub<ISite>();
      SetupResult.For (_stubSite.DesignMode).Return (true);
      _dataSource.Site = _stubSite;

      _mockDesignerHost = _mockRepository.CreateMock<IDesignerHost>();
      SetupResult.For (_stubSite.GetService (typeof (IDesignerHost))).Return (_mockDesignerHost);

      IDesignModeHelper helperStub = _mockRepository.Stub<IDesignModeHelper> ();
      SetupResult.For (helperStub.DesignerHost).Return (_mockDesignerHost);

      DesignerUtility.SetDesignMode (helperStub);

      PrepareMixinConfiguration (_mockDesignerHost);
    }

    private void PrepareMixinConfiguration (IDesignerHost host)
    {
      SetupResult.For (host.GetType ("Remotion.Mixins.BridgeImplementations.TypeUtilityImplementation, Remotion, Version = 1.9.0.202"))
          .Return (typeof (TypeUtilityImplementation));
      SetupResult.For (host.GetType ("Remotion.Mixins.BridgeImplementations.TypeFactoryImplementation, Remotion, Version = 1.9.0.202"))
          .Return (typeof (TypeFactoryImplementation));
      SetupResult.For (host.GetType ("Remotion.Context.BootstrapStorageProvider, Remotion, Version = 1.9.0.202"))
          .Return (typeof (BootstrapStorageProvider));
      SetupResult.For (host.GetType ("Remotion.Mixins.BridgeImplementations.MixedObjectInstantiator, Remotion, Version = 1.9.0.202"))
          .Return (typeof (MixedObjectInstantiator));
      ITypeDiscoveryService serviceStub = _mockRepository.Stub<ITypeDiscoveryService> ();
      SetupResult.For (serviceStub.GetTypes (null, false)).IgnoreArguments ().Return (Assembly.GetExecutingAssembly ().GetTypes ());
      SetupResult.For (host.GetService (typeof (ITypeDiscoveryService))).Return (serviceStub);
      SetupResult.For (host.GetType ("Remotion.Context.CallContextStorageProvider, Remotion, Version = 1.9.0.202"))
          .Return (typeof (CallContextStorageProvider));
    }

    [TearDown]
    public override void TearDown ()
    {
      base.TearDown();
      DesignerUtility.ClearDesignMode ();
    }

    [Test]
    public void GetAndSetType ()
    {
      Expect.Call (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      Assert.That (_dataSource.Type, Is.Null);
      _dataSource.Type = typeof (SimpleBusinessObjectClass);
      Assert.That (_dataSource.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetType_WithNull ()
    {
      _mockRepository.ReplayAll();

      _dataSource.Type = null;
      Assert.That (_dataSource.Type, Is.Null);

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass ()
    {
      Expect.Call (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual.BusinessObjectProvider, Is.SameAs (BindableObjectProvider.GetProviderForBindableObjectType (typeof (SimpleBusinessObjectClass))));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetBusinessObjectClass_SameTwice ()
    {
      SetupResult.For (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleBusinessObjectClass, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleBusinessObjectClass));
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleBusinessObjectClass);

      IBusinessObjectClass actual = _dataSource.BusinessObjectClass;
      Assert.That (actual, Is.Not.Null);
      Assert.That (actual, Is.SameAs (_dataSource.BusinessObjectClass));

      _mockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "The type 'Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleReferenceType' does not have the "
        + "'Remotion.ObjectBinding.BusinessObjectProviderAttribute' applied.\r\nParameter name: type")]
    public void GetBusinessObjectClass_WithTypeNotUsingBindableObjectMixin ()
    {
      Expect.Call (
          _mockDesignerHost.GetType (
              "Remotion.ObjectBinding.UnitTests.Core.TestDomain.SimpleReferenceType, Remotion.ObjectBinding.UnitTests"))
          .Return (typeof (SimpleReferenceType))
          .Repeat.AtLeastOnce();
      _mockRepository.ReplayAll();

      _dataSource.Type = typeof (SimpleReferenceType);
      Dev.Null = _dataSource.BusinessObjectClass;

      _mockRepository.VerifyAll();
    }
  }
}
