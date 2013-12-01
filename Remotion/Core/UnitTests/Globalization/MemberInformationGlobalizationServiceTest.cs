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
using NUnit.Framework;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.UnitTests.Globalization.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class MemberInformationGlobalizationServiceTest
  {
    private IGlobalizationService _globalizationServiceMock1;
    private IGlobalizationService _globalizationServiceMock2;
    private MemberInformationGlobalizationService _service;
    private ITypeInformation _typeInformationForResourceResolutionStub;
    private ITypeInformation _typeInformationStub;
    private IPropertyInformation _propertyInformationStub;
    private IResourceManager _resourceManager1Mock;
    private IResourceManager _resourceManager2Mock;
    private IMemberInformationNameResolver _memberInformationNameResolverStub;
    private string _shortPropertyResourceID;
    private string _longPropertyResourceID;
    private string _shortTypeResourceID;
    private string _longTypeResourceID;

    [SetUp]
    public void SetUp ()
    {
      _globalizationServiceMock1 = MockRepository.GenerateStrictMock<IGlobalizationService>();
      _globalizationServiceMock2 = MockRepository.GenerateStrictMock<IGlobalizationService>();
      _resourceManager1Mock = MockRepository.GenerateStrictMock<IResourceManager>();
      _resourceManager1Mock.Stub (stub => stub.IsNull).Return (false);
      _resourceManager1Mock.Stub (stub => stub.Name).Return ("RM1");
      _resourceManager2Mock = MockRepository.GenerateStrictMock<IResourceManager>();
      _resourceManager2Mock.Stub (stub => stub.IsNull).Return (false);
      _resourceManager2Mock.Stub (stub => stub.Name).Return ("RM2");

      _typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationStub.Stub (stub => stub.Name).Return ("TypeName");

      _typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();
      _typeInformationForResourceResolutionStub.Stub (stub => stub.Name).Return ("TypeNameForResourceResolution");

      _propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation>();
      _propertyInformationStub.Stub (stub => stub.Name).Return ("PropertyName");

      _memberInformationNameResolverStub = MockRepository.GenerateStub<IMemberInformationNameResolver>();
      _memberInformationNameResolverStub.Stub (stub => stub.GetPropertyName (_propertyInformationStub)).Return ("FakePropertyFullName");
      _memberInformationNameResolverStub.Stub (stub => stub.GetTypeName (_typeInformationStub)).Return ("FakeTypeFullName");

      _shortPropertyResourceID = "property:PropertyName";
      _longPropertyResourceID = "property:FakePropertyFullName";
      _shortTypeResourceID = "type:TypeName";
      _longTypeResourceID = "type:FakeTypeFullName";

      _service = new MemberInformationGlobalizationService (
          new CompoundGlobalizationService(new [] { _globalizationServiceMock1, _globalizationServiceMock2 }), _memberInformationNameResolverStub);
    }

    [Test]
    public void GetPropertyDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("PropertyName"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByFirstResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundBySecondResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByBothResourceManager_LongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);
      _resourceManager2Mock.Stub (stub => stub.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByFirstResourceManager_ShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (false);
      
      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundBySecondResourceManager_ShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longPropertyResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortPropertyResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetPropertyDisplayName_ResourceFoundByBothResourceManager_LongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Stub (stub => stub.ContainsString (_shortPropertyResourceID)).Return (true);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is(_longPropertyResourceID), out Arg<string>.Out("Test").Dummy)).Return (true);
      _resourceManager2Mock.Stub (stub => stub.ContainsString (_longPropertyResourceID)).Return (true);
      _resourceManager2Mock.Stub (stub => stub.ContainsString (_shortPropertyResourceID)).Return (true);
      
      var result = _service.GetPropertyDisplayName (_propertyInformationStub, _typeInformationStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo("Test"));
    }

    [Test]
    public void GetTypeDisplayName_NoResourceFound ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);

      var result = _service.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("TypeName"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);

      var result = _service.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByShortResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is(_shortTypeResourceID), out Arg<string>.Out ("Test").Dummy)).Return (true);
      _resourceManager2Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out (null).Dummy)).Return (false);
      
      var result = _service.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("Test"));
    }

    [Test]
    public void GetTypeDisplayName_ResourceFoundByLongAndShortResourceID_ReturnedByLongResourceID ()
    {
      _globalizationServiceMock1.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager2Mock);
      _globalizationServiceMock2.Expect (mock => mock.GetResourceManager (_typeInformationForResourceResolutionStub)).Return (_resourceManager1Mock);

      _resourceManager1Mock.Expect (mock => mock.TryGetString (Arg.Is (_longTypeResourceID), out Arg<string>.Out ("TestLong").Dummy)).Return (true);
      _resourceManager1Mock.Stub (stub => stub.TryGetString (Arg.Is (_shortTypeResourceID), out Arg<string>.Out ("TestShort").Dummy)).Return (true);
      
      var result = _service.GetTypeDisplayName (_typeInformationStub, _typeInformationForResourceResolutionStub);

      _globalizationServiceMock1.VerifyAllExpectations();
      _globalizationServiceMock2.VerifyAllExpectations ();
      _resourceManager1Mock.VerifyAllExpectations();
      _resourceManager2Mock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("TestLong"));
    }
  }
}