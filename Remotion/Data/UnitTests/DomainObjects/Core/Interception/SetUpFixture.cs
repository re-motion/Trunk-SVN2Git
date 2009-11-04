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
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static string s_assemblyDirectory;
    private static InterceptedDomainObjectTypeFactory s_factory;

    [SetUp]
    public void SetUp()
    {
      s_assemblyDirectory = Path.Combine (Environment.CurrentDirectory, "Interception.TestDlls");
      s_factory = new InterceptedDomainObjectTypeFactory (s_assemblyDirectory);
    }

    [TearDown]
    public void TearDown()
    {
      s_assemblyDirectory = null;
      s_factory = null;
    }

    public static string AssemblyDirectory
    {
      get
      {
        if (s_assemblyDirectory == null)
          throw new InvalidOperationException ("SetUp must be executed first");
        return s_assemblyDirectory; 
      }
    }

    public static InterceptedDomainObjectTypeFactory Factory
    {
      get
      {
        if (s_factory == null)
          throw new InvalidOperationException ("SetUp must be executed first");
        return s_factory;
      }
    }

    public static void SetupAssemblyDirectory()
    {
      CleanupAssemblyDirectory ();
      Directory.CreateDirectory (AssemblyDirectory);
    }

    public static void CleanupAssemblyDirectory()
    {
      if (Directory.Exists (AssemblyDirectory))
        Directory.Delete (InterceptedDomainObjectFactoryTest.AssemblyDirectory, true);
      Assert.That (Directory.Exists (AssemblyDirectory), Is.False);
    }
  }
}
