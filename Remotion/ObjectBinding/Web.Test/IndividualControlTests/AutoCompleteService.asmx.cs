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
using System.ComponentModel;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace OBWTest.IndividualControlTests
{
  /// <summary>
  /// Summary description for AutoCompleteService
  /// </summary>
  [WebService (Namespace = "http://tempuri.org/")]
  [WebServiceBinding (ConformsTo = WsiProfiles.BasicProfile1_1)]
  [ToolboxItem (false)]
  [ScriptService]
  public class AutoCompleteService : WebService, ISearchAvailableObjectWebService
  {
    #region Values

    private static string[] s_values = new string[]
                                       {
                                           "sdfg",
                                           "sdfgh",
                                           "sdfghj",
                                           "sdfghjk",
                                           "sdfghjkl",
                                           "sdfg 0qqqqwwww",
                                           "sdfg 1qqqqwwww",
                                           "sdfg 2qqqqwwww",
                                           "sdfg 3qqqqwwww",
                                           "sdfg 4qqqqwwww",
                                           "sdfg 5qqqqwwww",
                                           "sdfg 7qqqqwwww",
                                           "sdfg 8qqqqwwww",
                                           "sdfg 9qqqqwwww",
                                           "sdfg q",
                                           "sdfg qq",
                                           "sdfg qqq",
                                           "sdfg qqqq",
                                           "sdfg qqqqq",
                                           "sdfg qqqqqq",
                                           "sdfg qqqqqqq",
                                           "sdfg qqqqqqqq",
                                           "sdfg qqqqqqqqq",
                                           "sdfg qqqqqqqqqq",
                                           "sdfg qqqqqqqqqqq",
                                           "access control list (ACL)",
                                           "ADO.NET",
                                           "aggregate event",
                                           "alpha channel",
                                           "anchoring",
                                           "antialiasing",
                                           "application base",
                                           "application domain (AppDomain)",
                                           "application manifest",
                                           "application state",
                                           "ASP.NET",
                                           "ASP.NET application services database",
                                           "ASP.NET mobile controls",
                                           "ASP.NET mobile Web Forms",
                                           "ASP.NET page",
                                           "ASP.NET server control",
                                           "ASP.NET Web application",
                                           "assembly",
                                           "assembly cache",
                                           "assembly manifest",
                                           "assembly metadata",
                                           "assertion (Assert)",
                                           "association class",
                                           "ASSOCIATORS OF",
                                           "asynchronous method",
                                           "attribute",
                                           "authentication",
                                           "authorization",
                                           "autopostback",
                                           "bounds",
                                           "boxing",
                                           "C#",
                                           "card",
                                           "catalog",
                                           "CCW",
                                           "chevron",
                                           "chrome",
                                           "cHTML",
                                           "CIM",
                                           "CIM Object Manager",
                                           "CIM schema",
                                           "class",
                                           "client area",
                                           "client coordinates",
                                           "clip",
                                           "closed generic type",
                                           "CLR",
                                           "CLS",
                                           "CLS-compliant",
                                           "code access security",
                                           "code-behind class",
                                           "code-behind file",
                                           "code-behind page",
                                           "COM callable wrapper (CCW)",
                                           "COM interop",
                                           "Common Information Model (CIM)",
                                           "common language runtime",
                                           "common language runtime host",
                                           "Common Language Specification (CLS)",
                                           "common object file format (COFF)",
                                           "common type system (CTS)",
                                           "comparison evaluator",
                                           "composite control",
                                           "configuration file",
                                           "connection",
                                           "connection point",
                                           "constraint",
                                           "constructed generic type",
                                           "constructed type",
                                           "consumer",
                                           "container",
                                           "container control",
                                           "content page",
                                           "context",
                                           "context property",
                                           "contract",
                                           "control state",
                                           "cross-page posting",
                                           "CTS",
                                           "custom attribute (Attribute)",
                                           "custom control"
                                       };

    #endregion

    [WebMethod]
    [ScriptMethod (UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public BusinessObjectWithIdentityProxy[] Search (
        string prefixText,
        int? completionSetCount,
        string businessObjectClass,
        string businessObjectProperty,
        string businessObjectID,
        string args)
    {
      if (prefixText.Equals ("throw", StringComparison.OrdinalIgnoreCase))
        throw new Exception ("Test Exception");

      List<BusinessObjectWithIdentityProxy> persons = new List<BusinessObjectWithIdentityProxy>();
      foreach (Person person in XmlReflectionBusinessObjectStorageProvider.Current.GetObjects (typeof (Person)))
        persons.Add (
            new BusinessObjectWithIdentityProxy ((IBusinessObjectWithIdentity) person) { IconUrl = GetUrl (GetIcon ((IBusinessObject) person)) });

      foreach (string value in s_values)
        persons.Add (new BusinessObjectWithIdentityProxy { UniqueIdentifier = "invalid", DisplayName = value, IconUrl = GetUrl (IconInfo.Spacer) });

      List<BusinessObjectWithIdentityProxy> filteredPersons =
          persons.FindAll (person => person.DisplayName.StartsWith (prefixText, StringComparison.OrdinalIgnoreCase));

      filteredPersons.Sort ((left, right) => string.Compare (left.DisplayName, right.DisplayName, StringComparison.OrdinalIgnoreCase));

      return filteredPersons.ToArray();
    }

    private string GetUrl (IconInfo iconInfo)
    {
      return UrlUtility.GetAbsoluteUrl (new HttpContextWrapper (Context), iconInfo.Url);
    }

    private IconInfo GetIcon (IBusinessObject businessObject)
    {
      return BusinessObjectBoundWebControl.GetIcon (businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);
    }
  }
}