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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.Core.SampleDomain;
using Remotion.Security.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class ClassReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IStatePropertyReflector _statePropertyReflectorMock;
    private IAccessTypeReflector _accessTypeReflectorMock;
    private ClassReflector _classReflector;
    private MetadataCache _cache;
    private StatePropertyInfo _confidentialityProperty;
    private StatePropertyInfo _stateProperty;

    // construction and disposing

    public ClassReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _statePropertyReflectorMock = _mocks.StrictMock<IStatePropertyReflector> ();
      _accessTypeReflectorMock = _mocks.StrictMock<IAccessTypeReflector> ();
      _classReflector = new ClassReflector (_statePropertyReflectorMock, _accessTypeReflectorMock);
      _cache = new MetadataCache ();

      _confidentialityProperty = new StatePropertyInfo ();
      _confidentialityProperty.ID = Guid.NewGuid ().ToString ();
      _confidentialityProperty.Name = "Confidentiality";

      _stateProperty = new StatePropertyInfo ();
      _stateProperty.ID = Guid.NewGuid().ToString();
      _stateProperty.Name = "State";
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IClassReflector), _classReflector);
      Assert.AreSame (_statePropertyReflectorMock, _classReflector.StatePropertyReflector);
      Assert.AreSame (_accessTypeReflectorMock, _classReflector.AccessTypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      List<EnumValueInfo> fileAccessTypes = new List<EnumValueInfo> ();
      fileAccessTypes.Add (AccessTypes.Read);
      fileAccessTypes.Add (AccessTypes.Write);
      fileAccessTypes.Add (AccessTypes.Journalize);

      List<EnumValueInfo> paperFileAccessTypes = new List<EnumValueInfo> ();
      paperFileAccessTypes.Add (AccessTypes.Read);
      paperFileAccessTypes.Add (AccessTypes.Write);
      paperFileAccessTypes.Add (AccessTypes.Journalize);
      paperFileAccessTypes.Add (AccessTypes.Archive);

      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache)).Return (_confidentialityProperty);
      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (PaperFile).GetProperty ("State"), _cache)).Return (_stateProperty);
      Expect.Call (_statePropertyReflectorMock.GetMetadata (typeof (File).GetProperty ("Confidentiality"), _cache)).Return (_confidentialityProperty);
      Expect.Call (_accessTypeReflectorMock.GetAccessTypesFromType (typeof (File), _cache)).Return (fileAccessTypes);
      Expect.Call (_accessTypeReflectorMock.GetAccessTypesFromType(typeof (PaperFile), _cache)).Return (paperFileAccessTypes);
      _mocks.ReplayAll ();

      SecurableClassInfo info = _classReflector.GetMetadata (typeof (PaperFile), _cache);

      _mocks.VerifyAll ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Remotion.Security.UnitTests.TestDomain.PaperFile, Remotion.Security.UnitTests.TestDomain", info.Name);
      Assert.AreEqual ("00000000-0000-0000-0002-000000000000", info.ID);
      
      Assert.AreEqual (0, info.DerivedClasses.Count);
      Assert.IsNotNull (info.BaseClass);
      Assert.AreEqual ("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain", info.BaseClass.Name);
      Assert.AreEqual (1, info.BaseClass.DerivedClasses.Count);
      Assert.Contains (info, info.BaseClass.DerivedClasses);

      Assert.AreEqual (2, info.Properties.Count);
      Assert.Contains (_confidentialityProperty, info.Properties);
      Assert.Contains (_stateProperty, info.Properties);

      Assert.AreEqual (4, info.AccessTypes.Count);
      foreach (EnumValueInfo accessType in paperFileAccessTypes)
        Assert.Contains (accessType, info.AccessTypes);
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      ClassReflector reflector = new ClassReflector ();
      SecurableClassInfo paperFileInfo = reflector.GetMetadata (typeof (PaperFile), _cache);

      Assert.IsNotNull (paperFileInfo);
      Assert.AreEqual (paperFileInfo, _cache.GetSecurableClassInfo (typeof (PaperFile)));

      SecurableClassInfo fileInfo = _cache.GetSecurableClassInfo (typeof (File));
      Assert.IsNotNull (fileInfo);
      Assert.AreEqual ("Remotion.Security.UnitTests.TestDomain.File, Remotion.Security.UnitTests.TestDomain", fileInfo.Name);
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void GetMetadataWithInvalidType ()
    {
      new ClassReflector ().GetMetadata (typeof (Role), _cache);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Value types are not supported.\r\nParameter name: type")]
    public void GetMetadataWithInvalidValueType ()
    {
      new ClassReflector ().GetMetadata (typeof (TestValueType), _cache);
    }
  }
}
