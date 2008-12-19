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
namespace Remotion.Diagnostics.ToText.Infrastructure
{
  public class ToTextBuilderSettings
  {
    public ToTextBuilderSettings ()
    {
      EnumerablePostfix = "}";
      EnumerableElementPostfix = "";
      EnumerableElementPrefix = "";
      EnumerableSeparator = ",";
      EnumerablePrefix = "{";

      ArrayPostfix = "}";
      ArrayElementPostfix = "";
      ArrayElementPrefix = "";
      ArraySeparator = ",";
      ArrayPrefix = "{";

      AutoIndentSequences = false;
    }

    public string ArrayPrefix { get; set; }
    public string ArrayElementPrefix { get; set; }
    public string ArrayElementPostfix { get; set; }
    public string ArraySeparator { get; set; }
    public string ArrayPostfix { get; set; }

    public string EnumerablePrefix { get; set; }
    public string EnumerableElementPrefix { get; set; }
    public string EnumerableElementPostfix { get; set; }
    public string EnumerableSeparator { get; set; }
    public string EnumerablePostfix { get; set; }

    public bool AutoIndentSequences { get; set; }
  }
}