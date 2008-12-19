// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class FieldInfoReference : TypeReference
  {
    private readonly FieldInfo _field;

    public FieldInfoReference (Reference owner, FieldInfo field)
        : base (owner, field.FieldType)
    {
      _field = field;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsflda, _field);
      else
        gen.Emit (OpCodes.Ldflda, _field);
    }

    public override void LoadReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Ldsfld, _field);
      else
        gen.Emit (OpCodes.Ldfld, _field);
    }

    public override void StoreReference (ILGenerator gen)
    {
      if (IsStaticField)
        gen.Emit (OpCodes.Stsfld, _field);
      else
        gen.Emit (OpCodes.Stfld, _field);
    }

    private bool IsStaticField
    {
      get { return _field.IsStatic; }
    }
  }
}
