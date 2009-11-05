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
using System.Linq;
using Remotion.ExtensibleEnums;
using Remotion.ExtensibleEnums.Infrastructure;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject.Properties
{
  public class ExtensibleEnumerationProperty : PropertyBase, IBusinessObjectEnumerationProperty
  {
    private readonly IExtensibleEnumDefinition _definition;

    public ExtensibleEnumerationProperty (Parameters parameters)
        : base(parameters)
    {
      _definition = ExtensibleEnumDefinitionCache.Instance.GetDefinition (PropertyType);
    }

    public IEnumerationValueInfo[] GetAllValues (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);
      return _definition.GetValueInfos ()
          .Select (info => CreateEnumerationValueInfo(info))
          .ToArray();
    }

    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      throw new NotImplementedException();
    }

    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      throw new NotImplementedException();
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      throw new NotImplementedException();
    }

    public EnumerationValueInfo CreateEnumerationValueInfo (IExtensibleEnumInfo extensibleEnumInfo)
    {
      ArgumentUtility.CheckNotNull ("extensibleEnumInfo", extensibleEnumInfo);

      return new EnumerationValueInfo (extensibleEnumInfo.Value, extensibleEnumInfo.Value.ID, GetDisplayName(extensibleEnumInfo), false);
    }

    private string GetDisplayName (IExtensibleEnumInfo extensibleEnumInfo)
    {
      var globalizationService = BusinessObjectProvider.GetService<IBindableObjectGlobalizationService> ();
      if (globalizationService == null)
        return extensibleEnumInfo.Value.ToString ();
      return globalizationService.GetExtensibleEnumerationValueDisplayName (extensibleEnumInfo.Value);
    }
  }
}