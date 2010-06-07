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
      SandboxTestRunner.RunTestFixturesInSandbox (_testFixtureTypes, permissions, new[] { typeof (SandboxTestRunnerTest).Assembly });

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    public void RunTestFixtures ()
    {
      _sandboxTestRunner.RunTestFixtures (_testFixtureTypes);

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void RunTestFixtures_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixtures (null);
    }

    [Test]
    public void RunTestFixture_WithSetupAndTearDownMethod ()
    {
      _sandboxTestRunner.RunTestFixture (typeof(DummyTest1));

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    public void RunTestFixture_WithoutSetupAndTearDownMethod ()
    {
      _sandboxTestRunner.RunTestFixture (typeof (DummyTest2));

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    public void RunTestFixture_WithoutTearDownMethod ()
    {
      _sandboxTestRunner.RunTestFixture (typeof (DummyTest3));

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    public void RunTestFixture_WithoutSetupMethod ()
    {
      _sandboxTestRunner.RunTestFixture (typeof (DummyTest4));

      // TODO 2857: Assert that the tests have been run by analyzing the test results.
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void RunTestFixture_ArgumentIsNull_ThrowsException ()
    {
      _sandboxTestRunner.RunTestFixture (null);
    }

  }
}