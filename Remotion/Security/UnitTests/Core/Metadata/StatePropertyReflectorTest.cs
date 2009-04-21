// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class StatePropertyReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IEnumerationReflector _enumeratedTypeReflectorMock;
    private StatePropertyReflector _statePropertyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public StatePropertyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _enumeratedTypeReflectorMock = _mocks.StrictMock<IEnumerationReflector> ();
      _statePropertyReflector = new StatePropertyReflector (_enumeratedTypeReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.IsInstanceOfType (typeof (IStatePropertyReflector), _statePropertyReflector);
      Assert.AreSame (_enumeratedTypeReflectorMock, _statePropertyReflector.EnumerationTypeReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Dictionary<Enum, EnumValueInfo> values = new Dictionary<Enum, EnumValueInfo> ();
      values.Add (Confidentiality.Normal, PropertyStates.ConfidentialityNormal);
      values.Add (Confidentiality.Confidential, PropertyStates.ConfidentialityConfidential);
      values.Add (Confidentiality.Private, PropertyStates.ConfidentialityPrivate);

      Expect.Call (_enumeratedTypeReflectorMock.GetValues (typeof (Confidentiality), _cache)).Return (values);
      _mocks.ReplayAll ();

      StatePropertyInfo info = _statePropertyReflector.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache);

      _mocks.VerifyAll ();

      Assert.IsNotNull (info);
      Assert.AreEqual ("Confidentiality", info.Name);
      Assert.AreEqual ("00000000-0000-0000-0001-000000000001", info.ID);
      
      Assert.IsNotNull (info.Values);
      Assert.AreEqual (3, info.Values.Count);
      Assert.Contains (PropertyStates.ConfidentialityNormal, info.Values);
      Assert.Contains (PropertyStates.ConfidentialityPrivate, info.Values);
      Assert.Contains (PropertyStates.ConfidentialityConfidential, info.Values);
    }

    [Test]
    public void GetMetadataFromCache ()
    {
      StatePropertyReflector reflector = new StatePropertyReflector ();
      reflector.GetMetadata (typeof (PaperFile).GetProperty ("Confidentiality"), _cache);
      reflector.GetMetadata (typeof (File).GetProperty ("Confidentiality"), _cache);

      StatePropertyInfo paperFileConfidentialityInfo = _cache.GetStatePropertyInfo (typeof (PaperFile).GetProperty ("Confidentiality"));
      Assert.IsNotNull (paperFileConfidentialityInfo);
      Assert.AreEqual ("Confidentiality", paperFileConfidentialityInfo.Name);
      Assert.AreSame (paperFileConfidentialityInfo, _cache.GetStatePropertyInfo (typeof (File).GetProperty ("Confidentiality")));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type of the property 'ID' in type 'Remotion.Security.UnitTests.TestDomain.File' is not an enumerated type.\r\nParameter name: property")]
    public void GetMetadataWithInvalidType ()
    {
      new StatePropertyReflector().GetMetadata (typeof (PaperFile).GetProperty ("ID"), _cache);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type of the property 'SimpleEnum' in type 'Remotion.Security.UnitTests.TestDomain.File' does not have the Remotion.Security.SecurityStateAttribute applied.\r\nParameter name: property")]
    public void GetMetadataWithInvalidEnum ()
    {
      new StatePropertyReflector ().GetMetadata (typeof (PaperFile).GetProperty ("SimpleEnum"), _cache);
    }
  }
}
