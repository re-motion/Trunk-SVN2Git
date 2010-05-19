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
using System.Collections;
using System.Xml;
using Remotion.Diagnostics.ToText.Infrastructure;
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

    public override IToTextBuilder WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    public override string ToString ()
    {
      //Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }

    public override IToTextBuilder Flush ()
    {
      _disableableWriter.Flush();
      return this;
    }

    public override IToTextBuilder WriteNewLine ()
    {
      if (AllowNewline)
      {
        _disableableWriter.WriteStartElement ("br");
        _disableableWriter.WriteEndElement ();
      }
      return this;
    }

    public override IToTextBuilder WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      throw new System.NotSupportedException ("ToTextBuilderXml does not support literal sequences.");
    }

    protected override IToTextBuilder SequenceBegin ()
    {
      return SequenceXmlBegin(null,null, "seq", "e");
    }



    public override IToTextBuilder WriteSequenceBegin ()
    {
      return SequenceBegin();
    }

    public override IToTextBuilder WriteRawStringUnsafe (string s)
    {
      _disableableWriter.WriteValue (s);
      return this;
    }

    public override IToTextBuilder WriteRawStringEscapedUnsafe (string s)
    {
      return WriteRawStringUnsafe(s);
    }


    public override IToTextBuilder WriteRawCharUnsafe (char c)
    {
      _disableableWriter.WriteValue (c);
      return this;
    }

    public override IToTextBuilder WriteEnumerable (IEnumerable enumerable)
    {
      SequenceXmlBegin (enumerable.GetType ().Name, "enumerable", "enumerable", "e");
      foreach (Object element in enumerable)
      {
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }

    public override IToTextBuilder WriteDictionary (IDictionary dictionary)
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

    public override IToTextBuilder indent ()
    {
      // intentionally do nothing
      return this;
    }

    public override IToTextBuilder unindent ()
    {
      // intentionally do nothing
      return this;
    }


    public override IToTextBuilder WriteArray (Array array)
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

    public override IToTextBuilder WriteSequenceArrayBegin ()
    {
      //throw new System.NotImplementedException();
      SequenceXmlBegin (null, null, "array", "e");
      return this;
    }

    public override IToTextBuilder WriteRaw (object obj)
    {
      AssertIsInRawSequence ();
      _disableableWriter.WriteValue (obj);
      return this;
    }

    public override IToTextBuilder WriteInstanceBegin (Type type, string shortTypeName)
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

    protected IToTextBuilder SequenceXmlBegin (string name, string sequenceType, string sequenceTag, string elementTag)
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


    protected override IToTextBuilder WriteMemberRaw (string name, Object obj)
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
