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
using System.Text;
using Remotion.Utilities;

namespace Remotion.Diagnostic
{
  public class ToTextBuilder
  {
    //TODO: Move to outer scope
    public class SequenceStateHolder
    {
      private int _sequenceCounter;
      private string _sequencePrefix;
      private string _firstElementPrefix;
      private string _otherElementPrefix;
      private string _elementPostfix;
      private string _sequencePostfix;

      //TODO: Null-checks?
      public SequenceStateHolder (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
      {
        _sequenceCounter = 0;
        _sequencePrefix = sequencePrefix;
        _firstElementPrefix = firstElementPrefix;
        _otherElementPrefix = otherElementPrefix;
        _elementPostfix = elementPostfix;
        _sequencePostfix = sequencePostfix;
      }

      public string SequencePrefix
      {
        get { return _sequencePrefix; }
      }

      public int Counter
      {
        get { return _sequenceCounter; }
      }

      public string ElementPostfix
      {
        get { return _elementPostfix; }
      }

      public string OtherElementPrefix
      {
        get { return _otherElementPrefix; }
      }

      public string FirstElementPrefix
      {
        get { return _firstElementPrefix; }
      }

      public string SequencePostfix
      {
        get { return _sequencePostfix; }
      }

      //TODO: rename IncrementCounter, what is the purpose of this counter??
      public void IncreaseCounter ()
      {
        ++_sequenceCounter;
      }
    }


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

    //TODO: Document and rename, should be immutable?
    private class StringBuilderToText
    {
      private StringBuilder _stringBuilder = new StringBuilder ();

      public StringBuilderToText()
      {
        Enabled = true;
      }

      public bool Enabled { get; set; }

      public StringBuilder Append<T> (T t)
      {
        if (Enabled)
        {
          _stringBuilder.Append (t);
        }
        return _stringBuilder;
      }

      public override string ToString ()
      {
        return _stringBuilder.ToString ();
      }
    }

    private StringBuilderToText _textStringBuilderToText = new StringBuilderToText ();
    //TODO: make readonly
    private ToTextProvider _toTextProvider;
    
    private string _enumerablePrefix = "{";
    private string _enumerableFirstElementPrefix = "";
    private string _enumerableOtherElementPrefix = ",";
    private string _enumerableElementPostfix = "";
    private string _enumerablePostfix = "}";

    private string _arrayPrefix = "{";
    private string _arrayFirstElementPrefix = "";
    private string _arrayOtherElementPrefix = ",";
    private string _arrayElementPostfix = "";
    private string _arrayPostfix = "}";
    private bool _useMultiline = true;
    private OutputComplexityLevel _outputComplexity = OutputComplexityLevel.Basic;
    private Stack<SequenceStateHolder> _sequenceStack = new Stack<SequenceStateHolder>(16);
    private SequenceStateHolder _sequenceState = null;

    //TODO: Are setters really necessary?, Move to parameter object and keep them there
    public string ArrayPrefix
    {
      get { return _arrayPrefix; }
      set { _arrayPrefix = value; }
    }

    public string ArrayFirstElementPrefix
    {
      get { return _arrayFirstElementPrefix; }
      set { _arrayFirstElementPrefix = value; }
    }

    public string ArrayOtherElementPrefix
    {
      get { return _arrayOtherElementPrefix; }
      set { _arrayOtherElementPrefix = value; }
    }

    public string ArrayElementPostfix
    {
      get { return _arrayElementPostfix; }
      set { _arrayElementPostfix = value; }
    }

    public string ArrayPostfix
    {
      get { return _arrayPostfix; }
      set { _arrayPostfix = value; }
    }


    public string EnumerablePrefix
    {
      get { return _enumerablePrefix; }
      set { _enumerablePrefix = value; }
    }

    public string EnumerableFirstElementPrefix
    {
      get { return _enumerableFirstElementPrefix; }
      set { _enumerableFirstElementPrefix = value; }
    }

    public string EnumerableOtherElementPrefix
    {
      get { return _enumerableOtherElementPrefix; }
      set { _enumerableOtherElementPrefix = value; }
    }

    public string EnumerableElementPostfix
    {
      get { return _enumerableElementPostfix; }
      set { _enumerableElementPostfix = value; }
    }

    public string EnumerablePostfix
    {
      get { return _enumerablePostfix; }
      set { _enumerablePostfix = value; }
    }

    //TODO: move to outer scope
    public enum OutputComplexityLevel
    {
      Disable,
      Skeleton,
      Basic,
      Medium,
      Complex,
      Full,
    };

