// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Text;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Represents a constructor that does not exist yet. This is used to represent constructors yet to be generated within an expression tree.
  /// </summary>
  [DebuggerDisplay ("{ToDebugString(),nq}")]
  public class MutableConstructorInfo : ConstructorInfo, IMutableMethodBase
  {
    private readonly MutableType _declaringType;
    private readonly UnderlyingConstructorInfoDescriptor _underlyingConstructorInfoDescriptor;
    private readonly ReadOnlyCollection<MutableParameterInfo> _parameters;

    private Expression _body;

    public MutableConstructorInfo (MutableType declaringType, UnderlyingConstructorInfoDescriptor underlyingConstructorInfoDescriptor)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("underlyingConstructorInfoDescriptor", underlyingConstructorInfoDescriptor);

      _declaringType = declaringType;
      _underlyingConstructorInfoDescriptor = underlyingConstructorInfoDescriptor;

      Assertion.IsFalse (IsStatic, "Static constructors are not (yet) supported.");

      _parameters = _underlyingConstructorInfoDescriptor.ParameterDeclarations
          .Select ((pd, i) => MutableParameterInfo.CreateFromDeclaration (this, i, pd))
          .ToList().AsReadOnly();

      _body = _underlyingConstructorInfoDescriptor.Body;
    }

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    MutableType IMutableMethodBase.DeclaringType
    {
      get { return _declaringType; }
    }

    public ConstructorInfo UnderlyingSystemConstructorInfo
    {
      get { return _underlyingConstructorInfoDescriptor.UnderlyingSystemMethodBase ?? this; }
    }

    public bool IsNew
    {
      get { return _underlyingConstructorInfoDescriptor.UnderlyingSystemMethodBase == null; }
    }

    public bool IsModified
    {
      get { return _body != _underlyingConstructorInfoDescriptor.Body; }
    }

    public override MethodAttributes Attributes
    {
      get { return _underlyingConstructorInfoDescriptor.Attributes; }
    }

    public override CallingConventions CallingConvention
    {
      get
      {
        Assertion.IsFalse (IsStatic, "Static constructors are not (yet) supported.");
        return CallingConventions.HasThis;
      }
    }

    public override string Name
    {
      get { return _underlyingConstructorInfoDescriptor.Name; }
    }

    public IEnumerable<ParameterExpression> ParameterExpressions
    {
      get { return _underlyingConstructorInfoDescriptor.ParameterDeclarations.Select (pd => pd.Expression); }
    }

    public Expression Body
    {
      get { return _body; }
    }

    public void SetBody (Func<ConstructorBodyModificationContext, Expression> bodyProvider)
    {
      ArgumentUtility.CheckNotNull ("bodyProvider", bodyProvider);

      var context = new ConstructorBodyModificationContext (_declaringType, ParameterExpressions, _body);
      _body = BodyProviderUtility.GetTypedBody (typeof (void), bodyProvider, context);
    }

    public override string ToString ()
    {
      return SignatureDebugStringGenerator.GetConstructorSignatureString (this);
    }

    public string ToDebugString()
    {
      return string.Format ("MutableConstructor = \"{0}\", DeclaringType = \"{1}\"", ToString(), DeclaringType.Name);
    }

    public override ParameterInfo[] GetParameters ()
    {
      return _parameters.ToArray();
    }

    #region Not Implemented from ConstructorInfo interface

    public override object[] GetCustomAttributes (bool inherit)
    {
      throw new NotImplementedException();
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override MethodImplAttributes GetMethodImplementationFlags ()
    {
      throw new NotImplementedException();
    }

    public override object Invoke (object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override RuntimeMethodHandle MethodHandle
    {
      get { throw new NotImplementedException(); }
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      throw new NotImplementedException();
    }

    public override object Invoke (BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    #endregion
  }
}