﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Text;

namespace Remotion.Web.UI.Controls.Hotkey
{
  public class HotkeyParser
  {
    private const char c_hotkeyMarker = '&';

    public HotkeyParser ()
    {
    }

    public TextWithHotkey Parse (string value)
    {
      if (string.IsNullOrEmpty (value))
        return new TextWithHotkey (string.Empty, null);

      var resultBuilder = new StringBuilder (value.Length);
      int? hotkeyIndex = null;
      for (int i = 0; i < value.Length; i++)
      {
        var currentChar = value[i];
        if (currentChar == c_hotkeyMarker && i + 1 < value.Length)
        {
          if (char.IsLetterOrDigit (value, i + 1))
          {
            if (hotkeyIndex.HasValue)
              return new TextWithHotkey (value, null);

            hotkeyIndex = i;
            continue;
          }
        }

        resultBuilder.Append (currentChar);
      }

      return new TextWithHotkey (resultBuilder.ToString(), hotkeyIndex);
    }
  }
}