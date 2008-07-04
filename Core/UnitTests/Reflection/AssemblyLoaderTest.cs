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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Development.UnitTesting;
using System.Diagnostics;

using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class AssemblyLoaderTest
  {
    private MockRepository _mockRepository;
    private IAssemblyFinderFilter _filterMock;
    private AssemblyLoader _loader;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
      _filterMock = _mockRepository.CreateMock<IAssemblyFinderFilter> ();
      _loader = new AssemblyLoader (_filterMock);
    }

    [Test]
    public void TryLoadAssembly ()
    {
      SetupFilterTrue();

      Assembly referenceAssembly = typeof (AssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.SameAs (referenceAssembly));
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeTrue ()
    {
      Assembly referenceAssembly = typeof (AssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
        .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
        .Return (true);
      Expect.Call (_filterMock.ShouldIncludeAssembly (null))
        .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
        .Return (true);

      _mockRepository.ReplayAll ();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.SameAs (referenceAssembly));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderTrue_IncludeFalse ()
    {
      Assembly referenceAssembly = typeof (AssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
        .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
        .Return (true);
      Expect.Call (_filterMock.ShouldIncludeAssembly (null))
        .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
        .Return (false);

      _mockRepository.ReplayAll ();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.Null);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryLoadAssembly_FilterConsiderFalse ()
    {
      Assembly referenceAssembly = typeof (AssemblyLoaderTest).Assembly;
      string path = new Uri (referenceAssembly.EscapedCodeBase).AbsolutePath;

      Expect.Call (_filterMock.ShouldConsiderAssembly (null))
        .Constraints (Mocks_Property.Value ("FullName", referenceAssembly.FullName))
        .Return (false);

      _mockRepository.ReplayAll ();
      Assembly loadedAssembly = _loader.TryLoadAssembly (path);
      Assert.That (loadedAssembly, Is.Null);
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void TryLoadAssembly_WithBadImageFormatException ()
    {
      SetupFilterTrue ();

      const string path = "Invalid.dll";
      using (File.CreateText (path))
      {
        // no contents
      }

      try
      {
        Assembly loadedAssembly = _loader.TryLoadAssembly (path);
        Assert.That (loadedAssembly, Is.Null);
      }
      finally
      {
        FileUtility.DeleteAndWaitForCompletion (path);
      }
    }

    // Assembly.Load will lock a file when it throws a FileLoadException, making it impossible to restore the previous state
    // for naive tests. We therefore run the actual test in another process using Process.Start; that way, the locked file
    // will be unlocked when the process exits and we can delete it after the test has run.
    [Test]
    public void TryLoadAssembly_WithFileLoadException ()
    {
      string program = 
          Compile("Reflection\\TestAssemblies\\FileLoadExceptionConsoleApplication", "FileLoadExceptionConsoleApplication.exe", true);
      string delaySignAssembly =
          Compile ("Reflection\\TestAssemblies\\DelaySignAssembly", "DelaySignAssembly.dll", false);

      ProcessStartInfo startInfo = new ProcessStartInfo (program);
      startInfo.UseShellExecute = false;
      startInfo.CreateNoWindow = true;
      startInfo.RedirectStandardOutput = true;
      startInfo.Arguments = delaySignAssembly;

      Process process = Process.Start (startInfo);
      string output = process.StandardOutput.ReadToEnd();
      process.WaitForExit ();
      Assert.That (process.ExitCode, Is.EqualTo (0), output);

      FileUtility.DeleteAndWaitForCompletion (program);
      FileUtility.DeleteAndWaitForCompletion (delaySignAssembly);
    }

    [Test]
    public void LoadAssemblies ()
    {
      Assembly referenceAssembly1 = typeof (AssemblyLoaderTest).Assembly;
      Assembly referenceAssembly2 = typeof (AssemblyLoader).Assembly;

      AssemblyLoader loaderPartialMock = _mockRepository.PartialMock<AssemblyLoader> (_filterMock);
      Expect.Call (loaderPartialMock.TryLoadAssembly ("abc")).Return (null);
      Expect.Call (loaderPartialMock.TryLoadAssembly ("def")).Return (referenceAssembly1);
      Expect.Call (loaderPartialMock.TryLoadAssembly ("ghi")).Return (null);
      Expect.Call (loaderPartialMock.TryLoadAssembly ("jkl")).Return (referenceAssembly2);

      _mockRepository.ReplayAll ();

      IEnumerable<Assembly> assemblies = loaderPartialMock.LoadAssemblies ("abc", "def", "ghi", "jkl");
      Assert.That (EnumerableUtility.ToArray (assemblies), Is.EqualTo (new object[] {referenceAssembly1, referenceAssembly2}));
      _mockRepository.VerifyAll ();
    }

    private void SetupFilterTrue ()
    {
      SetupResult.For (_filterMock.ShouldConsiderAssembly (null)).IgnoreArguments ().Return (true);
      SetupResult.For (_filterMock.ShouldIncludeAssembly (null)).IgnoreArguments ().Return (true);

      _mockRepository.ReplayAll ();
    }


    private string Compile (string sourceDirectory, string outputAssemblyName, bool generateExecutable)
    {
      AssemblyCompiler compiler = new AssemblyCompiler(sourceDirectory,
          outputAssemblyName,
          typeof (AssemblyLoader).Assembly.Location);
      compiler.CompilerParameters.GenerateExecutable = generateExecutable;
      compiler.Compile();
      return compiler.OutputAssemblyPath;
    }
  }
}