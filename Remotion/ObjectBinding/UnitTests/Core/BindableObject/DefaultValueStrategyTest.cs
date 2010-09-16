// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DefaultValueStrategyTest : TestBase
  {
    [Test]
    public void IsDefaultValue ()
    {
      var businessObjectStub = MockRepository.GenerateStub<IBusinessObject>();
      var propertyInformationStub = MockRepository.GenerateStub<IPropertyInformation> ();
      propertyInformationStub.Stub (stub => stub.PropertyType).Return (typeof (bool));
      var property = CreateProperty (propertyInformationStub);
      var strategy = new DefaultValueStrategy();

      Assert.That (strategy.IsDefaultValue (businessObjectStub, property), Is.False);
    }

    private BooleanProperty CreateProperty (IPropertyInformation propertyInformation)
    {
      var businessObjectProvider = CreateBindableObjectProviderWithStubBusinessObjectServiceFactory ();
      return new BooleanProperty (GetPropertyParameters (propertyInformation, businessObjectProvider));
    }
  }
}