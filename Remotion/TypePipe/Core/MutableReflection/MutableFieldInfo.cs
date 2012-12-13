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
using System.Reflection;
using Remotion.TypePipe.MutableReflection.Descriptors;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection
{
  /// <summary>
  /// Represents a <see cref="FieldInfo"/> that can be modified.
  /// </summary>
  [DebuggerDisplay ("{ToDebugString(),nq}")]
  public class MutableFieldInfo : FieldInfo, IMutableInfo
  {
    private readonly MutableType _declaringType;
    private readonly FieldDescriptor _descriptor;
    private readonly MutableInfoCustomAttributeHelper _customAttributeHelper;

    private FieldAttributes _attributes;

    public MutableFieldInfo (MutableType declaringType, FieldDescriptor descriptor)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("descriptor", descriptor);

      _declaringType = declaringType;
      _descriptor = descriptor;
      _customAttributeHelper = new MutableInfoCustomAttributeHelper (this, descriptor.CustomAttributeDataProvider, () => CanAddCustomAttributes);

      _attributes = descriptor.Attributes;
    }

    public override Type DeclaringType
    {
      get { return _declaringType; }
    }

    public FieldInfo UnderlyingSystemFieldInfo
    {
      get { return _descriptor.UnderlyingSystemInfo ?? this; }
    }

    public bool IsNew
    {
      get { return _descriptor.UnderlyingSystemInfo == null; }
    }

    public bool IsModified
    {
      get { return _customAttributeHelper.AddedCustomAttributeDeclarations.Count != 0; }
    }

    public override Type FieldType
    {
      get { return _descriptor.Type; }
    }

    public override string Name
    {
      get { return _descriptor.Name; }
    }

    public override FieldAttributes Attributes
    {
      get { return _attributes; }
    }

    public bool CanAddCustomAttributes
    {
      // TODO 4695
      get { return IsNew; }
    }

    public ReadOnlyCollection<CustomAttributeDeclaration> AddedCustomAttributeDeclarations
    {
      get { return _customAttributeHelper.AddedCustomAttributeDeclarations; }
    }

    public void AddCustomAttribute (CustomAttributeDeclaration customAttributeDeclaration)
    {
      ArgumentUtility.CheckNotNull ("customAttributeDeclaration", customAttributeDeclaration);

      _customAttributeHelper.AddCustomAttribute (customAttributeDeclaration);

      if (customAttributeDeclaration.Type == typeof (NonSerializedAttribute))
        _attributes |= FieldAttributes.NotSerialized;
    }

    public IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return _customAttributeHelper.GetCustomAttributeData();
    }

    public override object[] GetCustomAttributes (bool inherit)
    {
      return _customAttributeHelper.GetCustomAttributes (inherit);
    }

    public override object[] GetCustomAttributes (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return _customAttributeHelper.GetCustomAttributes (attributeType, inherit);
    }

    public override bool IsDefined (Type attributeType, bool inherit)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return _customAttributeHelper.IsDefined (attributeType, inherit);
    }

    public override string ToString ()
    {
      return SignatureDebugStringGenerator.GetFieldSignature (this);
    }

    public string ToDebugString ()
    {
      return string.Format ("MutableField = \"{0}\", DeclaringType = \"{1}\"", ToString(), DeclaringType);
    }

    #region Not Implemented from FieldInfo interface

    public override object GetValue (object obj)
    {
      throw new NotImplementedException();
    }

    public override void SetValue (object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    public override Type ReflectedType
    {
      get { throw new NotImplementedException(); }
    }

    public override RuntimeFieldHandle FieldHandle
    {
      get { throw new NotImplementedException(); }
    }

    #endregion
  }
}