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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Logging;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class StopwatchScopeTest
  {
    [Test]
    public void CreateScope_WithAction ()
    {
      var result = TimeSpan.Zero;
      using (var scope = StopwatchScope.CreateScope (time => result = time))
      {
        while (scope.Elapsed <= TimeSpan.FromMilliseconds (100.0))
        {
        }
      }
      Assert.That (result, Is.GreaterThan (TimeSpan.FromMilliseconds (100.0)));
      Assert.That (result, Is.LessThan (TimeSpan.FromSeconds (10.0)));
    }

    [Test]
    public void CreateScope_Writer ()
    {
      var writerMock = MockRepository.GenerateMock<TextWriter> ();

      var scope = StopwatchScope.CreateScope (writerMock, "Elapsed: {0}");
      while (scope.Elapsed <= TimeSpan.FromMilliseconds (100.0))
      {
      }
      scope.Dispose ();

      writerMock.AssertWasCalled (mock => mock.WriteLine (
          Arg.Is ("Elapsed: {0}"),
          Arg<object>.Matches (obj => obj.Equals (scope.Elapsed.ToString ()))));
    }

    [Test]
    public void CreateScope_Writer_Milliseconds ()
    {
      var writerMock = MockRepository.GenerateMock<TextWriter> ();
      
      var scope = StopwatchScope.CreateScopeForMilliseconds (writerMock, "Elapsed: {0}");
      while (scope.Elapsed <= TimeSpan.FromMilliseconds (100.0))
      {
      }
      scope.Dispose();
      
      writerMock.AssertWasCalled (mock => mock.WriteLine (
          Arg.Is ("Elapsed: {0}"),
          Arg<object>.Matches (obj => obj.Equals (scope.Elapsed.TotalMilliseconds))));
    }

    [Test]
    public void CreateScope_ILog ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      var scope = StopwatchScope.CreateScope (logMock, LogLevel.Debug, "Elapsed: {0}");
      while (scope.Elapsed <= TimeSpan.FromMilliseconds (100.0))
      {
      }
      scope.Dispose ();

      logMock.AssertWasCalled (mock => mock.LogFormat (
          Arg.Is (LogLevel.Debug),
          Arg.Is ("Elapsed: {0}"),
          Arg<object[]>.Matches (objs => objs[0].Equals (scope.Elapsed.ToString()))));
    }

    [Test]
    public void CreateScope_ILog_Milliseconds ()
    {
      var logMock = MockRepository.GenerateMock<ILog> ();

      var scope = StopwatchScope.CreateScopeForMilliseconds (logMock, LogLevel.Debug, "Elapsed: {0}");
      while (scope.Elapsed <= TimeSpan.FromMilliseconds (100.0))
      {
      }
      scope.Dispose ();

      logMock.AssertWasCalled (mock => mock.LogFormat (
          Arg.Is (LogLevel.Debug),
          Arg.Is ("Elapsed: {0}"),
          Arg<object[]>.Matches (objs => objs[0].Equals (scope.Elapsed.TotalMilliseconds))));
    }
  }
}