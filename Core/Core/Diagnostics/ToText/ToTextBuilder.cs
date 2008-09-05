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
  public class ToTextBuilder
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
    private ToTextProvider _toTextProvider;

    private bool _useMultiline = true;
    private readonly Stack<SequenceStateHolder> _sequenceStack = new Stack<SequenceStateHolder> (16);

    public enum ToTextBuilderOutputComplexityLevel
    {
      Disable,
      Skeleton,
      Basic,
      Medium,
      Complex,
      Full,
    };


    public ToTextBuilder (ToTextProvider toTextProvider, TextWriter textWriter)
    {
      _toTextProvider = toTextProvider;
      _disableableWriter = new DisableableWriter (textWriter);
      Settings = new ToTextBuilderSettings ();
      OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic;
      SequenceState = null;
      //EnumerablePostfix = "}";
      //EnumerableElementPostfix = "";
      //EnumerableOtherElementPrefix = ",";
      //EnumerableFirstElementPrefix = "";
      //EnumerablePrefix = "{";
      //ArrayPostfix = "}";
      //ArrayElementPostfix = "";
      //ArrayOtherElementPrefix = ",";
      //ArrayFirstElementPrefix = "";
      //ArrayPrefix = "{";
    }

    public ToTextBuilder (ToTextProvider toTextProvider)
      : this (toTextProvider, new StringWriter())
    {
    }

    public ToTextBuilderSettings Settings { get; private set; }

    //public string ArrayPrefix { get; set; }
    //public string ArrayFirstElementPrefix { get; set; }
    //public string ArrayOtherElementPrefix { get; set; }
    //public string ArrayElementPostfix { get; set; }
    //public string ArrayPostfix { get; set; }

    //public string EnumerablePrefix { get; set; }
    //public string EnumerableFirstElementPrefix { get; set; }
    //public string EnumerableOtherElementPrefix { get; set; }
    //public string EnumerableElementPostfix { get; set; }
    //public string EnumerablePostfix { get; set; }

    public SequenceStateHolder SequenceState { get; private set; }

    public ToTextBuilderOutputComplexityLevel OutputComplexity { get; private set; }

    public void OutputDisable () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Disable; }
    public void OutputSkeleton () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Skeleton; }
    public void OutputBasic () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Basic; }
    public void OutputMedium () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Medium; }
    public void OutputComplex () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Complex; }
    public void OutputFull () { OutputComplexity = ToTextBuilderOutputComplexityLevel.Full; }

    public ToTextBuilder AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel complexityLevel)
    {
      _disableableWriter.Enabled = (OutputComplexity >= complexityLevel) ? true : false;
      return this;
    }

    public bool IsInSequence
    {
      get { return SequenceState != null; }
    }

    public ToTextProvider ToTextProvider
    {
      get { return _toTextProvider; }
      set { _toTextProvider = value; }
    }

    //--------------------------------------------------------------------------
    // Settings Properties
    //--------------------------------------------------------------------------
    // TODO?: Move to Settings object 
    
    //public string EnumerableBegin
    //{
    //  get { return EnumerablePrefix; }
    //  set { EnumerablePrefix = value; }
    //}

    //public string EnumerableSeparator
    //{
    //  get { return EnumerableOtherElementPrefix; }
    //  set { EnumerableOtherElementPrefix = value; }
    //}

    //public string EnumerableEnd
    //{
    //  get { return EnumerablePostfix; }
    //  set { EnumerablePostfix = value; }
    //}


    //public string ArrayBegin
    //{
    //  get { return ArrayPrefix; }
    //  set { ArrayPrefix = value; }
    //}

    //public string ArraySeparator
    //{
    //  get { return ArrayOtherElementPrefix; }
    //  set { ArrayOtherElementPrefix = value; }
    //}

    //public string ArrayEnd
    //{
    //  get { return ArrayPostfix; }
    //  set { ArrayPostfix = value; }
    //}

    public bool UseMultiLine
    {
      get { return _useMultiline; }
      set { _useMultiline = value; }
    }

    public bool Enabled
    {
      get { return _disableableWriter.Enabled; }
      set { _disableableWriter.Enabled = value; }
    }


    //--------------------------------------------------------------------------
    // Final Output Methods
    //--------------------------------------------------------------------------

    public string CheckAndConvertToString ()
    {
      Assertion.IsFalse (IsInSequence);
      return _disableableWriter.ToString ();
    }

    //public override string ToString ()
    //{
    //  return _disableableWriter.ToString ();
    //}



    public ToTextBuilder ToText (object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }


    //--------------------------------------------------------------------------
    // Before/After Element
    //--------------------------------------------------------------------------

    private void BeforeAppendElement ()
    {
      if (IsInSequence)
      {
        _disableableWriter.Write (SequenceState.Counter == 0 ? SequenceState.FirstElementPrefix : SequenceState.OtherElementPrefix);
      }
    }

    private void AfterAppendElement ()
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


    public ToTextBuilder Flush ()
    {
      _disableableWriter.Flush ();
      return this;
    }
 


    //--------------------------------------------------------------------------
    // Low Level Emitters
    //--------------------------------------------------------------------------

    public ToTextBuilder sf (string format, params object[] paramArray)
    {
      return AppendString (string.Format (format, paramArray));
    }


    public ToTextBuilder AppendNewLine ()
    {
      if (_useMultiline)
      {
        _disableableWriter.Write (System.Environment.NewLine);
      }
      return this;
    }

    public ToTextBuilder nl ()
    {
      AppendNewLine ();
      return this;
    }


    public ToTextBuilder AppendSpace ()
    {
      _disableableWriter.Write (" ");
      return this;
    }

    public ToTextBuilder space ()
    {
      AppendSpace ();
      return this;
    }

    // TODO?: Introduce highlevel sibling "Indent" ?
    public ToTextBuilder AppendTabulator ()
    {
      _disableableWriter.Write ("\t");
      return this;
    }

    public ToTextBuilder tab ()
    {
      AppendTabulator ();
      return this;
    }


    public ToTextBuilder AppendSeperator ()
    {
      _disableableWriter.Write (",");
      return this;
    }

    public ToTextBuilder seperator
    {
      get { AppendSeperator (); return this; }
    }


    public ToTextBuilder AppendComma ()
    {
      _disableableWriter.Write (",");
      return this;
    }

    public ToTextBuilder comma
    {
      get { AppendComma (); return this; }
    }


    public ToTextBuilder AppendColon ()
    {
      _disableableWriter.Write (":");
      return this;
    }

    public ToTextBuilder colon
    {
      get { AppendColon (); return this; }
    }


    public ToTextBuilder AppendSemiColon ()
    {
      _disableableWriter.Write (";");
      return this;
    }

    public ToTextBuilder semicolon
    {
      get { AppendSemiColon (); return this; }
    }


    private ToTextBuilder AppendObjectToString (object obj)
    {
      _disableableWriter.Write (obj.ToString ());
      return this;
    }

    public ToTextBuilder ts (object obj)
    {
      return AppendObjectToString (obj);
    }


    //--------------------------------------------------------------------------
    // Low level Sequence Emitters
    //--------------------------------------------------------------------------

    public ToTextBuilder AppendSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      BeforeAppendElement ();

      return SequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    }

    private ToTextBuilder SequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      _sequenceStack.Push (SequenceState);
      SequenceState = new SequenceStateHolder (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);

      _disableableWriter.Write (SequenceState.SequencePrefix);

      return this;
    }

    public ToTextBuilder sb (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      return AppendSequenceBegin (sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    }

    public ToTextBuilder sb (string sequencePrefix, string separator, string sequencePostfix)
    {
      return AppendSequenceBegin (sequencePrefix, "", separator, "", sequencePostfix);
    }

    public ToTextBuilder sb (string sequencePrefix, string sequencePostfix)
    {
      return AppendSequenceBegin (sequencePrefix, "", ",", "", sequencePostfix);
    }

    public ToTextBuilder sb ()
    {
      return AppendSequenceBegin ("(", "", ",", "", ")");
    }



    //--------------------------------------------------------------------------
    // High Level Emitters
    //--------------------------------------------------------------------------

    public ToTextBuilder AppendString (string s)
    {
      _disableableWriter.Write (s);
      return this;
    }

    public ToTextBuilder s (string s)
    {
      return AppendString (s);
    }

    public ToTextBuilder AppendEscapedString (string s)
    {
      EscapeString(s,_disableableWriter);
      return this;
    }

    public ToTextBuilder sEsc (string s)
    {
      return AppendEscapedString (s);
    }

    public ToTextBuilder AppendChar (char c)
    {
      _disableableWriter.Write (c);
      return this;
    }

    public ToTextBuilder AppendMember (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      BeforeAppendElement ();
      AppendMemberRaw (name, obj);
      AfterAppendElement ();
      return this;
    }

    public ToTextBuilder AppendMember<T> (Expression<Func<object, T>> expression)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      var variableName = RightUntilChar (expression.Body.ToString (), '.');
      var variableValue = expression.Compile ().Invoke (null);
      return AppendMember (variableName, variableValue);
    }



    public ToTextBuilder AppendMemberNonSequence (string name, Object obj)
    {
      ArgumentUtility.CheckNotNull ("name", name);
      AppendMemberRaw (name, obj);
      return this;
    }

    private ToTextBuilder AppendMemberRaw (string name, Object obj)
    {
      SequenceBegin (name + "=", "", "", "", "");
      _toTextProvider.ToText (obj, this);
      SequenceEnd ();

      return this;
    }

    public ToTextBuilder m (Object obj)
    {
      return AppendToText (obj);
    }

    public ToTextBuilder m (string name, Object obj, bool honorSequence)
    {
      return honorSequence ? AppendMember (name, obj) : AppendMemberNonSequence (name, obj);
    }

    public ToTextBuilder m<T> (Expression<Func<object, T>> expression)
    {
      return AppendMember (expression);
    }

    public ToTextBuilder m (string name, Object obj)
    {
      return AppendMember (name, obj);
    }


    public ToTextBuilder AppendEnumerable (IEnumerable collection)
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

    public ToTextBuilder collection (IEnumerable collection)
    {
      return AppendEnumerable (collection);
    }


    public ToTextBuilder AppendArray (Array array)
    {
      var outerProduct = new OuterProductIndexGenerator (array);
      SequenceBegin (Settings.ArrayPrefix, Settings.ArrayFirstElementPrefix,
        Settings.ArrayOtherElementPrefix, Settings.ArrayElementPostfix, Settings.ArrayPostfix);
      var processor = new ToTextBuilderArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;
    }

    public ToTextBuilder array (Array array)
    {
      return AppendArray (array);
    }


    public ToTextBuilder AppendToText (Object obj)
    {
      BeforeAppendElement ();
      _AppendToText (obj);
      AfterAppendElement ();
      return this;
    }

    public ToTextBuilder tt (Object obj)
    {
      return AppendToText (obj);
    }

    public ToTextBuilder tt (Object obj, bool honorSequence)
    {
      return honorSequence ? AppendToText (obj) : AppendToTextNonSequence (obj);
    }



    public ToTextBuilder AppendToTextNonSequence (Object obj)
    {
      _AppendToText (obj);
      return this;
    }

    private ToTextBuilder _AppendToText (Object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;

    }


    public ToTextBuilder Append (string s)
    {
      return AppendString (s);
    }

    public ToTextBuilder Append (Object obj)
    {
      _disableableWriter.Write (obj);
      return this;
    }


    public ToTextBuilder ToTextString (string s)
    {
      return AppendString (s);
    }


 
    private ToTextBuilder AppendInstanceBegin (Type type)
    {
      SequenceBegin ("[" + type.Name, "  ", ",", "", "]");
      return this;
    }

    public ToTextBuilder beginInstance (Type type)
    {
      return AppendInstanceBegin (type);
    }


    private ToTextBuilder AppendInstanceEnd ()
    {
      SequenceEnd ();
      return this;
    }

    public ToTextBuilder endInstance ()
    {
      return AppendInstanceEnd ();
    }


    //--------------------------------------------------------------------------
    // High Level Sequence Emitters
    //--------------------------------------------------------------------------

    public ToTextBuilder AppendSequenceEnd ()
    {
      SequenceEnd ();
      AfterAppendElement ();
      return this;
    }

    private void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _disableableWriter.Write (SequenceState.SequencePostfix);

      SequenceState = _sequenceStack.Pop ();
    }

    public ToTextBuilder se ()
    {
      return AppendSequenceEnd ();
    }


    public ToTextBuilder AppendSequenceElement (object obj)
    {
      Assertion.IsTrue (IsInSequence);
      BeforeAppendElement ();
      _toTextProvider.ToText (obj, this);
      AfterAppendElement ();
      return this;
    }

    public ToTextBuilder e (object obj)
    {
      return AppendSequenceElement (obj);
    }

    public ToTextBuilder AppendSequenceElements (params object[] sequenceElements)
    {
      Assertion.IsTrue (IsInSequence);
      foreach (var obj in sequenceElements)
      {
        AppendSequenceElement (obj);
      }
      return this;
    }

    public ToTextBuilder elements (params object[] sequenceElements)
    {
      return AppendSequenceElements (sequenceElements);
    }


    public ToTextBuilder elementsNumbered (string s1, int i0, int i1)
    {
      for (int i = i0; i <= i1; ++i)
      {
        AppendSequenceElement (s1 + i);
      }
      return this;
    }


    //--------------------------------------------------------------------------
    // High Level Complexity Switching Emitters
    //--------------------------------------------------------------------------

    public ToTextBuilder cSkeleton
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Skeleton);
      }
    }

    public ToTextBuilder cBasic
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Basic);
      }
    }

    public ToTextBuilder cMedium
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Medium);
      }
    }

    public ToTextBuilder cComplex
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Complex);
      }
    }

    public ToTextBuilder cFull
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilderOutputComplexityLevel.Full);
      }
    }


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
    private static string RightUntilChar (string s, char separator)
    {
      int iSeparator = s.LastIndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (iSeparator + 1, s.Length - iSeparator - 1);
      }
      else
      {
        return s;
      }
    }


  }
}