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
  //public interface IToTextBuilder
  //{
  //  ToTextBuilderSettings Settings { get; }
  //  bool UseMultiLine { get; set; }
  //  bool Enabled { get; set; }
  //  ToTextBuilder seperator { get; }
  //  ToTextBuilder comma { get; }
  //  ToTextBuilder colon { get; }
  //  ToTextBuilder semicolon { get; }
  //  SequenceStateHolder SequenceState { get; protected set; }
  //  ToTextBuilderBase.ToTextBuilderOutputComplexityLevel OutputComplexity { get; protected set; }
  //  bool IsInSequence { get; }
  //  ToTextProvider ToTextProvider { get; set; }
  //  IToTextBuilderBase cSkeleton { get; }
  //  IToTextBuilderBase cBasic { get; }
  //  IToTextBuilderBase cMedium { get; }
  //  IToTextBuilderBase cComplex { get; }
  //  IToTextBuilderBase cFull { get; }
  //  IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderBase.ToTextBuilderOutputComplexityLevel complexityLevel);
  //  string CheckAndConvertToString ();
  //  ToTextBuilder ToText (object obj);
  //  IToTextBuilderBase Flush ();
  //  IToTextBuilderBase sf (string format, params object[] paramArray);
  //  ToTextBuilder AppendNewLine ();
  //  ToTextBuilder nl ();
  //  ToTextBuilder AppendSpace ();
  //  IToTextBuilderBase space ();
  //  ToTextBuilder AppendTabulator ();
  //  ToTextBuilder tab ();
  //  ToTextBuilder AppendSeperator ();
  //  ToTextBuilder AppendComma ();
  //  ToTextBuilder AppendColon ();
  //  ToTextBuilder AppendSemiColon ();
  //  IToTextBuilderBase AppendArray (Array array);
  //  IToTextBuilderBase AppendString (string s);
  //  ToTextBuilder AppendEscapedString (string s);
  //  ToTextBuilder sEsc (string s);
  //  IToTextBuilderBase AppendChar (char c);
  //  IToTextBuilderBase AppendMember (string name, Object obj);
  //  IToTextBuilderBase AppendEnumerable (IEnumerable collection);
  //  IToTextBuilderBase array (Array array);
  //  IToTextBuilderBase Append (Object obj);
  //  IToTextBuilderBase ToTextString (string s);
  //  void OutputDisable ();
  //  void OutputSkeleton ();
  //  void OutputBasic ();
  //  void OutputMedium ();
  //  void OutputComplex ();
  //  void OutputFull ();
  //  IToTextBuilderBase ts (object obj);
  //  IToTextBuilderBase AppendSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
  //  IToTextBuilderBase sb ();
  //  IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
  //  IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix);
  //  IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix);
  //  IToTextBuilderBase s (string s);
  //  IToTextBuilderBase AppendMember<T> (Expression<Func<object, T>> expression);
  //  IToTextBuilderBase AppendMemberNonSequence (string name, Object obj);
  //  IToTextBuilderBase m (Object obj);
  //  IToTextBuilderBase m (string name, Object obj, bool honorSequence);
  //  IToTextBuilderBase m<T> (Expression<Func<object, T>> expression);
  //  IToTextBuilderBase m (string name, Object obj);
  //  IToTextBuilderBase collection (IEnumerable collection);
  //  IToTextBuilderBase AppendToText (Object obj);
  //  IToTextBuilderBase tt (Object obj);
  //  IToTextBuilderBase tt (Object obj, bool honorSequence);
  //  IToTextBuilderBase AppendToTextNonSequence (Object obj);
  //  IToTextBuilderBase Append (string s);
  //  IToTextBuilderBase beginInstance (Type type);
  //  IToTextBuilderBase endInstance ();
  //  IToTextBuilderBase AppendSequenceEnd ();
  //  IToTextBuilderBase se ();
  //  IToTextBuilderBase AppendSequenceElement (object obj);
  //  IToTextBuilderBase e (object obj);
  //  IToTextBuilderBase AppendSequenceElements (params object[] sequenceElements);
  //  IToTextBuilderBase elements (params object[] sequenceElements);
  //  IToTextBuilderBase elementsNumbered (string s1, int i0, int i1);
  //}

  public class ToTextBuilder : ToTextBuilderBase
  {
    /* Planned Features:
     * Start-/End(class)
     * Start-/EndCollection(class)
     * Start-/EndCollectionDimension(class)
     * Start-/EndCollectionEntry(class): seperator
     * 
     * s ... append string
     * sf ... append formatted string
     * nl ... append newline
     * space, tab ... append whitespace
     * m ... named class member
     * c ... class
     * 
     * XML: Support text to be added to be processed to become XML compatible ("<" -> "&lt;" etc). Use CDATA ?
    */

    private readonly DisableableWriter _disableableWriter;

    private bool _useMultiline = true;


    public ToTextBuilder (ToTextProvider toTextProvider, TextWriter textWriter)
      : base (toTextProvider)
    {
      //_toTextProvider = toTextProvider;
      _disableableWriter = new DisableableWriter (textWriter);
      Settings = new ToTextBuilderSettings ();
      //OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic;
      //SequenceState = null;
    }

    public ToTextBuilder (ToTextProvider toTextProvider)
      : this (toTextProvider, new StringWriter())
    {
    }

    public ToTextBuilderSettings Settings { get; private set; }

    public override IToTextBuilderBase AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
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



    public override ToTextBuilder ToText (object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }


    //--------------------------------------------------------------------------
    // Before/After Element
    //--------------------------------------------------------------------------

    protected override void BeforeAppendElement ()
    {
      if (IsInSequence)
      {
        _disableableWriter.Write (SequenceState.Counter == 0 ? SequenceState.FirstElementPrefix : SequenceState.OtherElementPrefix);
      }
    }

    protected override void AfterAppendElement ()
    {
      if (IsInSequence)
      {
        _disableableWriter.Write (SequenceState.ElementPostfix);
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

    public override IToTextBuilderBase sf (string format, params object[] paramArray)
    {
      return AppendString (string.Format (format, paramArray));
    }


    public override ToTextBuilder AppendNewLine ()
    {
      if (_useMultiline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
      }
      return this;
    }

    public override ToTextBuilder nl ()
    {
      AppendNewLine ();
      return this;
    }


    public override ToTextBuilder AppendSpace ()
    {
      _disableableWriter.Write (" ");
      return this;
    }

    public override IToTextBuilderBase space ()
    {
      AppendSpace ();
      return this;
    }

    // TODO?: Introduce highlevel sibling "Indent" ?
    public override ToTextBuilder AppendTabulator ()
    {
      _disableableWriter.Write ("\t");
      return this;
    }

    public override ToTextBuilder tab ()
    {
      AppendTabulator ();
      return this;
    }


    public override ToTextBuilder AppendSeperator ()
    {
      _disableableWriter.Write (",");
      return this;
    }

    //public override ToTextBuilder seperator
    //{
    //  get { AppendSeperator (); return this; }
    //}


    public override ToTextBuilder AppendComma ()
    {
      _disableableWriter.Write (",");
      return this;
    }

    //public override ToTextBuilder comma
    //{
    //  get { AppendComma (); return this; }
    //}


    public override ToTextBuilder AppendColon ()
    {
      _disableableWriter.Write (":");
      return this;
    }

    //public override ToTextBuilder colon
    //{
    //  get { AppendColon (); return this; }
    //}


    public override ToTextBuilder AppendSemiColon ()
    {
      _disableableWriter.Write (";");
      return this;
    }

    //public override ToTextBuilder semicolon
    //{
    //  get { AppendSemiColon (); return this; }
    //}


    protected override ToTextBuilder AppendObjectToString (object obj)
    {
      _disableableWriter.Write (obj.ToString ());
      return this;
    }


    //--------------------------------------------------------------------------
    // Low level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override ToTextBuilder SequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      _sequenceStack.Push (SequenceState);
      SequenceState = new SequenceStateHolder (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);

      _disableableWriter.Write (SequenceState.SequencePrefix);

      return this;
    }

    //public IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    //{
    //  return AppendSequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    //}

    //public IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix)
    //{
    //  return AppendSequenceBegin (sequencePrefix, "", separator, "", sequencePostfix);
    //}

    //public IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix)
    //{
    //  return AppendSequenceBegin (sequencePrefix, "", ",", "", sequencePostfix);
    //}


    //--------------------------------------------------------------------------
    // High Level Emitters
    //--------------------------------------------------------------------------

    public override IToTextBuilderBase AppendArray (Array array)
    {
      var outerProduct = new OuterProductIndexGenerator (array);
      SequenceBegin (Settings.ArrayPrefix, Settings.ArrayFirstElementPrefix,
                     Settings.ArrayOtherElementPrefix, Settings.ArrayElementPostfix, Settings.ArrayPostfix);
      var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;
    }

    public override IToTextBuilderBase AppendString (string s)
    {
      _disableableWriter.Write (s);
      return this;
    }

    public override ToTextBuilder AppendEscapedString (string s)
    {
      EscapeString(s,_disableableWriter);
      return this;
    }

    public override ToTextBuilder sEsc (string s)
    {
      return AppendEscapedString (s);
    }

    public override IToTextBuilderBase AppendChar (char c)
    {
      _disableableWriter.Write (c);
      return this;
    }

    public override IToTextBuilderBase AppendMember (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      BeforeAppendElement ();
      AppendMemberRaw (name, obj);
      AfterAppendElement ();
      return this;
    }


    protected override ToTextBuilder AppendMemberRaw (string name, Object obj)
    {
      SequenceBegin (name + "=", "", "", "", "");
      _toTextProvider.ToText (obj, this);
      SequenceEnd ();

      return this;
    }


    public override IToTextBuilderBase AppendEnumerable (IEnumerable collection)
    {
      SequenceBegin (Settings.EnumerablePrefix, Settings.EnumerableFirstElementPrefix,
        Settings.EnumerableOtherElementPrefix, Settings.EnumerableElementPostfix, Settings.EnumerablePostfix);
      foreach (Object element in collection)
      {
        AppendToText (element);
      }
      SequenceEnd ();
      return this;
    }


    public override IToTextBuilderBase array (Array array)
    {
      return AppendArray (array);
    }


    //public IToTextBuilderBase tt (Object obj)
    //{
    //  return AppendToText (obj);
    //}

    //public IToTextBuilderBase tt (Object obj, bool honorSequence)
    //{
    //  return honorSequence ? AppendToText (obj) : AppendToTextNonSequence (obj);
    //}



    //public ToTextBuilder AppendToTextNonSequence (Object obj)
    //{
    //  _AppendToText (obj);
    //  return this;
    //}


    public override IToTextBuilderBase Append (Object obj)
    {
      _disableableWriter.Write (obj);
      return this;
    }


    public override IToTextBuilderBase ToTextString (string s)
    {
      return AppendString (s);
    }


    //--------------------------------------------------------------------------
    // High Level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.Write (SequenceState.SequencePostfix);

      SequenceState = _sequenceStack.Pop ();
    }


    //--------------------------------------------------------------------------
    // High Level Complexity Switching Emitters
    //--------------------------------------------------------------------------


    //--------------------------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------------------------


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

    // TODO: Move to String Extension Class
  }
}