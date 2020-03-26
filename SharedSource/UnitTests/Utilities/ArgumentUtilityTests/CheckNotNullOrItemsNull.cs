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
using NUnit.Framework;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.UnitTests.Utilities.ArgumentUtilityTests
{
  [TestFixture]
  public class CheckNotNullOrItemsNull
  {
    [Test]
    public void Succeed_ICollection ()
    {
      ArrayList list = new ArrayList();
      ArrayList result = ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
      Assert.That (result, Is.SameAs (list));
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void Fail_NullICollection ()
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", (ICollection?) null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Item 0 of parameter 'arg' is null.\r\nParameter name: arg")]
    public void Fail_zItemNullICollection ()
    {
      ArrayList list = new ArrayList();
      list.Add (null);
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", list);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException), ExpectedMessage = "Item 0 of parameter 'arg' is null.\r\nParameter name: arg")]
    public void Fail_zItemNullIEnumerable ()
    {
      ArgumentUtility.CheckNotNullOrItemsNull ("arg", GetEnumerableWithNullValue());
    }

    private IEnumerable GetEnumerableWithNullValue ()
    {
      yield return null;
    }
  }
}