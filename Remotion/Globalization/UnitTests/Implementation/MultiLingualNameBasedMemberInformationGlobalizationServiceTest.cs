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
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Globalization.UnitTests.Implementation
{
  [TestFixture]
  public class MultiLingualNameBasedMemberInformationGlobalizationServiceTest
  {
    [Test]
    public void TryGetTypeDisplayName_WithMultilingualNameAttribute_ReturnsTheName ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (
              new[]
              {
                  new MultiLingualNameAttribute ("The Name", "")
              });

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.True);
      Assert.That (multiLingualName, Is.EqualTo ("The Name"));
    }

    [Test]
    public void TryGetTypeDisplayName_WithoutMultilingualNameAttribute_ReturnsNull ()
    {
      var service = new MultiLingualNameBasedMemberInformationGlobalizationService();

      var typeInformationStub = MockRepository.GenerateStub<ITypeInformation>();
      typeInformationStub
          .Stub (_ => _.GetCustomAttributes<MultiLingualNameAttribute> (false))
          .Return (new MultiLingualNameAttribute[0]);

      var typeInformationForResourceResolutionStub = MockRepository.GenerateStub<ITypeInformation>();

      string multiLingualName;

      var result = service.TryGetTypeDisplayName (typeInformationStub, typeInformationForResourceResolutionStub, out multiLingualName);

      Assert.That (result, Is.False);
      Assert.That (multiLingualName, Is.Null);
    }
  }
}