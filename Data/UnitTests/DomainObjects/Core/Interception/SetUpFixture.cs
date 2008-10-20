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
using System.IO;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [SetUpFixture]
  public class SetUpFixture
  {
    private static string s_assemblyDirectory;
    private static InterceptedDomainObjectFactory s_factory;

    [SetUp]
    public void SetUp()
    {
      s_assemblyDirectory = Path.Combine (Environment.CurrentDirectory, "Interception.TestDlls");
      s_factory = new InterceptedDomainObjectFactory (s_assemblyDirectory);
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

    public static InterceptedDomainObjectFactory Factory
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
      if (!Directory.Exists (InterceptedDomainObjectFactoryTest.AssemblyDirectory))
        Directory.CreateDirectory (InterceptedDomainObjectFactoryTest.AssemblyDirectory);
    }

    public static void CleanupAssemblyDirectory()
    {
      Directory.Delete (InterceptedDomainObjectFactoryTest.AssemblyDirectory, true);
    }
  }
}