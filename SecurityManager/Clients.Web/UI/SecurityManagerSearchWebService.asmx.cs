// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI.Controls;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  /// <summary>
  /// The <see cref="SecurityManagerSearchWebService"/> is used as an interface between <see cref="BocAutoCompleteReferenceValue"/> controls and the 
  /// <see cref="ISearchAvailableObjectsService"/> implementation.
  /// </summary>
  [WebService (Namespace = "http://www.re-motion.org/SecurityManager/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class SecurityManagerSearchWebService : WebService, ISearchAvailableObjectWebService
  {
    public static void BindServiceToControl (BocAutoCompleteReferenceValue control)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      control.ServicePath = ResourceUrlResolver.GetResourceUrl (
          (IControl) control, typeof (SecurityManagerSearchWebService), ResourceType.UI, "SecurityManagerSearchWebService.asmx");
      control.ServiceMethod = "Search";
    }

    [WebMethod (EnableSession = true)]
    public BusinessObjectWithIdentityProxy[] Search (
        string prefixText,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObjectID,
        string args)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("businessObjectClass", businessObjectClass);
      ArgumentUtility.CheckNotNullOrEmpty ("businessObjectProperty", businessObjectProperty);
      ArgumentUtility.CheckNotNullOrEmpty ("args", args);

      Type type = TypeUtility.GetType (businessObjectClass, true);
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (type);

      IBusinessObjectProperty propertyDefinition = bindableObjectClass.GetPropertyDefinition (businessObjectProperty);
      Assertion.IsNotNull (propertyDefinition);
      Assertion.IsTrue (propertyDefinition is IBusinessObjectReferenceProperty);

      IBusinessObjectReferenceProperty referenceProperty = (IBusinessObjectReferenceProperty) propertyDefinition;

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var result = referenceProperty.SearchAvailableObjects (
            (IBusinessObject) LifetimeService.NewObject (ClientTransaction.Current, type, ParamList.Empty), new DefaultSearchArguments (args));
        if (completionSetCount.HasValue)
          result.Take (completionSetCount.Value);
        return result.Cast<IBusinessObjectWithIdentity>().Select (o => new BusinessObjectWithIdentityProxy (o)).ToArray();
      }
    }
  }
}
