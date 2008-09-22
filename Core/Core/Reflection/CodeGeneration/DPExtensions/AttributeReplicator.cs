/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public static class AttributeReplicator
  {
    public static void ReplicateAttribute (IAttributableEmitter target, CustomAttributeData attributeData)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("attributeData", attributeData);

      CustomAttributeBuilder builder = CreateAttributeBuilderFromData (attributeData);
      target.AddCustomAttribute (builder);
    }

    private static CustomAttributeBuilder CreateAttributeBuilderFromData (CustomAttributeData attributeData)
    {
      CustomAttributeArguments arguments = CustomAttributeDataUtility.ParseCustomAttributeArguments (attributeData);
      return new CustomAttributeBuilder (attributeData.Constructor, arguments.ConstructorArgs, arguments.NamedProperties,
          arguments.PropertyValues, arguments.NamedFields, arguments.FieldValues);
    }
  }
}
