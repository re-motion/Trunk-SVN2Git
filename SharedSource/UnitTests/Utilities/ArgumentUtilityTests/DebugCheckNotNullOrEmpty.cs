// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 

using System;
using System.Collections;
using System.Diagnostics;
using NUnit.Framework;
using Remotion.Utilities;

// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class DebugCheckNotNullOrEmpty
  {
    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullString ()
    {
      const string value = null;
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", value);
    }

    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyString ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", "");
    }

    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyArray ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", new string[0]);
    }

    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyCollection ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", new ArrayList());
    }

    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_EmptyIEnumerable ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", GetEmptyEnumerable());
    }

    [Conditional ("DEBUG")]
    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'arg' cannot be empty.\r\nParameter name: arg")]
    public void Fail_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (false);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
    }

    [Conditional ("DEBUG")]
    [Test]
    public void Succeed_String ()
    {
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", "Test");
    }

    [Conditional ("DEBUG")]
    [Test]
    public void Succeed_Array ()
    {
      var array = new[] { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", array);
    }

    [Test]
    public void Succeed_Collection ()
    {
      var list = new ArrayList { "test" };
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", list);
    }

    [Test]
    public void Succeed_IEnumerable ()
    {
      IEnumerable enumerable = GetEnumerableWithValue();
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
    }

    [Test]
    public void Succeed_NonDisposableEnumerable ()
    {
      IEnumerable enumerable = new NonDisposableEnumerable (true);
      ArgumentUtility.DebugCheckNotNullOrEmpty ("arg", enumerable);
    }

    private IEnumerable GetEnumerableWithValue ()
    {
      yield return "test";
    }

    private IEnumerable GetEmptyEnumerable ()
    {
      yield break;
    }
  }
}