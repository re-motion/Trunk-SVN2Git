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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  /// <summary>
  /// Generates the <see cref="IntroducedMemberAttribute"/> on introduced members.
  /// </summary>
  public class IntroducedMemberAttributeGenerator
  {
    private readonly ConstructorInfo s_introducedMemberAttributeCtor =
        typeof (IntroducedMemberAttribute).GetConstructor (new[] { typeof (Type), typeof (string), typeof (Type), typeof (string) });

    public void AddIntroducedMemberAttribute (IAttributableEmitter memberEmitter, MemberDefinitionBase implementingMember, MemberInfo interfaceMember)
    {
      var constructorArgs = new object[] { 
          implementingMember.DeclaringClass.Type, 
          implementingMember.Name, 
          interfaceMember.DeclaringType, 
          interfaceMember.Name };

      var builder = new CustomAttributeBuilder (s_introducedMemberAttributeCtor, constructorArgs);
      memberEmitter.AddCustomAttribute (builder);
    }
  }
}