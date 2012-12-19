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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Utilities;

namespace Remotion.TypePipe.MutableReflection.Implementation
{
  /// <summary>
  /// A helper class that is used to implement custom <see cref="Attribute"/>-related members of <see cref="IMutableInfo"/> on mutable reflection
  /// objects.
  /// </summary>
  public class MutableInfoCustomAttributeContainer
  {
    // TODO 5057: Use Lazy<T>
    private readonly DoubleCheckedLockingContainer<ReadOnlyCollection<ICustomAttributeData>> _existingCustomAttributeDatas;
    private readonly Func<bool> _canAddCustomAttributesDecider;

    private readonly List<CustomAttributeDeclaration> _addedCustomAttributeDeclarations = new List<CustomAttributeDeclaration>();

    public MutableInfoCustomAttributeContainer (
        Func<ReadOnlyCollection<ICustomAttributeData>> customAttributeDataProvider, Func<bool> canAddCustomAttributesDecider)
    {
      ArgumentUtility.CheckNotNull ("customAttributeDataProvider", customAttributeDataProvider);
      ArgumentUtility.CheckNotNull ("canAddCustomAttributesDecider", canAddCustomAttributesDecider);

      _existingCustomAttributeDatas = new DoubleCheckedLockingContainer<ReadOnlyCollection<ICustomAttributeData>> (customAttributeDataProvider);
      _canAddCustomAttributesDecider = canAddCustomAttributesDecider;
    }

    public ReadOnlyCollection<CustomAttributeDeclaration> AddedCustomAttributes
    {
      get { return _addedCustomAttributeDeclarations.AsReadOnly(); }
    }

    public void AddCustomAttribute (CustomAttributeDeclaration customAttributeDeclaration)
    {
      ArgumentUtility.CheckNotNull ("customAttributeDeclaration", customAttributeDeclaration);

      if (!_canAddCustomAttributesDecider())
        throw new NotSupportedException ("Adding custom attributes to this element is not supported.");

      if (GetCustomAttributeData().Any (a => a.Type == customAttributeDeclaration.Type && !AttributeUtility.IsAttributeAllowMultiple (a.Type)))
      {
        var message = string.Format ("Attribute of type '{0}' (with AllowMultiple = false) is already present.", customAttributeDeclaration.Type.Name);
        throw new InvalidOperationException (message);
      }

      _addedCustomAttributeDeclarations.Add (customAttributeDeclaration);
    }

    public IEnumerable<ICustomAttributeData> GetCustomAttributeData ()
    {
      return _addedCustomAttributeDeclarations.Cast<ICustomAttributeData>().Concat (_existingCustomAttributeDatas.Value);
    }
  }
}