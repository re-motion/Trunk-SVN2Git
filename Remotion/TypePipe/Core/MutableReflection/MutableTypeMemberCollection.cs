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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// A container storing members and providing convenience properties for <see cref="ExistingDeclaredMembers"/> <see cref="ExistingBaseMembers"/>
  /// and <see cref="AddedMembers"/>.
  /// <see cref="GetMutableMember"/> can be used to retrieve the mutable version for an existing member.
  /// This class is an implementation detail of <see cref="MutableType"/>.
  /// </summary>
  /// <typeparam name="TMemberInfo">The type of the existing member infos.</typeparam>
  /// <typeparam name="TMutableMemberInfo">The type of the mutable member infos.</typeparam>
  public class MutableTypeMemberCollection<TMemberInfo, TMutableMemberInfo> : IEnumerable<TMemberInfo>
    where TMemberInfo : MemberInfo
    where TMutableMemberInfo : TMemberInfo
  {
    private readonly MutableType _declaringType;
    private readonly ReadOnlyDictionary<TMemberInfo, TMutableMemberInfo> _existingDeclaredMembers;
    private readonly ReadOnlyCollection<TMemberInfo> _existingBaseMembers;
    private readonly List<TMutableMemberInfo> _addedMembers = new List<TMutableMemberInfo>();

    public MutableTypeMemberCollection (
        MutableType declaringType,
        IEnumerable<TMemberInfo> existingMembers,
        Func<TMemberInfo, TMutableMemberInfo> mutableMemberProvider)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("existingMembers", existingMembers);
      ArgumentUtility.CheckNotNull ("mutableMemberProvider", mutableMemberProvider);

      _declaringType = declaringType;

      var declaredMembers = new Dictionary<TMemberInfo, TMutableMemberInfo>();
      var baseMembers = new List<TMemberInfo>();
      foreach (var member in existingMembers)
      {
        if (declaringType.IsEquivalentTo (member.DeclaringType))
          declaredMembers.Add (member, mutableMemberProvider (member));
        else
          baseMembers.Add (member);
      }

      _existingDeclaredMembers = declaredMembers.AsReadOnly();
      _existingBaseMembers = baseMembers.AsReadOnly();
    }

    public ReadOnlyCollectionDecorator<TMutableMemberInfo> ExistingDeclaredMembers
    {
      get { return _existingDeclaredMembers.Values.AsReadOnly(); }
    }

    public ReadOnlyCollection<TMemberInfo> ExistingBaseMembers
    {
      get { return _existingBaseMembers; }
    }

    public ReadOnlyCollection<TMutableMemberInfo> AddedMembers
    {
      get { return _addedMembers.AsReadOnly(); }
    }

    public IEnumerable<TMutableMemberInfo> AllMutableMembers
    {
      get { return ExistingDeclaredMembers.Concat (AddedMembers); }
    }

    public IEnumerator<TMemberInfo> GetEnumerator ()
    {
      return AllMutableMembers.Cast<TMemberInfo>().Concat (FilterBaseMembers(_existingBaseMembers)).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    public TMutableMemberInfo GetMutableMember (TMemberInfo member)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      CheckDeclaringType (_declaringType, member);

      if (member is TMutableMemberInfo)
        return (TMutableMemberInfo) member;

      return _existingDeclaredMembers.GetValueOrDefault (member);
    }

    public void Add (TMutableMemberInfo mutableMember)
    {
      ArgumentUtility.CheckNotNull ("mutableMember", mutableMember);
      CheckDeclaringTypeEquivalence (_declaringType, mutableMember);

      _addedMembers.Add (mutableMember);
    }

    protected virtual IEnumerable<TMemberInfo> FilterBaseMembers (IEnumerable<TMemberInfo> baseMembers)
    {
      return baseMembers;
    }

    protected virtual void CheckDeclaringType (MutableType declaringType, TMemberInfo member)
    {
      CheckDeclaringTypeEquivalence (declaringType, member);
    }

    private void CheckDeclaringTypeEquivalence (MutableType declaringType, TMemberInfo member)
    {
      if (!declaringType.IsEquivalentTo (member.DeclaringType))
      {
        var message = string.Format ("{0} is declared by a different type: '{1}'.", GetMemberTypeName (), member.DeclaringType);
        var parameterName = GetMemberTypeName().ToLower();
        throw new ArgumentException (message, parameterName);
      }
    }

    private string GetMemberTypeName ()
    {
      return typeof (TMemberInfo).Name.Replace ("Info", "");
    }
  }
}