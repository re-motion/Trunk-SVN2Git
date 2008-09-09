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
  //  IToTextBuilderBase WriteArray (Array array);
  //  IToTextBuilderBase AppendString (string s);
  //  ToTextBuilder AppendEscapedString (string s);
  //  ToTextBuilder sEsc (string s);
  //  IToTextBuilderBase AppendChar (char c);
  //  IToTextBuilderBase WriteElement (string name, Object obj);
  //  IToTextBuilderBase WriteEnumerable (IEnumerable collection);
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
  //  IToTextBuilderBase WriteSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
  //  IToTextBuilderBase sb ();
  //  IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix);
  //  IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix);
  //  IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix);
  //  IToTextBuilderBase s (string s);
  //  IToTextBuilderBase WriteElement<T> (Expression<Func<object, T>> expression);
  //  IToTextBuilderBase WriteElement (string name, Object obj);
  //  IToTextBuilderBase e (Object obj);
  //  IToTextBuilderBase e (string name, Object obj, bool honorSequence);
  //  IToTextBuilderBase e<T> (Expression<Func<object, T>> expression);
  //  IToTextBuilderBase e (string name, Object obj);
  //  IToTextBuilderBase collection (IEnumerable collection);
  //  IToTextBuilderBase WriteElement (Object obj);
  //  IToTextBuilderBase e (Object obj);
  //  IToTextBuilderBase e (Object obj, bool honorSequence);
  //  IToTextBuilderBase WriteElement (Object obj);
  //  IToTextBuilderBase Append (string s);
  //  IToTextBuilderBase WriteInstanceBegin (Type type);
  //  IToTextBuilderBase WriteInstanceEnd ();
  //  IToTextBuilderBase WriteSequenceEnd ();
  //  IToTextBuilderBase se ();
  //  IToTextBuilderBase WriteElement (object obj);
  //  IToTextBuilderBase e (object obj);
  //  IToTextBuilderBase WriteSequenceElements (params object[] sequenceElements);
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
        _disableableWriter.Write (SequenceState.Counter == 0 ? SequenceState.FirstElementPrefix : SequenceState.OtherElementPrefix);
      }
    }

    protected override void AfterWriteElement ()
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

    public IToTextBuilderBase sf (string format, params object[] paramArray)
    {
      return WriteRawStringUnsafe (string.Format (format, paramArray)); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }


    public override IToTextBuilderBase WriteNewLine ()
    {
      if (_useMultiline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
      }
      return this;
    }

    public override IToTextBuilderBase nl ()
    {
      WriteNewLine ();
      return this;
    }


    //private IToTextBuilderBase AppendSpace ()
    //{
    //  _disableableWriter.Write (" ");
    //  return this;
    //}

    //public IToTextBuilderBase space ()
    //{
    //  AppendSpace ();
    //  return this;
    //}

    //// TODO?: Introduce highlevel sibling "Indent" ?
    //private IToTextBuilderBase AppendTabulator ()
    //{
    //  _disableableWriter.Write ("\t");
    //  return this;
    //}

    //public IToTextBuilderBase tab ()
    //{
    //  AppendTabulator ();
    //  return this;
    //}


    //public override IToTextBuilderBase AppendSeperator ()
    //{
    //  _disableableWriter.Write (",");
    //  return this;
    //}

    //public override ToTextBuilder seperator
    //{
    //  get { AppendSeperator (); return this; }
    //}


    //public override ToTextBuilder comma
    //{
    //  get { AppendComma (); return this; }
    //}


    //public override ToTextBuilder colon
    //{
    //  get { AppendColon (); return this; }
    //}


    //public override ToTextBuilder semicolon
    //{
    //  get { AppendSemiColon (); return this; }
    //}


    protected override IToTextBuilderBase WriteObjectToString (object obj)
    {
      _disableableWriter.Write (obj.ToString ());
      return this;
    }


    //--------------------------------------------------------------------------
    // Low level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override IToTextBuilderBase SequenceBegin (string name, string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      BeforeWriteElement(); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

      _sequenceStack.Push (SequenceState);

      SequenceState = new SequenceStateHolder (name, sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);

      _disableableWriter.Write (SequenceState.SequencePrefix);
      if (name.Length > 0)
      {
        _disableableWriter.Write (name);
        _disableableWriter.Write (": ");
      }

      return this;
    }

    //public IToTextBuilderBase sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    //{
    //  return WriteSequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    //}

    //public IToTextBuilderBase sb (string sequencePrefix, string separator, string sequencePostfix)
    //{
    //  return WriteSequenceBegin (sequencePrefix, "", separator, "", sequencePostfix);
    //}

    //public IToTextBuilderBase sb (string sequencePrefix, string sequencePostfix)
    //{
    //  return WriteSequenceBegin (sequencePrefix, "", ",", "", sequencePostfix);
    //}


    //--------------------------------------------------------------------------
    // High Level Emitters
    //--------------------------------------------------------------------------

    public override IToTextBuilderBase WriteArray (Array array)
    {
      var outerProduct = new OuterProductIndexGenerator (array);

      SequenceBegin ("", Settings.ArrayPrefix, Settings.ArrayFirstElementPrefix,
                     Settings.ArrayOtherElementPrefix, Settings.ArrayElementPostfix, Settings.ArrayPostfix);

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

    //public override IToTextBuilderBase AppendRawString (string s)
    //{
    //  AssertIsInRawSequence();
    //  WriteRawStringUnsafe (s);
    //  return this;
    //}

    public override IToTextBuilderBase WriteRawStringEscapedUnsafe (string s)
    {
      EscapeString (s, _disableableWriter);
      return this;
    }

    //public override IToTextBuilderBase AppendRawEscapedString (string s)
    //{
    //  WriteRawStringEscapedUnsafe(s);
    //  return this;
    //}

    public override IToTextBuilderBase sEsc (string s)
    {
      return WriteRawStringEscapedUnsafe (s); 
      
    }

    public override IToTextBuilderBase WriteRawCharUnsafe (char c)
    {
      _disableableWriter.Write (c);
      return this;
    }

    //public override IToTextBuilderBase AppendRawChar (char c)
    //{
    //  AssertIsInRawSequence ();
    //  WriteRawCharUnsafe (c);
    //  return this;
    //}

    //public override IToTextBuilderBase WriteElement (string name, Object obj)
    //{
    //  ArgumentUtility.CheckNotNull ("name", name);
    //  WriteMemberRaw (name, obj);
    //  return this;
    //}


    //protected override IToTextBuilderBase WriteMemberRaw (string name, Object obj)
    //{
    //  SequenceBegin ("", name + "=", "", "", "", "");
    //  _toTextProvider.ToText (obj, this);
    //  SequenceEnd ();

    //  return this;
    //}


    public override IToTextBuilderBase WriteEnumerable (IEnumerable collection)
    {
      SequenceBegin ("", Settings.EnumerablePrefix, Settings.EnumerableFirstElementPrefix,
        Settings.EnumerableOtherElementPrefix, Settings.EnumerableElementPostfix, Settings.EnumerablePostfix);
      foreach (Object element in collection)
      {
        //WriteElement (element);
        WriteElement (element);
      }
      SequenceEnd ();
      return this;
    }


    public override IToTextBuilderBase array (Array array)
    {
      return WriteArray (array);
    }


    //public IToTextBuilderBase e (Object obj)
    //{
    //  return WriteElement (obj);
    //}

    //public IToTextBuilderBase e (Object obj, bool honorSequence)
    //{
    //  return honorSequence ? WriteElement (obj) : WriteElement (obj);
    //}



    //public ToTextBuilder WriteElement (Object obj)
    //{
    //  _AppendToText (obj);
    //  return this;
    //}


    public override IToTextBuilderBase LowLevelWrite (Object obj)
    {
      _disableableWriter.Write (obj);
      return this;
    }


    //public override IToTextBuilderBase ToTextString (string s)
    //{
    //  return AppendString (s);
    //}


    //--------------------------------------------------------------------------
    // High Level Sequence Emitters
    //--------------------------------------------------------------------------

    protected override void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.Write (SequenceState.SequencePostfix);

      SequenceState = _sequenceStack.Pop ();

      AfterWriteElement (); // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
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