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
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.Core.TableInheritance.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;
using File=System.IO.File;

namespace Remotion.Data.DomainObjects.UnitTests.Core
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    [Test]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");

      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
          ReflectionUtility.GetPropertyName (propertyInfo));
    }
       
    [Test]
    public void GetAssemblyPath ()
    {
      Assert.AreEqual (AppDomain.CurrentDomain.BaseDirectory, ReflectionUtility.GetAssemblyDirectory (typeof (ReflectionUtilityTest).Assembly));
    }

    [Test]
    public void GetAssemblyPath_WithHashInDirectoryName ()
    {
      string directoryPath = Path.Combine (AppDomain.CurrentDomain.BaseDirectory, "#HashTestPath");
      string originalAssemblyPath = typeof (ReflectionUtilityTest).Assembly.Location;
      string newAssemblyPath = Path.Combine (directoryPath, Path.GetFileName (originalAssemblyPath));

      if (Directory.Exists (directoryPath))
        Directory.Delete (directoryPath, true);

      Directory.CreateDirectory (directoryPath);
      try
      {
        File.Copy (originalAssemblyPath, newAssemblyPath);
        AppDomainRunner.Run (
            delegate (object[] args)
            {
              string directory = (string) args[0];
              string assemblyPath = (string) args[1];

              Assembly assembly = Assembly.LoadFile (assemblyPath);
              Assert.AreEqual (directory, Path.GetDirectoryName (assembly.Location));
              Assert.AreEqual (directory, ReflectionUtility.GetAssemblyDirectory (assembly));
            }, directoryPath, newAssemblyPath);
      }
      finally
      {
        Directory.Delete (directoryPath, true);
      }
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
        ExpectedMessage = "The assembly's code base 'http://server/File.ext' is not a local path.")]
    public void GetAssemblyPath_FromNonLocalUri ()
    {
      MockRepository mockRepository = new MockRepository();
      _Assembly assemblyMock = mockRepository.CreateMock<_Assembly>();

      SetupResult.For (assemblyMock.EscapedCodeBase).Return ("http://server/File.ext");
      mockRepository.ReplayAll();

      ReflectionUtility.GetAssemblyDirectory (assemblyMock);
    }

    [Test]
    public void GetDomainObjectAssemblyDirectory ()
    {
      Assert.AreEqual (Path.GetDirectoryName (new Uri (typeof (DomainObject).Assembly.EscapedCodeBase).AbsolutePath),
          ReflectionUtility.GetConfigFileDirectory());
    }
  }
}
