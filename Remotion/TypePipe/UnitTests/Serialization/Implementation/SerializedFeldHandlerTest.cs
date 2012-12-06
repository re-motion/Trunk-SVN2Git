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
using System.Reflection;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.Serialization.Implementation;
using Remotion.TypePipe.UnitTests.MutableReflection;

namespace Remotion.TypePipe.UnitTests.Serialization.Implementation
{
  [TestFixture]
  public class SerializedFeldHandlerTest
  {
    private SerializedFeldHandler _filter;

    [SetUp]
    public void SetUp ()
    {
      _filter = new SerializedFeldHandler();
    }

    [Test]
    public void GetSerializedFieldMapping_Filtering ()
    {
      var field1 = NormalizingMemberInfoFromExpressionUtility.GetField (() => s_staticField);
      var field2 = NormalizingMemberInfoFromExpressionUtility.GetField (() => _instanceField);
      var field3 = NormalizingMemberInfoFromExpressionUtility.GetField (() => _nonSerializedField);

      var result = _filter.GetSerializedFieldMapping (new[] { field1, field2, field3 });

      Assert.That (result, Is.EqualTo (new[] { Tuple.Create ("<tp>_instanceField", field2) }));
    }

    [Test]
    public void GetSerializedFieldMapping_SameName ()
    {
      FieldInfo field1 = MutableFieldInfoObjectMother.Create (name: "abc", type: typeof (int));
      FieldInfo field2 = MutableFieldInfoObjectMother.Create (name: "abc", type: typeof (string));

      var result = _filter.GetSerializedFieldMapping (new[] { field1, field2 });

      Assert.That (result, Is.EqualTo (new[] { Tuple.Create ("<tp>abc@System.Int32", field1), Tuple.Create ("<tp>abc@System.String", field2) }));
    }

    private static int s_staticField = 0;
    private int _instanceField = 0;
    [NonSerialized]
    private int _nonSerializedField = 0;
  }
}