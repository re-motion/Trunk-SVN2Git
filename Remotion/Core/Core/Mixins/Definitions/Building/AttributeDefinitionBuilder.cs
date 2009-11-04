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
using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions.Building
{
  public class AttributeDefinitionBuilder
  {
    private readonly IAttributableDefinition _attributableDefinition;

    public AttributeDefinitionBuilder (IAttributableDefinition attributableDefinition)
    {
      ArgumentUtility.CheckNotNull ("attributableDefinition", attributableDefinition);
      _attributableDefinition = attributableDefinition;
    }

    public void Apply (MemberInfo attributeSource)
    {
      ArgumentUtility.CheckNotNull ("attributeSource", attributeSource);

      Apply (attributeSource, CustomAttributeData.GetCustomAttributes (attributeSource), false);
    }

    public void Apply (MemberInfo attributeSource, IEnumerable<CustomAttributeData> attributes, bool isCopyTemplate)
    {
      ArgumentUtility.CheckNotNull ("attributeSource", attributeSource);
      ArgumentUtility.CheckNotNull ("attributes", attributes);

      foreach (CustomAttributeData attributeData in attributes)
      {
        Type attributeType = attributeData.Constructor.DeclaringType;
        if (attributeType == typeof (CopyCustomAttributesAttribute))
          Copy (attributeSource, attributeData);
        else if (attributeType.IsVisible && !IsIgnoredAttributeType (attributeType))
          _attributableDefinition.CustomAttributes.Add (new AttributeDefinition (_attributableDefinition, attributeData, isCopyTemplate));
      }
    }

    private bool IsIgnoredAttributeType (Type type)
    {
      return type == typeof (SerializableAttribute) 
          || (type.Assembly == typeof (ExtendsAttribute).Assembly && type.Namespace.StartsWith ("Remotion.Mixins"));
    }

    private void Copy (MemberInfo attributeSource, CustomAttributeData copyAttributeData)
    {
      Assertion.IsTrue (copyAttributeData.Constructor.DeclaringType == typeof (CopyCustomAttributesAttribute));
      string sourceName = GetFullMemberName (attributeSource);

      var copyAttribute = (CopyCustomAttributesAttribute) CustomAttributeDataUtility.InstantiateCustomAttributeData (copyAttributeData);

      MemberInfo copySource;
      try
      {
        copySource = copyAttribute.GetAttributeSource (UnifyTypeMemberTypes (attributeSource.MemberType));
      }
      catch (AmbiguousMatchException ex)
      {
        string message = string.Format ("The CopyCustomAttributes attribute on {0} specifies an ambiguous attribute source: {1}",
            sourceName, ex.Message);
        throw new ConfigurationException (message, ex);
      }
      
      if (copySource == null)
      {
        string message = string.Format ("The CopyCustomAttributes attribute on {0} specifies an unknown attribute source {1}.",
            sourceName, copyAttribute.AttributeSourceName);
        throw new ConfigurationException (message);
      }

      if (!AreCompatibleMemberTypes (copySource.MemberType, attributeSource.MemberType))
      {
        string message = string.Format ("The CopyCustomAttributes attribute on {0} specifies an attribute source {1} of a different member kind.",
            sourceName, copyAttribute.AttributeSourceName);
        throw new ConfigurationException (message);
      }

      bool parseCopyAttributes = !copySource.Equals (attributeSource);
      bool parseInheritedAttributes = !copySource.Equals (attributeSource);
      Apply (copySource, GetFilteredAttributeData(copySource, copyAttribute, parseCopyAttributes, parseInheritedAttributes), true);
    }

    private IEnumerable<CustomAttributeData> GetFilteredAttributeData (MemberInfo copySource, CopyCustomAttributesAttribute filterAttribute,
        bool includeCopyAttributes, bool includeInheritedAttributes)
    {
      foreach (CustomAttributeData attributeData in CustomAttributeData.GetCustomAttributes (copySource))
      {
        Type attributeType = attributeData.Constructor.DeclaringType;
        if (typeof (CopyCustomAttributesAttribute).IsAssignableFrom (attributeType))
        {
          if (includeCopyAttributes)
            yield return attributeData;
        }
        else if (filterAttribute.IsCopiedAttributeType (attributeType) && (includeInheritedAttributes || !AttributeUtility.IsAttributeInherited (attributeType)))
          yield return attributeData;
      }
    }

    private bool AreCompatibleMemberTypes (MemberTypes one, MemberTypes two)
    {
      return UnifyTypeMemberTypes (one) == UnifyTypeMemberTypes (two);
    }

    private MemberTypes UnifyTypeMemberTypes (MemberTypes memberType)
    {
      if (memberType == MemberTypes.NestedType || memberType == MemberTypes.TypeInfo)
        return MemberTypes.NestedType | MemberTypes.TypeInfo;
      else
        return memberType;
    }

    private string GetFullMemberName (MemberInfo attributeSource)
    {
      return attributeSource.DeclaringType != null ? attributeSource.DeclaringType.FullName + "." + attributeSource.Name : attributeSource.Name;
    }
  }
}
