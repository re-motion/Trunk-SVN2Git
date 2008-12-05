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
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class CodeGenerationSerializerUtility
  {
    public static LocalReference DeclareAndFillArrayLocal<T> (T[] array, AbstractCodeBuilder codeBuilder, Func<T, Expression> expressionGenerator)
    {
      ArgumentUtility.CheckNotNull ("array", array);
      ArgumentUtility.CheckNotNull ("codeBuilder", codeBuilder);
      ArgumentUtility.CheckNotNull ("expressionGenerator", expressionGenerator);

      var newArrayExpression = new NewArrayExpression (array.Length, typeof (T));
      var arrayLocal = codeBuilder.DeclareLocal (typeof (T[]));
      codeBuilder.AddStatement (new AssignStatement (arrayLocal, newArrayExpression));

      for (int i = 0; i < array.Length; ++i)
        codeBuilder.AddStatement (new AssignArrayStatement (arrayLocal, i, expressionGenerator (array[i])));
      return arrayLocal;
    }

  }
}