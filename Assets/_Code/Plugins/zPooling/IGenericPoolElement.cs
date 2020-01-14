/*
   Copyright (c) 2017 Anton Rudenok

   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
*/
using UnityEngine;

namespace Pooling {
   /// <summary>
   /// Generic pool element interface
   /// </summary>
   public interface IGenericPoolElement {
      /// <summary>
      /// Sets pool reference for the spawned object, so it could return to the pool via ReUse() call
      /// </summary>
      /// <value></value>
      int PoolRef { get; set; }

      /// <summary>
      /// Used by continuous check poolers, those that need do to something with the pooled element. Not every
      /// IGenericElement can have this one
      /// </summary>
      /// <value></value>
      bool IsAvailable { get; }

      /// <summary>
      /// Automatically set via the pool. Can be used for checking against if the element is set
      /// </summary>
      bool IsCommissioned { get; set; }

      /// <summary>
      /// Used internally by the pooling to determine the object should be returned.
      /// Do not modify please
      /// </summary>
      bool UsesAutoPool { get; set; }

      /// <summary>
      /// Called when element in the pool gets re-activated back
      /// Perform gameObject.SetActive() here, or something similar
      /// </summary>
      /// <returns></returns>
      void Commission();

      /// <summary>
      /// Called via AutoPooler automatically, on GenericPooler -> Call it when you're done with the object
      /// Also, you should do gameObject.SetActive(false) or something similar in here
      ///
      /// When done, call this.ReturnToPool for GenericPooler elements or this.ReturnToAutoPool for AutoPooler elements
      /// </summary>
      void Decommission();

      /// <summary>
      /// Ensures that destroy is covered as well. Call .RemoveFromPool if any object is destroyed, as a fail-safe
      /// mechanism
      /// </summary>
      void OnDestroy();

      // ReSharper disable once InconsistentNaming -> Unity's Naming
      GameObject gameObject { get; }

      // ReSharper disable once InconsistentNaming -> Unity's Naming
      Transform transform { get; }
   }
}