    public SequenceStateHolder SequenceState
    {
      get { return _sequenceState; }
    }

    public OutputComplexityLevel OutputComplexity
    {
      get { return _outputComplexity; }
    }

    public void OutputDisable () { _outputComplexity = OutputComplexityLevel.Disable; }
    public void OutputSkeleton () { _outputComplexity = OutputComplexityLevel.Skeleton; }
    public void OutputBasic () { _outputComplexity = OutputComplexityLevel.Basic; }
    public void OutputMedium () { _outputComplexity = OutputComplexityLevel.Medium; }
    public void OutputComplex () { _outputComplexity = OutputComplexityLevel.Complex; }
    public void OutputFull () { _outputComplexity = OutputComplexityLevel.Full; }

    //TODO: rename, make factory method, don't toggle flag
    public ToTextBuilder AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo(OutputComplexityLevel complexityLevel)
    {
      _textStringBuilderToText.Enabled = (_outputComplexity >= complexityLevel) ? true : false;
      return this; 
    }

    //TODO: rename
    public ToTextBuilder cSkeleton
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Skeleton);
      }
    }

    public ToTextBuilder cBasic
    {
      get {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Basic);
      }
    }

    public ToTextBuilder cMedium
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Medium);
      }
    }

    public ToTextBuilder cComplex
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Complex);
      }
    }

    public ToTextBuilder cFull
    {
      get
      {
        return AppendTheFollowingIfComplexityLevelIsGreaterThanOrEqualTo (ToTextBuilder.OutputComplexityLevel.Full);
      }
    }

    public ToTextBuilder(ToTextProvider toTextProvider)
    {
      _toTextProvider = toTextProvider;
    }


    public bool IsInSequence
    {
      //get { return _sequenceStack.Count > 0; }
      get { return _sequenceState != null; }
      //get { return true; }
    }

    public ToTextProvider ToTextProvider
    {
      get { return _toTextProvider; }
      set { _toTextProvider = value; }
    }



    public ToTextBuilder AppendString (string s)
    {
      //BeforeAppendElement ();
      _textStringBuilderToText.Append (s);
      //AfterAppendElement();
      return this;
    }

    public ToTextBuilder AppendNewLine ()
    {
      if (_useMultiline)
      {
        _textStringBuilderToText.Append (System.Environment.NewLine);
      }
      return this;
    }

    public ToTextBuilder AppendSpace ()
    {
      _textStringBuilderToText.Append (" ");
      return this;
    }

    public ToTextBuilder AppendTabulator ()
    {
      _textStringBuilderToText.Append ("\t");
      return this;
    }

    public ToTextBuilder AppendSeperator ()
    {
      _textStringBuilderToText.Append (",");
      return this;
    }

    public ToTextBuilder AppendComma ()
    {
      _textStringBuilderToText.Append (",");
      return this;
    }

    public ToTextBuilder AppendColon ()
    {
      _textStringBuilderToText.Append (":");
      return this;
    }

    public ToTextBuilder AppendSemiColon ()
    {
      _textStringBuilderToText.Append (";");
      return this;
    }


    private ToTextBuilder AppendObjectToString (object obj)
    {
      _textStringBuilderToText.Append (obj.ToString());
      return this;
    }

    //TODO: arg check for name
    public ToTextBuilder AppendMember (string name, Object obj)
    {
      BeforeAppendElement ();
      _AppendMember (name, obj);
      AfterAppendElement ();
      return this;
    }

    //TODO: arg check for name
    public ToTextBuilder AppendMemberNonSequence (string name, Object obj)
    {
      _AppendMember (name, obj);
      return this;
    }

    //TODO: rename
    private ToTextBuilder _AppendMember (string name, Object obj)
    {
      //_textStringBuilderToText.Append (" ");
      //_textStringBuilderToText.Append (name);
      //_textStringBuilderToText.Append ("=");
      //_toTextProvider.ToText (obj, this);
      //_textStringBuilderToText.Append (" ");

      //_textStringBuilderToText.Append (name);
      //_textStringBuilderToText.Append ("=");
      //_toTextProvider.ToText (obj, this);

      SequenceBegin (name+ "=", "", "", "", "");
      //_textStringBuilderToText. (name);
      //_textStringBuilderToText.Append ("=");
      _toTextProvider.ToText (obj, this);
      SequenceEnd();

      return this;
    }


    public string EnumerableBegin
    {
      get { return _enumerablePrefix; }
      set { _enumerablePrefix = value; }
    }

    public string EnumerableSeparator
    {
      get { return _enumerableOtherElementPrefix; }
      set { _enumerableOtherElementPrefix = value; }
    }

    public string EnumerableEnd
    {
      get { return _enumerablePostfix; }
      set { _enumerablePostfix = value; }
    }


    public string ArrayBegin
    {
      get { return _arrayPrefix; }
      set { _arrayPrefix = value; }
    }

    public string ArraySeparator
    {
      get { return _arrayOtherElementPrefix; }
      set { _arrayOtherElementPrefix = value; }
    }

    public string ArrayEnd
    {
      get { return _arrayPostfix; }
      set { _arrayPostfix = value; }
    }


    public ToTextBuilder AppendEnumerable (IEnumerable collection)
    {
      SequenceBegin (_enumerablePrefix, _enumerableFirstElementPrefix, _enumerableOtherElementPrefix, _enumerableElementPostfix, _enumerablePostfix);
      foreach (Object element in collection)
      {
        AppendToText (element);
      }
      SequenceEnd ();
      return this;
    }



    //private class ArrayToTextProcessor : OuterProduct.ProcessorBase
    //{
    //  protected readonly Array _array;
    //  private readonly ToTextBuilder _toTextBuilder;

    //  public ArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
    //  {
    //    _array = rectangularArray;
    //    _toTextBuilder = toTextBuilder;
    //  }

    //  public override bool DoBeforeLoop ()
    //  {
    //    InsertSeperator ();

    //    if (ProcessingState.IsInnermostLoop)
    //    {
    //      _toTextBuilder.ToText (_array.GetValue (ProcessingState.DimensionIndices));
    //    }
    //    else
    //    {
    //      _toTextBuilder.AppendString (_toTextBuilder.ArrayBegin);
    //    }
    //    return true;
    //  }

    //  public override bool DoAfterLoop ()
    //  {
    //    if (!ProcessingState.IsInnermostLoop)
    //    {
    //      _toTextBuilder.AppendString (_toTextBuilder.ArrayEnd);
    //    }
    //    return true;
    //  }

    //  protected void InsertSeperator ()
    //  {
    //    if (!ProcessingState.IsFirstLoopElement)
    //    {
    //      _toTextBuilder.AppendString (_toTextBuilder.ArraySeparator);
    //    }
    //  }
    //}


    //TODO: move to outer scope
    private class ArrayToTextProcessor : OuterProduct.ProcessorBase
    {
      protected readonly Array _array;
      private readonly ToTextBuilder _toTextBuilder;

      public ArrayToTextProcessor (Array rectangularArray, ToTextBuilder toTextBuilder)
      {
        _array = rectangularArray;
        _toTextBuilder = toTextBuilder;
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          _toTextBuilder.AppendToText (_array.GetValue (ProcessingState.DimensionIndices));
        }
        else
        {
          _toTextBuilder.AppendSequenceBegin (_toTextBuilder._arrayPrefix, _toTextBuilder._arrayFirstElementPrefix, _toTextBuilder._arrayOtherElementPrefix, _toTextBuilder._arrayElementPostfix, _toTextBuilder._arrayPostfix);
        }
        return true;
      }



      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
        {
          _toTextBuilder.AppendSequenceEnd ();
        }
        return true;
      }
    }


    public ToTextBuilder AppendArray (Array array)
    {
      var outerProduct = new OuterProduct (array);
      SequenceBegin (_arrayPrefix, _arrayFirstElementPrefix, _arrayOtherElementPrefix, _arrayElementPostfix, _arrayPostfix);
      var processor = new ArrayToTextProcessor (array, this);
      outerProduct.ProcessOuterProduct (processor);
      SequenceEnd ();

      return this;
    }



    public ToTextBuilder AppendToText (Object obj)
    {
      BeforeAppendElement ();
      _AppendToText (obj);
      AfterAppendElement ();
      return this;
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
      _textStringBuilderToText.Append(obj);
      return this;
    }


    public ToTextBuilder ToTextString (string s)
    {
      return AppendString (s);
    }


    public bool UseMultiLine
    {
      get { return _useMultiline; }
      set { _useMultiline = value; }
    }

    public bool Enabled
    {
      get { return _textStringBuilderToText.Enabled;  } 
      set { _textStringBuilderToText.Enabled = value;  } 
    }


    //TODO: Capitalize method and property names, no abbreviations

    //--------------------------------------------------------------------------
    // Shorthand notations
    //--------------------------------------------------------------------------

    public ToTextBuilder sf (string format, params object[] paramArray)
    {
      return AppendString (string.Format (format, paramArray));
    }


    public ToTextBuilder s (string s)
    {
      return AppendString (s);
    }

    public ToTextBuilder s ()
    {
      return this;
    }

    public ToTextBuilder nil ()
    {
      return this;
    }

    public ToTextBuilder nl 
    {
      get { AppendNewLine (); return this; }
    }

    public ToTextBuilder space
    {
      get { AppendSpace (); return this; }
    }

    public ToTextBuilder tab
    {
      get { AppendTabulator (); return this; }
    }

    public ToTextBuilder seperator
    {
      get { AppendSeperator (); return this; }
    }

    public ToTextBuilder comma
    {
      get { AppendComma (); return this; }
    }

    public ToTextBuilder colon
    {
      get { AppendColon (); return this; }
    }

    public ToTextBuilder semicolon
    {
      get { AppendSemiColon (); return this; }
    }


    public ToTextBuilder tt (Object obj)
    {
      return AppendToText (obj);
    }

    public ToTextBuilder tt ( Object obj, bool honorSequence)
    {
      return honorSequence ? AppendToText (obj) : AppendToTextNonSequence (obj);
    }


    public ToTextBuilder m (Object obj)
    {
      return AppendToText (obj);
    }

    public ToTextBuilder m(string name, Object obj, bool honorSequence)
    {
      return honorSequence ? AppendMember (name, obj) : AppendMemberNonSequence (name, obj);
    }

    public ToTextBuilder m (string name, Object obj)
    {
      return AppendMember (name, obj);
    }


    public ToTextBuilder ts (object obj)
    {
      return AppendObjectToString (obj);
    }

    public ToTextBuilder collection (IEnumerable collection)
    {
      return AppendEnumerable (collection);
    }


    public ToTextBuilder array (Array array)
    {
      return AppendArray (array);
    }



    public override string ToString ()
    {
      //TODO: No exception at all, ToString should never throw
      Assertion.IsFalse(IsInSequence);
      return _textStringBuilderToText.ToString();
    }



    public ToTextBuilder ToText (object obj)
    {
      _toTextProvider.ToText (obj, this);
      return this;
    }


    private ToTextBuilder AppendInstanceBegin (Type type)
    {
      //this.AppendString("[");
      //this.AppendString (type.Name);
      //this.AppendString ("  ");
      //SequenceBegin ("[" + type.Name + "  ", "", ",", "", "]");
      SequenceBegin ("[" + type.Name, "  ", ",", "", "]");
      return this;
    }

    public ToTextBuilder beginInstance (Type type)
    {
      return AppendInstanceBegin(type);
    }


    private ToTextBuilder AppendInstanceEnd ()
    {
      //this.AppendString ("]");
      SequenceEnd();
      return this;
    }

    public ToTextBuilder endInstance ()
    {
      return AppendInstanceEnd ();
    }


    private void BeforeAppendElement ()
    {
      if (IsInSequence)
      {
        _textStringBuilderToText.Append (SequenceState.Counter == 0 ? SequenceState.FirstElementPrefix : SequenceState.OtherElementPrefix);
      }
    }

    private void AfterAppendElement ()
    {
      if (IsInSequence)
      {
        _textStringBuilderToText.Append (SequenceState.ElementPostfix);
        SequenceState.IncreaseCounter ();
      }
    }


    public ToTextBuilder AppendSequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      BeforeAppendElement();

      return SequenceBegin(sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
    }

    private ToTextBuilder SequenceBegin (string sequencePrefix, string firstElementPrefix, string otherElementPrefix, string elementPostfix, string sequencePostfix)
    {
      //_sequenceStack.Push (new SequenceStateHolder(sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix));
      _sequenceStack.Push (_sequenceState);
      _sequenceState = new SequenceStateHolder(sequencePrefix, firstElementPrefix, otherElementPrefix, elementPostfix, sequencePostfix);
      
      _textStringBuilderToText.Append (SequenceState.SequencePrefix);

      return this;
    }


    public ToTextBuilder AppendSequenceEnd ()
    {
      SequenceEnd();

      AfterAppendElement();
      return this;
    }

    private void SequenceEnd ()
    {
      Assertion.IsTrue (IsInSequence);
      _textStringBuilderToText.Append (SequenceState.SequencePostfix);

      //_sequenceStack.Pop ();
      _sequenceState = _sequenceStack.Pop ();
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

    public ToTextBuilder se ()
    {
      return AppendSequenceEnd ();
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
      return AppendSequenceElements(sequenceElements);
    }


    public ToTextBuilder elementsNumbered (string s1, int i0, int i1)
    {
      for (int i = i0; i <= i1; ++i)
      {
        AppendSequenceElement (s1 + i);
      }
      return this;
    }
  }
}