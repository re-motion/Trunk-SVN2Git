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
