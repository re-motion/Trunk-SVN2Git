// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.DynamicProxy
{
  public static class ConcreteMixinTypeAttributeUtility
  {
    private static readonly ConstructorInfo s_attributeCtor = typeof (ConcreteMixinTypeAttribute).GetConstructor (new[] { typeof (object[]) });

    public static CustomAttributeBuilder CreateAttributeBuilder (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      ArgumentUtility.CheckNotNull ("concreteMixinTypeIdentifier", concreteMixinTypeIdentifier);
      Assertion.IsNotNull (s_attributeCtor);

      ConcreteMixinTypeAttribute attribute = ConcreteMixinTypeAttribute.Create (concreteMixinTypeIdentifier);
      var builder = new CustomAttributeBuilder (s_attributeCtor, new object[] { attribute.ConcreteMixinTypeIdentifierData });

      return builder;
    }
  }
}
