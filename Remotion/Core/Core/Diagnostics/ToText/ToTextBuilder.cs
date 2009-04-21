// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Collections.Generic;
using System.IO;
using Remotion.Diagnostics.ToText.Infrastructure;
using Remotion.Utilities;

namespace Remotion.Diagnostics.ToText
{
  public class ToTextBuilder : ToTextBuilderBase
  {
    private readonly DisableableWriter _disableableWriter;
    private readonly Stack<int> _indendationStack = new Stack<int> (16);

    public ToTextBuilder (ToTextProvider toTextProvider, TextWriter textWriter)
      : base (toTextProvider)
    {
      IndentationString = "  ";
      IndendationLevel = 0;
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


    public override bool Enabled
    {
      get { return _disableableWriter.Enabled; }
      set { _disableableWriter.Enabled = value; }
    }


    //--------------------------------------------------------------------------
    // Final Output Methods
    //--------------------------------------------------------------------------

    public override string ToString ()
    {
      //Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }



    //--------------------------------------------------------------------------
    // Before/After Element
    //--------------------------------------------------------------------------

    protected override void BeforeWriteElement ()
    {
      if (IsInSequence)
      {
        _disableableWriter.Write (SequenceState.ElementPrefix);
      }
    }

    protected override void AfterWriteElement ()
    {
      if (IsInSequence)
      {
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

    public override IToTextBuilder WriteNewLine ()
    {
      if (AllowNewline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
        for (int i = 0; i < IndendationLevel; i++)
        {
          _disableableWriter.Write (IndentationString);
        }
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
    // Indentation
    //--------------------------------------------------------------------------

    public object IndentationString
    {
      get;
      set;
    }

    private int IndendationLevel
    {
      get;
      set;
    }

    public override IToTextBuilder indent ()
    {
      _indendationStack.Push (IndendationLevel);
      ++IndendationLevel;
      return this;
    }

    public override IToTextBuilder unindent ()
    {
      Assertion.IsTrue (_indendationStack.Count > 0, "unindent called without pairing call to indent.");
      IndendationLevel = _indendationStack.Pop ();
      return this;
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
      
      // Always write the sequence end if the start was written
      if (SequenceState.SequenceStartWritten)
      {
        _disableableWriter.WriteAlways (SequenceState.SequencePostfix);
      }

      SequenceState = sequenceStack.Pop ();

      if (Settings.AutoIndentSequences)
      {
        unindent ();
        //WriteNewLine ();
        //_disableableWriter.WriteDelayedAsPrefix (Environment.NewLine);
      }

      AfterWriteElement ();
    }

    protected override void BeforeNewSequence ()
    {
      if (Settings.AutoIndentSequences)
      {
        //_disableableWriter.ClearDelayedPrefix (); 
        WriteNewLine ();
        indent ();
      }

      BeforeWriteElement ();
      PushSequenceState (SequenceState);
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