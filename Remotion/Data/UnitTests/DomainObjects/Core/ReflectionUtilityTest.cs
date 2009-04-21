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
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ReflectionUtilityTest : StandardMappingTest
  {
    [Test]
    [Obsolete]
    public void GetPropertyName ()
    {
      PropertyInfo propertyInfo = typeof (DerivedClassWithMixedProperties).GetProperty ("Int32");

      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.ReflectionBasedMappingSample.ClassWithMixedProperties.Int32",
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
      _Assembly assemblyMock = mockRepository.StrictMock<_Assembly>();

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
