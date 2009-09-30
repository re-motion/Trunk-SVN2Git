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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using System.Reflection.Emit;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  public class MethodBuilderEmitter : IMemberEmitter
  {
    private readonly MethodBuilder _methodBuilder;
    private readonly MethodCodeBuilder _codeBuilder;

    public MethodBuilderEmitter (MethodBuilder methodBuilder)
    {
      ArgumentUtility.CheckNotNull ("methodBuilder", methodBuilder);
      _methodBuilder = methodBuilder;
      _codeBuilder = new MethodCodeBuilder (methodBuilder.GetILGenerator());
    }

    public MethodBuilder MethodBuilder
    {
      get { return _methodBuilder; }
    }

    public MethodCodeBuilder CodeBuilder
    {
      get { return _codeBuilder; }
    }

    public void Generate ()
    {
      PrivateInvoke.InvokeNonPublicMethod (_codeBuilder, "Generate", this, _codeBuilder.Generator);
    }

    public void EnsureValidCodeBlock ()
    {
      throw new NotImplementedException();
    }

    public MemberInfo Member
    {
      get { return _methodBuilder; }
    }

    public Type ReturnType
    {
      get { return _methodBuilder.ReturnType; }
    }
  }
}
