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
using System.Diagnostics;
using Remotion.Mixins.Context;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// The <see cref="ITypeIdentifierProvider"/> returned by the <see cref="MixinParticipant"/>.
  /// </summary>
  public class MixinParticipantTypeIdentifierProvider : ITypeIdentifierProvider
  {
    // TODO 5370: Remove.
    private readonly IConcreteTypeMetadataImporter _concreteTypeMetadataImporter;

    public MixinParticipantTypeIdentifierProvider (IConcreteTypeMetadataImporter concreteTypeMetadataImporter)
    {
      ArgumentUtility.CheckNotNull ("concreteTypeMetadataImporter", concreteTypeMetadataImporter);

      _concreteTypeMetadataImporter = concreteTypeMetadataImporter;
    }

    public object GetID (Type requestedType)
    {
      // Using Debug.Assert because it will be compiled away.
      Debug.Assert (requestedType != null);

      return MixinConfiguration.ActiveConfiguration.GetContext (requestedType);
    }

    public Expression GetExpression (object id)
    {
      var classContext = ArgumentUtility.CheckNotNullAndType<ClassContext> ("id", id);

      var classContextExpression = GetClassContextExpression (classContext);
      return Expression.Convert (classContextExpression, typeof (object));
    }

    public Expression GetFlatValueExpressionForSerialization (object id)
    {
      var classContext = ArgumentUtility.CheckNotNullAndType<ClassContext> ("id", id);

      var classContextExpression = GetClassContextExpression (classContext);
      return Expression.Call (typeof (FlatClassContext), "Create", Type.EmptyTypes, classContextExpression);
    }

    private Expression GetClassContextExpression (ClassContext classContext)
    {
      var classContextCodeGenerator = new CodeGenerationClassContextSerializer ();
      classContext.Serialize (classContextCodeGenerator);
      var classContextExpression = classContextCodeGenerator.GetConstructorInvocationExpression ();
      return classContextExpression;
    }
  }
}