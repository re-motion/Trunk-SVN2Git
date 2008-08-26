using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Remotion.Text.Diagnostic
{
  //TODO: NULL CHECKS!!!!!!!!!!!!!!!!!!!!!!!!
  /// <summary>
  /// Allows a class implementing the IProcessor interface to visit each member of an outer product of a variable number of independently sized tuples.
  /// From a programmer's view the class supplies "variable number of nested for loops"-functionality.
  /// For convenience derive you processor class from <see cref="OuterProduct.ProcessorBase"/> (see examples below).
  /// </summary>
  /// <include file='doc\include\Text\Diagnostic\OuterProduct.xml' path='OuterProduct/ClassExample1/*' />
  /// <include file='doc\include\Text\Diagnostic\OuterProduct.xml' path='OuterProduct/ClassExample2/*' />
  //TODO: rename. OuterProduct is only a result, sicne it is also a calculator, OuterProductBuilder is a better name
  public class OuterProduct 
  {
    /// <summary>
    /// Interface a "Processor" class which can be passed to OuterProduct.ProcessOuterProduct needs to implement.
    /// </summary>
    //TODO: Rename to strategy, document intention
    //TODO: move to outer scope, file
    //TODO: use ndoc-syntax for keywords, references
    public interface IProcessor {
      /// <summary>
      /// Processor callback invoked before a nested for loop starts.
      /// </summary>
      /// <returns><c>true</c> to continue looping, <c>false</c> to break from the current loop.</returns>
      //TODO: rename ProcessStateBeforeLoop?
      bool DoBeforeLoop ();
      /// <summary>
      /// Processor callback invoked after a nested for loop has finished.
      /// </summary>
      /// <returns><c>true</c> to continue looping, <c>false</c> to break from the current loop.</returns>
      //TODO: rename ProcessStateAfterLoop?
      bool DoAfterLoop ();
      /// <summary>
      /// Before each callback to the processor the OuterProduct class sets the current <c>ProcessingState</c> through a
      /// call to this method. The processor class is expected to store the <c>ProcessingState</c> to be able to access
      /// it during the callbacks.
      /// </summary>
      /// <param name="processingState"></param>
      void SetProcessingState (ProcessingState processingState);

      /// <summary>
      /// The current <c>ProcessingState</c> to be used during callbacks. Set by the OuterProduct class
      /// in call to <see cref="SetProcessingState"></see>.
      /// </summary>
      OuterProduct.ProcessingState ProcessingState { get; }
    }

    /// <summary>
    /// Convenience class to derive OuterProduct-processors from. Already supplies ProcessingState-functionality.
    /// </summary>
    //TODO: move to outer scope, file
    public class ProcessorBase : IProcessor 
    { 
      private OuterProduct.ProcessingState _processingState;
      /// <summary>
      /// The current <see cref="ProcessingState"/> to be used during callbacks.
      /// </summary>
      public OuterProduct.ProcessingState ProcessingState
      {
        get { return _processingState; }
      }

      /// <summary>
      /// Default implementation for the callback before a new for loop starts. Simply keeps on looping.
      /// Override to implement your own functionality.
      /// </summary>
      /// <returns><see cref="IProcessor.DoBeforeLoop"/></returns>
      public virtual bool DoBeforeLoop ()
      {
        return true;
      }

      /// <summary>
      /// Default implementation for the callback after a for loop finishes. Simply keeps on looping.
      /// Override to implement your own functionality.
      /// </summary>
      /// <returns><see cref="IProcessor.DoAfterLoop"/></returns>
      public virtual bool DoAfterLoop ()
      {
        return true;
      }
      
      /// <summary>
      /// Internal use only: Used by OuterProduct class to set the current <see cref="ProcessingState"/> before invoking a callback.
      /// </summary>
      /// <param name="processingState"></param>
      public void SetProcessingState (OuterProduct.ProcessingState processingState)
      {
        _processingState = processingState;
      } 
    }

    /// <summary>
    /// The current state of the outer product / nested for loops traversal.
    /// DimensionIndices supplies the current permutation of indices (array with an entry for each for-loop).
    /// DimensionIndex is the currently running for-loop; ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
    /// IsInnermostLoop, IsOutermostLoop can be queried to treat the innermost and outermost loop differently, if so required.
    /// </summary>
    //TODO: move to outer scope, file
    //TODO: remove empty xml-commmets
    public struct ProcessingState
    {
      /// <summary>
      /// Initializes a ProcessingState with an OuterProduct reference and the current dimension index 
      /// (= nested-for-loop loop-variable index).
      /// </summary>
      /// <param name="outerProduct"></param>
      /// <param name="dimensionIndex"></param>
      public ProcessingState (OuterProduct outerProduct, int dimensionIndex)
      {
        _outerProduct = outerProduct;
        _dimensionIndex = dimensionIndex;
      }

      private readonly OuterProduct _outerProduct;
      private readonly int _dimensionIndex;

      /// <summary>
      /// The outer product dimension which is currently processed (i.e. the index of the currently running for-loop).
      /// </summary>
      /// <value></value>
      public int DimensionIndex
      {
        get { return _dimensionIndex; }
      }

      /// <summary>
      /// Integer array containing the number of elements in each outer product dimension.
      /// </summary>
      /// <value></value>
      public int[] NumberElementsPerDimension
      {
        get { return _outerProduct.NumberElementsPerDimension; }
      }

      /// <summary>
      /// Integer array containing the current permutation of outer product indices (i.e. each array entry is the current value of each for-loop variable;
      /// <see cref="ElementIndex"/>).
      /// </summary>
      /// <value></value>
      public int[] DimensionIndices
      {
        get { return _outerProduct.DimensionIndices; }
      }

      /// <summary>
      /// The overall number of elements in the outer product.
      /// </summary>
      /// <value></value>
      public int NumberElementsOverall
      {
        get { return _outerProduct.NumberElementsOverall; }
      }

      /// <summary>
      /// ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
      /// </summary>
      /// <value></value>
      public int ElementIndex
      {
        get { return _outerProduct.DimensionIndices[_dimensionIndex]; }
      }

      /// <summary>
      /// Whether the element is the first element in the current for-loop.
      /// </summary>
      public bool IsFirstLoopElement
      {
        get { return ElementIndex == 0; }
      }

      /// <summary>
      /// Whether the element is the last element in the current for-loop.
      /// </summary>
      /// <value></value>
      public bool IsLastLoopElement
      {
        get { return ElementIndex == (NumberElementsPerDimension[DimensionIndex] - 1); }
      }

      /// <summary>
      /// Whether the current for-loop is the innermost loop.
      /// </summary>
      /// <value></value>
      public bool IsInnermostLoop
      {
        get { return DimensionIndex == (NumberElementsPerDimension.Length - 1); }
      }

      /// <summary>
      /// Whether the current for-loop is the outermost loop.
      /// </summary>
      /// <value></value>
      public bool IsOutermostLoop
      {
        get { return DimensionIndex == 0; }
      }

      /// <summary>
      /// Used internally by OuterProduct.ProcessOuterProduct. 
      /// </summary>
      /// <value>The element index to set.</value>
      public void SetCurrentElementIndex (int elementIndex)
      {
        DimensionIndices[_dimensionIndex] = elementIndex;
      }

      /// <summary>
      /// The overall elements of the outer product which have already been processed.
      /// </summary>
      /// <value></value>
      public int NumberElementsProcessed
      {
        get { return _outerProduct.NumberElementsProcessed; }
      }

      /// <summary>
      /// Returns a copy of the current <c>DimensionIndices</c>-array. 
      /// Use if you want to e.g. store the generated dimension indices permutations
      /// in your own collection.
      /// </summary>
      public int[] GetDimensionIndicesCopy ()
      {
        return (int[]) DimensionIndices.Clone();
      }
    }



    //TODO: remove these comments since it isn't needed when the class is shorter
    //-------------------------------------------------------------------------------
    // Class members
    //-------------------------------------------------------------------------------

    private int _numberElementsProcessed;
    private int[] _numberElementsPerDimension;
    private int _numberElementsOverall;
    private int[] _currentDimensionIndices;


    /// <summary>
    /// The total number of outer product elements that have been visited by the processor.
    /// </summary>
    public int NumberElementsProcessed
    {
      get { return _numberElementsProcessed; }
    }

    /// <summary>
    /// The number of elements in each outer product dimension.
    /// </summary>
    public int[] NumberElementsPerDimension
    {
      get { return _numberElementsPerDimension; }
      //TODO: remove unused setter
      private set { _numberElementsPerDimension = value; }
    }

    /// <summary>
    /// The total number of elements in the outer product (= the product of all NumberElementsPerDimension entries).
    /// </summary>
    public int NumberElementsOverall
    {
      get { return _numberElementsOverall; }
    }

    /// <summary>
    /// The dimension indices representing the current outer product permutation.
    /// </summary>
    public int[] DimensionIndices
    {
      get { return _currentDimensionIndices; }
    }

    /// <summary>
    /// The total number of combinations in the outer product.
    /// </summary>
    public int Length { get { return NumberElementsOverall; } }


    //TODO: remove these comments since it isn't needed when the class is shorter
    //TODO: ctor before property
    //-------------------------------------------------------------------------------
    // Ctors
    //-------------------------------------------------------------------------------

    ///<overloads>
    ///OuterProduct can be initialized in a general way by passing the number of elements
    ///along each dimension in an integer array, or specialized by passing a rectangular array whose
    ///dimensions shall be used by the outer product.
    ///</overloads>
    /// <summary>
    /// Initializes OuterProduct from an integer array, where each array entry gives the number of elements along its
    /// corresponding dimension. In programers terms: The number of times each nested for-loop will loop.
    /// </summary>
    /// <param name="numberElementsPerDimension">"Number of loops for each for"-loop array</param>
    public OuterProduct (int[] numberElementsPerDimension)
    {
      Init ((int[]) numberElementsPerDimension.Clone () );
    }

    /// <summary>
    /// Initializes OuterProduct from a (rectangular) array. Use to iterate over a rectangular array and access
    /// its members with <c>rectangularArray.GetValue (ProcessingState.DimensionIndices)</c> in the
    /// OuterProduct.IProcessor implementation.
    /// </summary>
    /// <param name="array">Array from whose dimensions the dimensions of the outer product will be initialized.</param>
    public OuterProduct (Array array)
    {
      Init (array);
    }

    //TODO: to static, use from ctor-cascading
    private void Init (Array array)
    {
      int numberDimensions = array.Rank;
      int[] numberElementsPerDimension = new int[numberDimensions];
      //TODO: rename
      for (int iDimension = 0; iDimension < numberDimensions; ++iDimension)
      {
        numberElementsPerDimension[iDimension] = array.GetLength (iDimension);
      }
      Init (numberElementsPerDimension);
    }

    private void Init (int[] numberElementsPerDimension)
    {
      _numberElementsPerDimension = numberElementsPerDimension;
      InitProcessing();
    }

    //TODO: inline
    private void InitProcessing ()
    {
      int rank = _numberElementsPerDimension.Length;
      _currentDimensionIndices = new int[rank];
      _numberElementsOverall = CalcOuterProductNrElementsOverall (_numberElementsPerDimension);
      _numberElementsProcessed = 0;
    }


    /// <summary>
    /// Calcs the number of elements in an outer product. 
    /// </summary>
    /// <param name="numberElementsPerDimension">The array giving the number of elements along each dimension of the outer product.</param>
    /// <returns>The product of the numbers in the passed array of integers.</returns>
    public static int CalcOuterProductNrElementsOverall (int[] numberElementsPerDimension)
    {
      if (numberElementsPerDimension.Length <= 0)
      {
        return 0;
      }
      else
      {
        int numberStateCombinations = 1;
        foreach (var numberElements in numberElementsPerDimension)
        {
          numberStateCombinations *= numberElements;
        }
        return numberStateCombinations;
      }
    }

 
    /// <summary>
    /// The recursive method which implements the variable number of for-loops together with processing callbacks to the outerProductProcessor.
    /// </summary>
    /// <param name="dimensionIndex"></param>
    /// <param name="outerProductProcessor"></param>
    private void ProcessOuterProductRecursive (int dimensionIndex, IProcessor outerProductProcessor)
    {
      if (dimensionIndex >= _numberElementsPerDimension.Length)
      {
        return;
      }

      var processingState = new ProcessingState (this, dimensionIndex);


      for (int iCurrentForLoop = 0; iCurrentForLoop < _numberElementsPerDimension[dimensionIndex]; ++iCurrentForLoop)
      {
        processingState.SetCurrentElementIndex (iCurrentForLoop);
        
        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingBeforeLoop = outerProductProcessor.DoBeforeLoop ();
        if (!continueProcessingBeforeLoop)
        {
          break;
        }

        ProcessOuterProductRecursive (dimensionIndex + 1, outerProductProcessor);

        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingAfterLoop = outerProductProcessor.DoAfterLoop ();
        if (!continueProcessingAfterLoop)
        {
          break;
        }

        ++_numberElementsProcessed;
      }
    }

    /// <summary>
    /// Call to start the processing of each OuterProduct-element.
    /// </summary>
    /// <param name="outerProductProcessor">An OuterProduct-processor which needs to implement the IProcessor interface.</param>
    public void ProcessOuterProduct (IProcessor outerProductProcessor)
    {
      //Init (_numberElementsPerDimension);
      //TODO: NO reset in type, builders are one way
      InitProcessing();
      ProcessOuterProductRecursive (0, outerProductProcessor);
    }


  }
}
