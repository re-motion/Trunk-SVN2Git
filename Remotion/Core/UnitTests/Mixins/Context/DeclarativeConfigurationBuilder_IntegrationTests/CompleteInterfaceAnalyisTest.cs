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
using Remotion.Mixins.Context;
using Remotion.UnitTests.Mixins.TestDomain;

namespace Remotion.UnitTests.Mixins.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class CompleteInterfaceAnalyisTest
  {
    [Test]
    public void CompleteInterface_ViaIHasCompleteInterface ()
    {
      var result = new DeclarativeConfigurationBuilder (null).AddType (typeof (ClassWithHasCompleteInterfaces)).BuildConfiguration ();

      var classContext = result.ResolveCompleteInterface (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface1));
      Assert.That (classContext, Is.Not.Null);
      Assert.That (classContext.Type, Is.SameAs (typeof (ClassWithHasCompleteInterfaces)));
      Assert.That (classContext.CompleteInterfaces, List.Contains (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface1)));

      var classContext2 = result.ResolveCompleteInterface (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface2));
      Assert.That (classContext2, Is.SameAs (classContext));
      Assert.That (classContext2.CompleteInterfaces, List.Contains (typeof (ClassWithHasCompleteInterfaces.ICompleteInterface2)));
    }
  }
}