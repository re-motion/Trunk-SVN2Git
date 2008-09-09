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
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilder : ToTextBuilderBase
  {
    private readonly DisableableWriter _disableableWriter;

    private bool _useMultiline = true;


    public ToTextBuilder (ToTextProvider toTextProvider, TextWriter textWriter)
      : base (toTextProvider)
    {
      _disableableWriter = new DisableableWriter (textWriter);
      Settings = new ToTextBuilderSettings ();
    }

    public ToTextBuilder (ToTextProvider toTextProvider)
      : this (toTextProvider, new StringWriter())
    {
    }

    public ToTextBuilderSettings Settings { get; private set; }

    public override IToTextBuilderBase WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    //--------------------------------------------------------------------------
    // Settings Properties
    //--------------------------------------------------------------------------


    public override bool UseMultiLine
    {
      get { return _useMultiline; }
      set { _useMultiline = value; }
    }

    public override bool Enabled
    {
      get { return _disableableWriter.Enabled; }
      set { _disableableWriter.Enabled = value; }
    }


    //--------------------------------------------------------------------------
    // Final Output Methods
    //--------------------------------------------------------------------------

    public override string CheckAndConvertToString ()
    {
      Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }

    //public override string ToString ()
    //{
    //  return _disableableWriter.ToString ();
    //}


    //--------------------------------------------------------------------------
    // Before/After Element
    //--------------------------------------------------------------------------

    protected override void BeforeWriteElement ()
    {
      if (IsInSequence)
      {
        //_disableableWriter.Write (SequenceState.Counter == 0 ? SequenceState.ElementPrefix : SequenceState.Separator);
        _disableableWriter.Write (SequenceState.ElementPrefix);
      }
    }

    protected override void AfterWriteElement ()
    {
      if (IsInSequence)
      {
        //_disableableWriter.Write (SequenceState.ElementPostfix);
        _disableableWriter.Write (SequenceState.ElementPostfix);
        _disableableWriter.WriteDelayedAsPrefix (SequenceState.Separator);
        SequenceState.IncrementCounter ();
      }
    }


    //--------------------------------------------------------------------------
    // Special Emitters
    //--------------------------------------------------------------------------


    public override IToTextBuilderBase Flush ()
    {
      _disableableWriter.Flush ();
      return this;
    }
 


    //--------------------------------------------------------------------------
    // Low Level Emitters
    //--------------------------------------------------------------------------

    public IToTextBuilderBase sf (string format, params object[] paramArray)
    {
      return WriteRawStringUnsafe (string.Format (format, paramArray)); 
    }


    public override IToTextBuilderBase WriteNewLine ()
    {
      if (_useMultiline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
      }
      return this;
    }

    //public override IToTextBuilderBase nl ()
    //{
    //  WriteNewLine ();
    //  return this;
    //}


    //protected override IToTextBuilderBase WriteObjectToString (object obj)
    //{
    //  _disableableWriter.Write (obj.ToString ());
    //  return this;
    //}


    //--------------------------------------------------------------------------
    // Low level Sequence Emitters
    //--------------------------------------------------------------------------

    //protected override IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    //{
    //  BeforeWriteElement();

    //  sequenceStack.Push (SequenceState);

    //  SequenceState = new SequenceStateHolder (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);

    //  SequenceBeginWritePart (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);

    //  return this;
    //}

    protected override void SequenceBeginWritePart (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      _disableableWriter.Write (SequenceState.SequencePrefix);
      if (name.Length > 0)
      {
        _disableableWriter.Write (name);
        _disableableWriter.Write (": ");
      }
    }


    //--------------------------------------------------------------------------
    // High Level Emitters
    //--------------------------------------------------------------------------

    public override IToTextBuilderBase WriteArray (Array array)
    {
      var outerProduct = new OuterProductIndexGenerator (array);

      SequenceBegin ("", Settings.ArrayPrefix, Settings.ArrayElementPrefix,
                     Settings.ArrayElementPostfix, Settings.ArraySeparator, Settings.ArrayPostfix);

      //SequenceBegin ("", "A ", "AE ", "~AE ","_AE ","_A"); 

      var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;
    }


    public override IToTextBuilderBase WriteRawStringUnsafe (string s)
    {
      _disableableWriter.Write (s);
      return this;
    }


    public override IToTextBuilderBase WriteRawStringEscapedUnsafe (string s)
    {
      EscapeString (s, _disableableWriter);
      return this;
    }


    public override IToTextBuilderBase WriteRawCharUnsafe (char c)
    {
      _disableableWriter.Write (c);
      return this;
    }


    public override IToTextBuilderBase WriteEnumerable (IEnumerable collection)
    {
      SequenceBegin ("", Settings.EnumerablePrefix, Settings.EnumerableElementPrefix,
        Settings.EnumerableElementPostfix, Settings.EnumerableSeparator, Settings.EnumerablePostfix);
      foreach (Object element in collection)
      {
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }


    public override IToTextBuilderBase array (Array array)
    {
      return WriteArray (array);
    }


    public override IToTextBuilderBase LowLevelWrite (Object obj)
    {
      _disableableWriter.Write (obj);
      return this;
    }

    public override IToTextBuilderBase WriteInstanceBegin (Type type)
    {
      //SequenceBegin ("", "[" + type.Name + "  ", "", "", ",", "]");
      SequenceBegin ("", "[" + type.Name, "", "", ",", "]");
      _disableableWriter.WriteDelayedAsPrefix ("  ");
      return this;
    }

    //--------------------------------------------------------------------------
    // High Level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.ClearDelayedPrefix(); // sequence closes = > clear delayed element separator 
      _disableableWriter.Write (SequenceState.SequencePostfix);

      SequenceState = sequenceStack.Pop ();

      AfterWriteElement (); 
    }




    //--------------------------------------------------------------------------
    // High Level Complexity Switching Emitters
    //--------------------------------------------------------------------------


    //--------------------------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------------------------


    // TODO: Move to String Extension Class
    private void EscapeString (string s, DisableableWriter disableableWriter)
    {
      var mapping = new Dictionary<char, string> () { { '"', "\\\"" }, { '\n', "\\n" }, { '\r', "\\r" }, { '\t', "\\t" }, { '\\', "\\\\" }, { '\b', "\\b" }, { '\v', "\\v" }, { '\f', "\\f" } };
      foreach (char c in s)
      {
        string mappedString;
        mapping.TryGetValue (c, out mappedString);
        if (mappedString == null)
        {
          disableableWriter.Write (c);
        }
        else
        {
          disableableWriter.Write (mappedString);
        }
      }
    }

  }
}