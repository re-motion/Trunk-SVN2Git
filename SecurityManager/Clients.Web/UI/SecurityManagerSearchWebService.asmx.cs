// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;
using System.Linq;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.Web;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Clients.Web.UI
{
  [WebService (Namespace = "http://www.re-motion.org/SecurityManager/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class SecurityManagerSearchWebService : WebService
  {
    [WebMethod (EnableSession = true)]
    public BusinessObjectWithIdentityProxy[] GetBusinessObjects (
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

      Type type = TypeUtility.GetType (businessObjectClass);
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (type);
      
      IBusinessObjectProperty propertyDefinition = bindableObjectClass.GetPropertyDefinition (businessObjectProperty);
      Assertion.IsNotNull (propertyDefinition);
      Assertion.IsTrue (propertyDefinition is IBusinessObjectReferenceProperty);
      
      IBusinessObjectReferenceProperty referenceProperty = (IBusinessObjectReferenceProperty) propertyDefinition;

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var result = referenceProperty.SearchAvailableObjects ((IBusinessObject)RepositoryAccessor.NewObject (type).With(), new DefaultSearchArguments (args));
        if (completionSetCount.HasValue)
          result.Take (completionSetCount.Value);
        return result.Cast<IBusinessObjectWithIdentity>().Select (o => new BusinessObjectWithIdentityProxy (o)).ToArray();
      }
    }
  }
}
