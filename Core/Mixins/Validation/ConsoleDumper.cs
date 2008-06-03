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
using System.Collections.Generic;
using System.Text;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.Validation;
using Remotion.Utilities;

namespace Remotion.Mixins.Validation
{
  public static class ConsoleDumper
  {
    public static void DumpValidationResults (IEnumerable<ValidationResult> results)
    {
      ArgumentUtility.CheckNotNull ("results", results);

      foreach (ValidationResult result in results)
      {
        if (result.TotalRulesExecuted == 0)
        {
          //Console.ForegroundColor = ConsoleColor.DarkGray;
          //Console.WriteLine ("No rules found for {0} '{1}'", result.Definition.GetType ().Name, result.Definition.FullName);
        }
        else if (result.TotalRulesExecuted != result.Successes.Count)
        {
					using (ConsoleUtility.EnterColorScope (ConsoleColor.Gray, null))
					{
						Console.WriteLine ("{0} '{1}', {2} rules executed", result.Definition.GetType().Name, result.Definition.FullName, result.TotalRulesExecuted);
						DumpContext (result);
					}
        }
        DumpResultList ("unexpected exceptions", result.Exceptions, ConsoleColor.White, ConsoleColor.DarkRed);
				// DumpResultList ("successes", result.Successes, ConsoleColor.Green, null);
				DumpResultList ("warnings", result.Warnings, ConsoleColor.Yellow, null);
				DumpResultList ("failures", result.Failures, ConsoleColor.Red, null);
      }
    }

    private static void DumpContext (ValidationResult result)
    {
      string contextString = result.GetParentDefinitionString();
      if (contextString.Length > 0)
        Console.WriteLine ("Context: " + contextString);
    }

    private static void DumpResultList<T> (string title, List<T> resultList, ConsoleColor foreColor, ConsoleColor? backColor) where T : IDefaultValidationResultItem
    {
      if (resultList.Count > 0)
      {
				using (ConsoleUtility.EnterColorScope (foreColor, backColor))
				{
					Console.WriteLine ("  {0} - {1}", title, resultList.Count);
					foreach (T resultItem in resultList)
						Console.WriteLine ("    {0} ({1})", resultItem.Message, resultItem.Rule.RuleName);
				}
      }
    }
  }
}
