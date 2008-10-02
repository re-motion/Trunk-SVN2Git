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
using Remotion.Diagnostics.ToText.Internal;
using Remotion.Utilities;
using Remotion.Text.StringExtensions;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilder : ToTextBuilderBase
  {
    private readonly DisableableWriter _disableableWriter;

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

    public override IToTextBuilder WriteTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    //--------------------------------------------------------------------------
    // Settings Properties
    //--------------------------------------------------------------------------


    //public override bool AllowNewline
    //{
    //  get { return _allowNewline; }
    //  set { _allowNewline = value; }
    //}

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


    public override IToTextBuilder Flush ()
    {
      _disableableWriter.Flush ();
      return this;
    }
 


    ////--------------------------------------------------------------------------
    //// Low Level Emitters
    ////--------------------------------------------------------------------------

    //public IToTextBuilder sf (string format, params object[] paramArray)
    //{
    //  return WriteRawStringUnsafe (string.Format (format, paramArray)); 
    //}


    public override IToTextBuilder WriteNewLine ()
    {
      if (AllowNewline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
      }
      return this;
    }




    public override IToTextBuilder WriteSequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      return SequenceLiteralBegin (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix);
    }

    protected override IToTextBuilder SequenceBegin ()
    {
      return SequenceLiteralBegin ("", "(", "", "", ",", ")");
    }


    //--------------------------------------------------------------------------
    // Low level Sequence Emitters
    //--------------------------------------------------------------------------


    protected IToTextBuilder SequenceLiteralBegin (string name, string sequencePrefix, string elementPrefix, string elementPostfix, string separator, string sequencePostfix)
    {
      BeforeNewSequence ();
      SequenceState = new SequenceStateHolder (name, sequencePrefix, elementPrefix, elementPostfix, separator, sequencePostfix, Enabled);
      _disableableWriter.Write (SequenceState.SequencePrefix);
      if (name.Length > 0)
      {
        _disableableWriter.Write (name);
        _disableableWriter.Write (": ");
      }
      return this;
    }



    //--------------------------------------------------------------------------
    // High Level Emitters
    //--------------------------------------------------------------------------

    public override IToTextBuilder WriteSequenceArrayBegin ()
    {
      WriteSequenceLiteralBegin ("", Settings.ArrayPrefix, Settings.ArrayElementPrefix,
        Settings.ArrayElementPostfix, Settings.ArraySeparator, Settings.ArrayPostfix
      );
      return this;
    }

    public override IToTextBuilder WriteArray (Array array)
    {
      var outerProduct = new OuterProductIndexGenerator (array);

      SequenceLiteralBegin ("", Settings.ArrayPrefix, Settings.ArrayElementPrefix,
                     Settings.ArrayElementPostfix, Settings.ArraySeparator, Settings.ArrayPostfix);

      //SequenceBegin ("", "A ", "AE ", "~AE ","_AE ","_A"); 

      var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;
    }


    public override IToTextBuilder WriteSequenceBegin ()
    {
      return SequenceLiteralBegin ("", "(", "", "", ",", ")");
    }

    public override IToTextBuilder WriteRawStringUnsafe (string s)
    {
      _disableableWriter.Write (s);
      return this;
    }


    public override IToTextBuilder WriteRawStringEscapedUnsafe (string s)
    {
      EscapeString (s, _disableableWriter);
      return this;
    }


    public override IToTextBuilder WriteRawCharUnsafe (char c)
    {
      _disableableWriter.Write (c);
      return this;
    }


    public override IToTextBuilder WriteEnumerable (IEnumerable enumerable)
    {
      SequenceLiteralBegin ("", Settings.EnumerablePrefix, Settings.EnumerableElementPrefix,
        Settings.EnumerableElementPostfix, Settings.EnumerableSeparator, Settings.EnumerablePostfix);
      foreach (Object element in enumerable)
      {
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }


    public override IToTextBuilder WriteDictionary (IDictionary dictionary)
    {
      SequenceLiteralBegin ("", Settings.EnumerablePrefix, Settings.EnumerableElementPrefix,
        Settings.EnumerableElementPostfix, Settings.EnumerableSeparator, Settings.EnumerablePostfix);
      foreach (DictionaryEntry de in dictionary)
      {
        SequenceLiteralBegin ("", "", "", "", ":", "");
        WriteElement (de.Key);
        WriteElement (de.Value);
        SequenceEnd ();
      }
      SequenceEnd ();
      return this;
    }



    public override IToTextBuilder WriteRaw (Object obj)
    {
      AssertIsInRawSequence ();
      _disableableWriter.Write (obj);
      return this;
    }

    public override IToTextBuilder WriteInstanceBegin (Type type, string shortTypeName)
    {
      string typeName = shortTypeName ?? type.Name;
      SequenceLiteralBegin ("", "[" + typeName, "", "", ",", "]");
      if (typeName.Length > 0)
      {
        _disableableWriter.WriteDelayedAsPrefix (" ");
      }
      return this;
    }

    //--------------------------------------------------------------------------
    // High Level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.ClearDelayedPrefix(); // sequence closes = > clear delayed element separator 
      
      //_disableableWriter.Write (SequenceState.SequencePostfix);

      // Always write the sequence end if the start was written
      if (SequenceState.SequenceStartWritten)
      {
        _disableableWriter.WriteAlways (SequenceState.SequencePostfix);
      }

      SequenceState = sequenceStack.Pop ();

      AfterWriteElement (); 
    }

    public override void Close ()
    {
      _disableableWriter.Close ();
    }


    //--------------------------------------------------------------------------
    // High Level Complexity Switching Emitters
    //--------------------------------------------------------------------------


    //--------------------------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------------------------


    private static void EscapeString (string s, DisableableWriter disableableWriter)
    {
      var mapping = new Dictionary<char, string> { { '"', "\\\"" }, { '\n', "\\n" }, { '\r', "\\r" }, { '\t', "\\t" }, { '\\', "\\\\" }, { '\b', "\\b" }, { '\v', "\\v" }, { '\f', "\\f" } };
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

    protected override IToTextBuilder WriteMemberRaw (string name, Object obj)
    {
      SequenceLiteralBegin ("", name + "=", "", "", "", "");
      ToTextProvider.ToText (obj, this);
      SequenceEnd ();

      return this;
    }





  }
}