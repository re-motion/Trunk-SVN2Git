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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using Rhino.Mocks;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.TypePipe.UnitTests.MutableReflection
{
  [TestFixture]
  public class MutableTypeMemberCollectionTest
  {
    private const BindingFlags c_all = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

    private MutableType _declaringType;
    private MethodInfo _excludedDeclaredMember;
    private MethodInfo[] _declaredMembers;
    private MethodInfo[] _baseMembers;
    private MethodInfo[] _allExistingMembers;
    private Func<MethodInfo, MutableMethodInfo> _mutableMemberProvider;

    private MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> _collection;

    [SetUp]
    public void SetUp ()
    {
      _declaringType = MutableTypeObjectMother.CreateForExistingType(typeof(DomainType));
      _excludedDeclaredMember = NormalizingMemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.ExcludedMember());
      var allDeclaredMembers = typeof (DomainType).GetMethods (c_all | BindingFlags.DeclaredOnly);
      _declaredMembers = allDeclaredMembers.Except (new[] { _excludedDeclaredMember }).ToArray();
      _baseMembers = typeof (DomainType).GetMethods (c_all).Except (allDeclaredMembers).ToArray();
      _allExistingMembers = _declaredMembers.Concat (_baseMembers).ToArray();
      _mutableMemberProvider = mi => MutableMethodInfoObjectMother.CreateForExisting (_declaringType, mi);

      _collection = new MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> (
          _declaringType, _allExistingMembers.AsOneTime(), _mutableMemberProvider);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_declaredMembers, Is.Not.Empty);
      Assert.That (_baseMembers, Is.Not.Empty);

      Assert.That (_collection.AddedMembers, Is.Empty);
      Assert.That (_collection.ExistingDeclaredMembers.Select (mm => mm.UnderlyingSystemMethodInfo), Is.EqualTo(_declaredMembers));
      Assert.That (_collection.ExistingBaseMembers, Is.EqualTo (_baseMembers));
    }

    [Test]
    public void AllMutableMembers ()
    {
      var addedMember = CreateMutableMember ();
      _collection.Add (addedMember);
      Assert.That (_collection.AddedMembers, Has.Count.EqualTo (1));
      Assert.That (_collection.ExistingDeclaredMembers, Has.Count.EqualTo (1));
      var declaredMember = _collection.ExistingDeclaredMembers.Single ();

      Assert.That (_collection.AllMutableMembers, Is.EqualTo (new[] { declaredMember, addedMember }));
    }

    [Test]
    public void GetEnumerator ()
    {
      var addedMember = CreateMutableMember ();
      _collection.Add (addedMember);
      Assert.That (_collection.AddedMembers, Has.Count.EqualTo (1));
      Assert.That (_collection.ExistingDeclaredMembers, Has.Count.EqualTo(1));
      var declaredMember = _collection.ExistingDeclaredMembers.Single();
      Assert.That (_collection.ExistingBaseMembers, Is.Not.Empty);
      
      IEnumerable<MethodInfo> enumerable = _collection;

      var expectedAllMembers = new MethodInfo[] { declaredMember, addedMember }.Concat (_collection.ExistingBaseMembers);
      Assert.That (enumerable, Is.EqualTo (expectedAllMembers));
    }

    [Test]
    public void GetMutableMember_MutableMemeberInfo ()
    {
      var mutableMember = CreateMutableMember();
      _collection.Add (mutableMember);

      var result = _collection.GetMutableMember(mutableMember);

      Assert.That (result, Is.SameAs (mutableMember));
    }

    [Test]
    public void GetMutableMember_StandardMemberInfo ()
    {
      var standardMember = _declaredMembers.First();
      Assert.That (standardMember, Is.Not.AssignableTo<MutableMethodInfo> ());

      var result = _collection.GetMutableMember(standardMember);

      var expectedMutableMember = _collection.ExistingDeclaredMembers.Single (m => m.UnderlyingSystemMethodInfo == standardMember);
      Assert.That (result, Is.SameAs (expectedMutableMember));
    }

    [Test]
    public void GetMutableMember_StandardMemberInfo_NoMatch ()
    {
      var collection = new MutableTypeMemberCollection<MethodInfo, MutableMethodInfo> (_declaringType, _allExistingMembers, _mutableMemberProvider);
      var result = collection.GetMutableMember (_excludedDeclaredMember);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void Add ()
    {
      var mutableMember = CreateMutableMember ();

      _collection.Add (mutableMember);

      Assert.That (_collection.AddedMembers, Is.EqualTo (new[] { mutableMember }));
    }

    private MutableMethodInfo CreateMutableMember (
        string name = "UnspecifiedMember",
        Type returnType = null,
        IEnumerable<ParameterDeclaration> parameterDeclarations = null)
    {
      return MutableMethodInfoObjectMother.Create (
          declaringType: _declaringType,
          name: name,
          returnType: returnType,
          parameterDeclarations: parameterDeclarations);
    }

    public class DomainType
    {
      public void Member () { }
      public void ExcludedMember () { }
    }
  }
}