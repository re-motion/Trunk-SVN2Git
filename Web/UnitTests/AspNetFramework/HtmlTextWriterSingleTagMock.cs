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
using System.Collections;
using System.IO;
using System.Web.UI;

namespace Remotion.Web.UnitTests.AspNetFramework
{

public class HtmlTextWriterSingleTagMock: HtmlTextWriter
{
  private Hashtable _attributes = new Hashtable();
  private HtmlTextWriterTag _tag;

	public HtmlTextWriterSingleTagMock()
    : base (new StringWriter ())
	{
	}

  public Hashtable Attributes
  {
    get { return _attributes; }
  }

  public HtmlTextWriterTag Tag
  {
    get { return _tag; }
  }

  public override void AddAttribute (HtmlTextWriterAttribute key, string value, bool fEncode)
  {
    base.AddAttribute (key, value, fEncode);
    _attributes[key] = value;
  }

  public override void AddAttribute (HtmlTextWriterAttribute key, string value)
  {
    base.AddAttribute (key, value);
    _attributes[key] = value;
  }

  protected override void AddAttribute (string name, string value, HtmlTextWriterAttribute key)
  {
    base.AddAttribute (name, value, key);
    _attributes[key] = value;
  }

  public override void RenderBeginTag (HtmlTextWriterTag tagKey)
  {
    base.RenderBeginTag (tagKey);
    _tag = tagKey;
  }
}
}
