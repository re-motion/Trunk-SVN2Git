// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting
{
  /// <summary>
  /// Constants for common JavaScript scripts.
  /// </summary>
  public static class CommonJavaScripts
  {
    /// <summary>
    /// Closes the current window.
    /// </summary>
    public static readonly string SelfClose = "self.close();";

    public static string CreateAutoCompleteWebServiceRequest (
        [NotNull] string autoCompleteTextValueInputFieldId,
        [NotNull] string searchText,
        int completionSetCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("autoCompleteTextValueInputFieldId", autoCompleteTextValueInputFieldId);
      ArgumentUtility.CheckNotNullOrEmpty ("searchText", searchText);

      return string.Format (@"
CallWebService = function() {{
  var input = $('#{0}');
  var options = input.getAutoCompleteSearchParameters('{1}');
  
  var data = options.params;
  data['searchString'] = options.searchString;
  data['completionSetCount'] = {2};

  data = Sys.Serialization.JavaScriptSerializer.serialize(data);
  console.log(data);

  var result = null;
  var request = {{
    async:false,
    type:'POST',
    contentType:'application/json; charset=utf-8',
    url:options.serviceUrl + '/' + options.serviceMethodSearch,
    data:data,
    dataType:'json',
    success:function(res, ctx, methodName){{ console.log('RETURN' + res.d); result = res.d; }},
    error:function(err, ctx, methodName){{ console.log('ERRRETURN'); console.log(err); console.log(request.data); console.log(request.url); }}
  }};

  $.ajax(request);
  return result;
}};
return CallWebService();", autoCompleteTextValueInputFieldId, searchText, completionSetCount);
    }
  }
}