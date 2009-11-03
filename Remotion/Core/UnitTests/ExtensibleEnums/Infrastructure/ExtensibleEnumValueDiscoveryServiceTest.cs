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
using System.ComponentModel.Design;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.UnitTests.ExtensibleEnums.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ExtensibleEnums.Infrastructure
{
  public class ExtensibleEnumValueDiscoveryServiceTest
  {
    [Test]
    public void GetValues ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService>();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var values = service.GetValues (new ExtensibleEnumDefinition<Color> (service));
      Assert.That (values, 
          Is.EquivalentTo (new[] { Color.Values.Red (), Color.Values.Green (), Color.Values.RedMetallic () }));
    }

    [Test]
    public void GetValues_PassesDefinition_ToExtensionMethod ()
    {
      var typeDiscoveryServiceStub = MockRepository.GenerateStub<ITypeDiscoveryService> ();
      typeDiscoveryServiceStub.Stub (stub => stub.GetTypes (null, false)).Return (new[] { typeof (ColorExtensions) });

      var service = new ExtensibleEnumValueDiscoveryService (typeDiscoveryServiceStub);
      var definition = new ExtensibleEnumDefinition<Color> (service);

      service.GetValues (definition);

      Assert.That (ColorExtensions.LastCallArgument, Is.SameAs (definition));
    }
  }
}