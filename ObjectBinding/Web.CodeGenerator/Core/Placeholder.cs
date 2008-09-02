/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

// --------------------------------------------------------------------------------------
// $Workfile: Placeholder.cs $
// $Revision: 1 $ of $Date: 10.03.06 15:02 $ by $Author: Harald-rene.flasch $
//
// Copyright 2006
// rubicon informationstechnologie gmbh
// --------------------------------------------------------------------------------------

using System;

namespace Remotion.ObjectBinding.Web.CodeGenerator
{
	public enum DefinedPlaceholder {
		// for each class
		INCLUDE_FOREACHCLASS,
		REPEAT_FOREACHCLASS_BEGIN,
		REPEAT_FOREACHCLASS_END,

		// for each property
		INCLUDE_FOREACHPROPERTY,
		REPEAT_FOREACHPROPERTY_BEGIN,
		REPEAT_FOREACHPROPERTY_END,
		REPEAT_FOREACHREFERENCEDPROPERTY_BEGIN,  // (islist=true|false)
		REPEAT_FOREACHREFERENCEDPROPERTY_END,

		// for each enum
		INCLUDE_FOREACHENUM,
		REPEAT_FOREACHENUM_BEGIN,
		REPEAT_FOREACHENUM_END,

		// project
		PROJECT_ROOTNAMESPACE,

		// domain
		DOMAIN_ASSEMBLYNAME,
		DOMAIN_ROOTNAMESPACE,
		DOMAIN_CLASSNAME,
		DOMAIN_ENUMNAME,
		DOMAIN_REFERENCEDCLASSNAME,
		DOMAIN_PROPERTYNAME,
		DOMAIN_QUALIFIEDCLASSTYPENAME,

		// others...
		CONTROLTYPE,
		ADDITIONALATTRIBUTES,
		ADDITIONALELEMENTS,
		ROOT_TEMPLATE_DIR,
		ROOT_TARGET_DIR, 
    WEB_CLIENT_GUID
	}

	public class Placeholder
	{
		private static string s_prefix = "$";
		private static string s_postfix = "$";

		public static string Prefix
		{
			set { s_prefix = value; }
			get { return s_prefix; }
		}

		public static string Postfix
		{
			set { s_postfix = value; }
			get { return s_postfix; }
		}

		public static string ToString(DefinedPlaceholder placeholder)
		{
			return s_prefix + placeholder.ToString() + s_postfix;
		}
	}
}
