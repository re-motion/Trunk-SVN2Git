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
  public class ToTextBuilderXml : ToTextBuilderBase , IDisposable
  {
    private readonly DisableableXmlWriter _disableableWriter;
    //private readonly bool _allowPartialXml = false;
    private bool _openingTagWritten = false;

    //public ToTextBuilderXml (ToTextProvider toTextProvider, XmlWriter xmlWriter, bool writePartialXml)
    //  : base (toTextProvider)
    //{
    //  _allowPartialXml = writePartialXml;
    //  _disableableWriter = new DisableableXmlWriter (xmlWriter);
    //}

    public ToTextBuilderXml (ToTextProvider toTextProvider, XmlWriter xmlWriter)
      //: this (toTextProvider, xmlWriter, false)
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

    public override IToTextBuilderBase WriteDictionary (IDictionary dictionary)
    {
      SequenceXmlBegin (dictionary.GetType ().Name, "dictionary", "dictionary", null);
      foreach (DictionaryEntry de in dictionary)
      {
        _disableableWriter.WriteStartElement ("de");
        
        SequenceXmlBegin (null, null, "key", null);
        WriteElement (de.Key);
        SequenceEnd ();

        SequenceXmlBegin (null, null, "val", null);
        WriteElement (de.Value);
        SequenceEnd ();

        _disableableWriter.WriteEndElement ();
      }
      SequenceEnd ();
      return this;
    }


    public override IToTextBuilderBase WriteArray (Array array)
    {
      //throw new System.NotImplementedException ();

      var outerProduct = new OuterProductIndexGenerator (array);

      WriteSequenceArrayBegin();
      //SequenceBegin ("", "A ", "AE ", "~AE ","_AE ","_A"); 

      var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;    
    }

    public override IToTextBuilderBase WriteSequenceArrayBegin ()
    {
      //throw new System.NotImplementedException();
      SequenceXmlBegin (null, null, "array", "e");
      return this;
    }

    public override IToTextBuilderBase WriteRaw (object obj)
    {
      AssertIsInRawSequence ();
      _disableableWriter.WriteValue (obj);
      return this;
    }

    public override IToTextBuilderBase WriteInstanceBegin (Type type)
    {
      SequenceXmlBegin (type.Name, "instance", "seq", "e");
      return this;
    }




    protected override void SequenceEnd ()
    {
      SequenceXmlEnd ();
    }




    protected override void BeforeNewSequence ()
    {
      //base.BeforeNewSequence();
      PushSequenceState (SequenceState);
    }

    protected IToTextBuilderBase SequenceXmlBegin (string name, string sequenceType, string sequenceTag, string elementTag)
    {
      // Note: All arguments can be null 
      //ArgumentUtility.CheckNotNull ("sequenceTag", sequenceTag);

      BeforeNewSequence ();
      SequenceState = new SequenceStateHolder { 
        Name = name, SequenceType = sequenceType, SequenceTag = sequenceTag, ElementTag = elementTag, 
        SequenceStartWritten = Enabled
      };

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
      
      if (SequenceState.SequenceTag != null)
      {
        //_disableableWriter.WriteEndElement ();
        if (SequenceState.SequenceStartWritten)
        {
          _disableableWriter.WriteEndElementAlways();
        }
      }

      SequenceState = sequenceStack.Pop ();
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


    public void Open ()
    {
      PushSequenceState (new SequenceStateHolder());
      SequenceState = new SequenceStateHolder { 
        Name = null, SequenceType = null, SequenceTag = "remotion", ElementTag = null,
        SequenceStartWritten = Enabled
      };
      _disableableWriter.WriteStartElement (SequenceState.SequenceTag);
      _openingTagWritten = true;
    }

    public override void Close ()
    {
      if (!_openingTagWritten)
      {
        throw new InvalidOperationException ("ToTextBuilderXml.Close() was called without a prior call to Open().");
      }
      SequenceXmlEnd();
      Flush();
      _disableableWriter.Close();
    }

    void IDisposable.Dispose ()
    {
      Close();
    }


  }
}
