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
using System.Linq;
using System.Reflection;
using Remotion.Collections;

namespace Remotion.Scripting
{
  public class MethodInfoFromRelatedTypesEqualityComparer : CompoundValueEqualityComparer<MethodInfo> {
    private static readonly MethodInfoFromRelatedTypesEqualityComparer s_equalityComparer = new MethodInfoFromRelatedTypesEqualityComparer();

    public MethodInfoFromRelatedTypesEqualityComparer (MethodAttributes methodAttributeMask)
        : base (
            x => new object[] {
                //x.Name, x.ReturnType, x.Attributes & methodAttributeMask, x.IsGenericMethod ? x.GetGenericArguments().Length : 0,
                //ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? new Type[0] : x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? typeof(Object) : pi.ParameterType)),
                //ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? pi.ParameterType.GenericParameterPosition : -1) : new int[0]),
                //ComponentwiseEqualsAndHashcodeWrapper.New (x.GetParameters ().Select (pi => pi.Attributes)) 

                //x.Name, x.ReturnType, x.Attributes & methodAttributeMask, x.IsGenericMethod ? x.GetGenericArguments().Length : 0,
                //ComponentwiseEqualsAndHashcodeWrapper.New (x.GetParameters ().Select (pi => pi.MetadataToken)) 

                x.Name, x.ReturnType, x.Attributes & methodAttributeMask, x.IsGenericMethod ? x.GetGenericArguments().Length : 0,
                ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? new Type[0] : x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? typeof(Object) : pi.ParameterType)),
                ComponentwiseEqualsAndHashcodeWrapper.New (x.IsGenericMethod ? x.GetParameters ().Select (pi => pi.ParameterType.IsGenericParameter ? pi.ParameterType.GenericParameterPosition : -1) : new int[0]),
                ComponentwiseEqualsAndHashcodeWrapper.New (x.GetParameters ().Select (pi => pi.Attributes)),
                ComponentwiseEqualsAndHashcodeWrapper.New (x.GetParameters ().Select (pi => pi.MetadataToken)) 
            })
    {
    }

    // Masking out VtableLayoutMask is necessary, otherwise a virtual method and its override will not compare equal
    public MethodInfoFromRelatedTypesEqualityComparer ()
      : this (~(MethodAttributes.ReservedMask | MethodAttributes.VtableLayoutMask))
    {
    }


    public static MethodInfoFromRelatedTypesEqualityComparer Get
    {
      get { return s_equalityComparer; }
    }

 
  }
}