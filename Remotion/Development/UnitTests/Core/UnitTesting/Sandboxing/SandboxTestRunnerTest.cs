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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Sandboxing;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Sandboxing
{
  [TestFixture]
  public class SandboxTestRunnerTest
  {
    private SandboxTestRunner _sandboxTestRunner;
    private Type[] _testFixtureTypes;

    [SetUp]
    public void SetUp ()
    {
      _sandboxTestRunner = new SandboxTestRunner();
      _testFixtureTypes = new[] { typeof (DummyTest1) };
    }

    [Test]
    public void RunTestsInSandbox ()
    {
      var permissions = PermissionSets.GetMediumTrust (AppDomain.CurrentDomain.BaseDirectory, Environment.MachineName);
      var testResults =
          SandboxTestRunner.RunTestFixturesInSandbox (_testFixtureTypes, permissions, new[] { typeof (SandboxTestRunnerTest).Assembly }).SelectMany (
              r => r.TestResults).Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixtures ()
    {
      var testResults =
          _sandboxTestRunner.RunTestFixtures (_testFixtureTypes).SelectMany (r => r.TestResults).Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count(), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed();
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RunTestFixtures_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixtures (null);
    }

    [Test]
    public void RunTestFixture_WithSetupAndTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest1)).TestResults.Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count(), Is.EqualTo (3));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed();
    }

    [Test]
    public void RunTestFixture_WithoutSetupAndTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest2)).TestResults.Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (1));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixture_WithoutTearDownMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest3)).TestResults.Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (2));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    public void RunTestFixture_WithoutSetupMethod ()
    {
      var testResults = _sandboxTestRunner.RunTestFixture (typeof (DummyTest4)).TestResults.Where (r => r.Status != TestStatus.Ignored);

      Assert.That (testResults.Count (), Is.EqualTo (2));
      foreach (var testResult in testResults)
        testResult.EnsureNotFailed ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RunTestFixture_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixture (null);
    }
  }
}