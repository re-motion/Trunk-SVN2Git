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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.ExtensibleEnums;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Configuration
{
  [TestFixture]
  public class TypeProviderTest: StandardMappingTest
  {
    private enum TestEnum { }

    [Test]
    public void CheckSupportedTypes()
    {
      TypeProvider typeProvider = new TypeProvider();
      Assert.That (typeProvider.IsTypeSupported (typeof (bool)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (byte)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (DateTime)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (decimal)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (double)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (TestEnum)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (Color)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (Guid)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (short)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (int)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (long)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (float)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (string)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (ObjectID)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (byte[])), Is.True);

      Assert.That (typeProvider.IsTypeSupported (typeof (Enum)), Is.True);
      Assert.That (typeProvider.IsTypeSupported (typeof (IExtensibleEnum)), Is.True);

      Assert.That (typeProvider.IsTypeSupported (typeof (object)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (char)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (char[])), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (DomainObject)), Is.False);
      Assert.That (typeProvider.IsTypeSupported (typeof (DomainObjectCollection)), Is.False);
    }
  }
}