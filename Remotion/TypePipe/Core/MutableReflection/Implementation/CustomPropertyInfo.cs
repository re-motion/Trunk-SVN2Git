﻿// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Implementation
{
  /// <summary>
  /// A custom <see cref="PropertyInfo"/> that re-implements parts of the reflection API. Other classes may derive from this class to inherit the 
  /// implementation. Note that the equality members <see cref="object.Equals(object)"/> and <see cref="object.GetHashCode"/> are implemented for
  /// reference equality.
  /// </summary>
  [DebuggerDisplay ("{ToDebugString(),nq}")]
  public abstract class CustomPropertyInfo : PropertyInfo, ICustomAttributeDataProvider
  {
    private readonly CustomType _declaringType;
    private readonly string _name;
    private readonly Type _type;
    private readonly PropertyAttributes _attributes;
    private readonly CustomMethodInfo _getMethod;
    private readonly CustomMethodInfo _setMethod;

    protected readonly CustomParameterInfo[] IndexParameters;

    protected CustomPropertyInfo (
        CustomType declaringType,
        string name,
        Type type,
        PropertyAttributes attributes,
        CustomMethodInfo getMethod,
        CustomMethodInfo setMethod,
        params CustomParameterInfo[] indexParameters)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("type", type);
      // Getter may be null.
      // Setter may be null.
      ArgumentUtility.CheckNotNull ("indexParameters", indexParameters);
      Assertion.IsTrue (type != typeof (void));
      Assertion.IsTrue (getMethod != null || setMethod != null);
      // TODO xxx: Maybe (if it is easy) add assertions that getMethod and setMethod have the right parameters/return type.
      //var parametersBase = indexParameters.Select (p => p.ParameterType).ToArray();
      //Assertion.IsTrue (
      //    getMethod == null
      //    || (getMethod.GetParameters().Select (p => p.ParameterType).SequenceEqual (parametersBase) && getMethod.ReturnType == type));
      //Assertion.IsTrue (
      //    setMethod == null || setMethod.GetParameters().Select (p => p.ParameterType).SequenceEqual (parametersBase.Concat (new[] { _type })));

      _declaringType = declaringType;
      _name = name;
      _type = type;
      _attributes = attributes;
      _getMethod = getMethod;
      _setMethod = setMethod;
      IndexParameters = indexParameters;
    }

    public abstract IEnumerable<ICustomAttributeData> GetCustomAttributeData ();

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    public override string Name
    {
      get { return _name; }
    }

    public override Type PropertyType
    {
      get { return _type; }
    }

    public override PropertyAttributes Attributes
    {
      get { return _attributes; }
    }

    public override ParameterInfo[] GetIndexParameters ()
    {
      return IndexParameters;
    }

    public override MethodInfo GetGetMethod (bool nonPublic)
    {
      return GetAccessorOrNull (_getMethod, nonPublic);
    }

    public override MethodInfo GetSetMethod (bool nonPublic)
    {
      return GetAccessorOrNull (_setMethod, nonPublic);
    }

    public override MethodInfo[] GetAccessors (bool nonPublic)
    {
      return new[] { GetGetMethod (nonPublic), GetSetMethod (nonPublic) }.Where (a => a != null).ToArray();
    }

    public IEnumerable<ICustomAttributeData> GetCustomAttributeData (bool inherit)
    {
      return TypePipeCustomAttributeData.GetCustomAttributes (this, inherit);
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      return CustomAttributeFinder.GetCustomAttributes (this, inherit);
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return CustomAttributeFinder.GetCustomAttributes (this, attributeType, inherit);
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return CustomAttributeFinder.IsDefined (this, attributeType, inherit);
    }

    public override string ToString ()
    {
      return SignatureDebugStringGenerator.GetPropertySignature (this);
    }

    public string ToDebugString ()
    {
      return string.Format ("{0} = \"{1}\", DeclaringType = \"{2}\"", GetType ().Name.Replace ("Info", ""), ToString (), DeclaringType);
    }

    private MethodInfo GetAccessorOrNull (MethodInfo accessor, bool nonPublic)
    {
      return accessor != null && (accessor.IsPublic || nonPublic) ? accessor : null;
    }

    #region Unsupported Members

    public override Type ReflectedType
    {
      get { throw new NotSupportedException ("Property ReflectedType is not supported."); }
    }

    public override bool CanRead
    {
      get { throw new NotSupportedException ("Property CanRead is not supported."); }
    }

    public override bool CanWrite
    {
      get { throw new NotSupportedException ("Property CanWrite is not supported."); }
    }

    public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotSupportedException ("Method SetValue is not supported.");
    }

    public override object GetValue (object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
    {
      throw new NotSupportedException ("Method GetValue is not supported.");
    }

    #endregion
  }
}