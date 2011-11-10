// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Castle.DynamicProxy.Generators.Emitters.CodeBuilders;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class CodeGenerationSerializerUtility
  {
    public static LocalReference DeclareAndFillArrayLocal<T> (ICollection<T> sourceCollection, AbstractCodeBuilder codeBuilder, Func<T, Expression> expressionGenerator)
    {
      ArgumentUtility.CheckNotNull ("sourceCollection", sourceCollection);
      ArgumentUtility.CheckNotNull ("codeBuilder", codeBuilder);
      ArgumentUtility.CheckNotNull ("expressionGenerator", expressionGenerator);

      var newArrayExpression = new NewArrayExpression (sourceCollection.Count, typeof (T));
      var arrayLocal = codeBuilder.DeclareLocal (typeof (T[]));
      codeBuilder.AddStatement (new AssignStatement (arrayLocal, newArrayExpression));

      int index = 0;
      foreach (var element in sourceCollection)
      {
        codeBuilder.AddStatement (new AssignArrayStatement (arrayLocal, index, expressionGenerator (element)));
        ++index;
      }
      return arrayLocal;
    }

  }
}
