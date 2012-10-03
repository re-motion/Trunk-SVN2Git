// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Reflection.CodeGeneration;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  /// <summary>
  /// Generates attribute data given as <see cref="CustomAttributeData"/> to an <see cref="IAttributableEmitter"/>.
  /// </summary>
  public class AttributeGenerator
  {
    public void GenerateAttribute (IAttributableEmitter target, ICustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilder builder = CreateAttributeBuilderFromData (attributeData);
      target.AddCustomAttribute (builder);
    }

    private CustomAttributeBuilder CreateAttributeBuilderFromData (ICustomAttributeData attributeData)
    {
      var namedArgumentsByMemberType = attributeData.NamedArguments.ToLookup (na => na.MemberInfo.MemberType);
      
      return new CustomAttributeBuilder (
          attributeData.Constructor, 
          attributeData.ConstructorArguments.ToArray(),
          namedArgumentsByMemberType[MemberTypes.Property].Select (na => (PropertyInfo) na.MemberInfo).ToArray(),
          namedArgumentsByMemberType[MemberTypes.Property].Select (na => na.Value).ToArray(),
          namedArgumentsByMemberType[MemberTypes.Field].Select (na => (FieldInfo) na.MemberInfo).ToArray(),
          namedArgumentsByMemberType[MemberTypes.Field].Select (na => na.Value).ToArray());
    }
  }
}
