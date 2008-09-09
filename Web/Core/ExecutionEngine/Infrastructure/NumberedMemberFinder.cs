/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Reflection;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  ///   Finds and orders type members using a prefix/number pattern.
  /// </summary>
  /// <remarks>
  ///   This class is used by WxeStep subclasses that use numbered elements, like Step1, Step2, ... or Catch1, Catch2 etc.
  ///   Underlines may be appended to separate member names from class names in case the class itself is called Step&lt;n;gt;
  /// </remarks>
  internal class NumberedMemberFinder
  {
    /// <summary>
    ///   Returnes an orderd array of member infos.
    /// </summary>
    public static MemberInfo[] FindMembers (Type type, string prefix, MemberTypes memberType, BindingFlags bindingFlags)
    {
      NumberedMemberFinder finder = new NumberedMemberFinder (prefix);
      MemberInfo[] members = type.FindMembers (memberType, bindingFlags, new MemberFilter (finder.PrefixMemberFilter), null);

      int[] numbers = new int[members.Length];
      for (int i = 0; i < members.Length; ++i)
        numbers[i] = finder.GetStepNumber (members[i].Name);
      Array.Sort (numbers, members);

      return members;
    }

    private string _prefix;

    private NumberedMemberFinder (string prefix)
    {
      _prefix = prefix;
    }

    private bool PrefixMemberFilter (MemberInfo member, object param)
    {
      return GetStepNumber (member.Name) != -1;
    }

    private int GetStepNumber (string memberName)
    {
      if (! memberName.StartsWith (_prefix))
        return -1;
      string numStr = memberName.TrimEnd('_').Substring (_prefix.Length);
      if (numStr.Length == 0)
        return -1;
      double num;
      if (! double.TryParse (numStr, System.Globalization.NumberStyles.Integer, null, out num))
        return -1;
      return (int) num;
    }
  }
}