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
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilderXml : ToTextBuilderBase
  {
    //public DisableableXmlWriter XmlWriter { get; private set; }
    private readonly DisableableXmlWriter _disableableWriter;

    public ToTextBuilderXml (ToTextProvider toTextProvider, XmlWriter xmlWriter)
      : base (toTextProvider)
    {
      _disableableWriter = new DisableableXmlWriter (xmlWriter);
    }


    public override bool Enabled
    {
      get { return _disableableWriter.Enabled; }
      set { _disableableWriter.Enabled = value; }
    }

    public override IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      // TODO: Derive DisableableWriter interface, move to base class
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    public override string CheckAndConvertToString ()
    {
      Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }

    public override IToTextBuilderBase Flush ()
    {
      _disableableWriter.Flush();
      return this;
    }

    public override IToTextBuilderBase WriteNewLine ()
    {
      if (AllowNewline)
      {
        _disableableWriter.WriteStartElement ("br");
        _disableableWriter.WriteEndElement ();
      }
      return this;
    }

    public override IToTextBuilderBase WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      throw new System.NotSupportedException ("ToTextBuilderXml does not support literal sequences.");
    }

    protected override IToTextBuilderBase SequenceBegin ()
    {
      return SequenceXmlBegin(null,null, "seq", "e");
    }



    //protected override IToTextBuilderBase WriteObjectToString (object obj)
    //{
    //  _disableableWriter.WriteValue (obj.ToString ());
    //  return this;
    //}




    //protected override void SequenceBeginWritePart (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //protected override void SequenceBeginWritePart (SequenceStateHolder sequenceState)
    //{
    //  _disableableWriter.WriteStartElement ("seq");
    //  //_disableableWriter.Write (SequenceState.SequencePrefix);
    //  string name = sequenceState.Name;
    //  if (name.Length > 0)
    //  {
    //    _disableableWriter.WriteAttribute ("name",name);
    //  }
    //}

    public override IToTextBuilderBase WriteSequenceBegin ()
    {
      return SequenceBegin();
    }

    public override IToTextBuilderBase WriteRawStringUnsafe (string s)
    {
      _disableableWriter.WriteValue (s);
      return this;
    }

    public override IToTextBuilderBase WriteRawStringEscapedUnsafe (string s)
    {
      return WriteRawStringUnsafe(s);
    }

    //public override IToTextBuilderBase sEsc (string s)
    //{
    //  throw new System.NotImplementedException();
    //}

    public override IToTextBuilderBase WriteRawCharUnsafe (char c)
    {
      _disableableWriter.WriteValue (c);
      return this;
    }

    public override IToTextBuilderBase WriteEnumerable (IEnumerable enumerable)
    {
      SequenceXmlBegin (enumerable.GetType ().Name, "enumerable", "enumerable", "e");
      foreach (Object element in enumerable)
      {
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }

    public override IToTextBuilderBase WriteArray (Array array)
    {
      throw new System.NotImplementedException ();
      //  var outerProduct = new OuterProductIndexGenerator (array);

      //  SequenceBegin ("array", "", "", "", "", "");
      //  var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      //  outerProduct.ProcessOuterProduct (processor);
      //  SequenceEnd ();

      //  return this;
    }

    public override IToTextBuilderBase WriteSequenceArrayBegin ()
    {
      throw new System.NotImplementedException();
    }

    public override IToTextBuilderBase WriteRaw (object obj)
    {
      AssertIsInRawSequence ();
      _disableableWriter.WriteValue (obj);
      return this;
    }

    public override IToTextBuilderBase WriteInstanceBegin (Type type)
    {
      //throw new System.NotImplementedException ();
      SequenceXmlBegin (type.Name, "instance", "seq", "e");
      return this;
    }




    protected override void SequenceEnd ()
    {
      SequenceXmlEnd ();
    }

    //protected override IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //{
    //  throw new System.NotImplementedException();
    //}

    protected override void BeforeNewSequence () // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
      //base.BeforeNewSequence();
      PushSequenceState (SequenceState);
    }

    protected IToTextBuilderBase SequenceXmlBegin (string name, string sequenceType, string sequenceTag, string elementTag)
    {
      // Note: All arguments can be null 
      //ArgumentUtility.CheckNotNull ("sequenceTag", sequenceTag);

      BeforeNewSequence ();
      SequenceState = new SequenceStateHolder { Name = name, SequenceType = sequenceType, SequenceTag = sequenceTag, ElementTag = elementTag };

      //_disableableWriter.WriteStartElement ("e");
      //string elementTag = SequenceState.ElementTag;
      if (SequenceState.SequenceTag != null)
      {
        _disableableWriter.WriteStartElement (SequenceState.SequenceTag);
        _disableableWriter.WriteAttributeIfNotEmpty ("name", SequenceState.Name);
        _disableableWriter.WriteAttributeIfNotEmpty ("type", SequenceState.SequenceType);
      }

      //_disableableWriter.WriteStartElement ("seq");
      return this;
    }
    
    
    private void SequenceXmlEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      
      //_disableableWriter.WriteEndElement ();

      if (SequenceState.SequenceTag != null)
      {
        _disableableWriter.WriteEndElement ();
      }

      SequenceState = sequenceStack.Pop ();
      //AfterWriteElement (); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }


    protected override void BeforeWriteElement ()
    {
      ArgumentUtility.CheckNotNull ("SequenceState",SequenceState);
      //_disableableWriter.WriteStartElement ("e");
      string elementTag = SequenceState.ElementTag;
      if (elementTag != null)
      {
        _disableableWriter.WriteStartElement (elementTag);
      }
    }

    protected override void AfterWriteElement ()
    {
      ArgumentUtility.CheckNotNull ("SequenceState", SequenceState);
      if (SequenceState.ElementTag != null)
      {
        _disableableWriter.WriteEndElement ();
      }
    }


    protected override IToTextBuilderBase WriteMemberRaw (string name, Object obj)
    {
      //_disableableWriter.WriteStartElement ("var");
      //_disableableWriter.WriteAttribute ("name", name);
      //WriteElement (obj);
      //_disableableWriter.WriteEndElement ();

      SequenceXmlBegin (null, null, "var", null);
      _disableableWriter.WriteAttribute ("name", name);
      WriteElement (obj);
      SequenceXmlEnd ();

      return this;
    }


    public void Begin ()
    {
      PushSequenceState (new SequenceStateHolder());
      SequenceState = new SequenceStateHolder { Name = null, SequenceType = null, SequenceTag = "remotion", ElementTag = null };
      //sequenceStack.Push (SequenceState);
      //PushSequenceState (SequenceState);
      //PushSequenceState (null);
      _disableableWriter.WriteStartElement (SequenceState.SequenceTag);
      //SequenceXmlBegin (null, null, "remotion", null);

    }

    public void End ()
    {
      //_disableableWriter.WriteEndElement ();
      SequenceXmlEnd();
      Flush();
    }
  }
}
