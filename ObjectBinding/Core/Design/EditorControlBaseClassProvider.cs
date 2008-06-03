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
using System.ComponentModel;

namespace Remotion.ObjectBinding.Design
{
  internal class EditorControlBaseClassProvider : TypeDescriptionProvider
  {
    public EditorControlBaseClassProvider ()
        : base (TypeDescriptor.GetProvider (typeof (EditorControlBase)))
    {
    }

    public override Type GetReflectionType (Type objectType, object instance)
    {
      if (objectType == typeof (EditorControlBase))
        return typeof (ConcreteEditorControlBase);

      return base.GetReflectionType (objectType, instance);
    }

    public override object CreateInstance (IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
    {
      if (objectType == typeof (EditorControlBase))
        objectType = typeof (ConcreteEditorControlBase);

      return base.CreateInstance (provider, objectType, argTypes, args);
    }
  }
}
