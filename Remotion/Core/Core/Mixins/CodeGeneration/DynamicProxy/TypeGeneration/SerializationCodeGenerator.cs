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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Mixins.Definitions;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy.TypeGeneration
{
  public class SerializationCodeGenerator
  {
    private static readonly MethodInfo s_getObjectDataMethod = typeof (SerializationHelper).GetMethod ("GetObjectDataForGeneratedTypes");

    private readonly FieldReference _configurationField;
    private readonly FieldReference _extensionsField;

    public SerializationCodeGenerator (FieldReference configurationField, FieldReference extensionsField)
    {
      ArgumentUtility.CheckNotNull ("configurationField", configurationField);
      ArgumentUtility.CheckNotNull ("extensionsField", extensionsField);

      _configurationField = configurationField;
      _extensionsField = extensionsField;
    }

    public void ImplementISerializable (IClassEmitter emitter)
    {
      var contextReference = new PropertyReference (_configurationField, typeof (TargetClassDefinition).GetProperty ("ConfigurationContext"));
      SerializationImplementer.ImplementGetObjectDataByDelegation (
          emitter,
          (newMethod, baseIsISerializable) => new MethodInvocationExpression (
                                                  null,
                                                  s_getObjectDataMethod,
                                                  newMethod.ArgumentReferences[0].ToExpression (),
                                                  newMethod.ArgumentReferences[1].ToExpression (),
                                                  SelfReference.Self.ToExpression (),
                                                  contextReference.ToExpression(),
                                                  _extensionsField.ToExpression (),
                                                  new ConstReference (!baseIsISerializable).ToExpression()));

      // Implement dummy ISerializable constructor if we haven't already replicated it
      SerializationImplementer.ImplementDeserializationConstructorByThrowingIfNotExistsOnBase (emitter);
    }
  }
}