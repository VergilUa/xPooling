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
using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
   /// <summary>
   /// Generic pool for base pool operations
   /// </summary>
   public class GenericPool {
      public GameObject Prefab;

      #region [Properties]

      public bool HasAvailableElements => AvailableElements.Count > 0;
      public int AvailableElementsCount => AvailableElements.Count;
      public bool HasInUseElements => InUseElements.Count > 0;
      
#if UNITY_EDITOR && DEBUG_ZPOOLING
      public int TotalSpawned; // Keep incremental number of spawned entities for tracking names
#endif

      #endregion

      #region [Fields]

      public readonly HashSet<IGenericPoolElement> InUseElements = new HashSet<IGenericPoolElement>();
      public readonly HashSet<IGenericPoolElement> AvailableElements = new HashSet<IGenericPoolElement>();
      private readonly List<IGenericPoolElement> _elementBuffer = new List<IGenericPoolElement>();

      #endregion

      /// <summary>
      /// Activates first available elements and updates available collection
      /// </summary>
      /// <returns>Activated component</returns>
      public T PoolFirst<T>() where T : MonoBehaviour {
         IGenericPoolElement element = null;

         // Get first, .First Allocates 32b
         foreach (IGenericPoolElement el in AvailableElements) {
            element = el;
            break;
         }
         
         AvailableElements.Remove(element);
         InUseElements.Add(element);

         return element as T;
      }

      /// <summary>
      /// Destroys overhead of the objects to specific value 
      /// </summary>
      public virtual void TrimTo(int objCount) {
         if (objCount <= 0) {
#if DEBUG
            Debug.LogError("GenericPool:: Invalid object count to trim " + objCount);
#endif
            return;
         }

         _elementBuffer.Clear();
         foreach (IGenericPoolElement element in AvailableElements) {
            _elementBuffer.Add(element);
         }

         for (int i = objCount - 1; i < _elementBuffer.Count; i++) {
            IGenericPoolElement element = _elementBuffer[i];

            InUseElements.Remove(element);
            AvailableElements.Remove(element);
            Object.Destroy(element.gameObject);
         }
      }

      public void AddToPool(IGenericPoolElement poolElement) {
         poolElement.PoolRef = GetHashCode();
         InUseElements.Add(poolElement);
      }

      public void ReturnToPool(IGenericPoolElement poolElement) {
         Debug.Assert(!AvailableElements.Contains(poolElement),
                      "GenericPool:: This element is already in the pool. "
                      + "This may cause some bugs, consider not decommissioning twice!");
         
         AvailableElements.Add(poolElement);
         InUseElements.Remove(poolElement);
         
         poolElement.IsCommissioned = false;
      }

      public void RemoveFromPool(IGenericPoolElement element) {
         AvailableElements.Remove(element);
         InUseElements.Remove(element);
      }

      /// <summary>
      /// Forces all IGenericPoolElements in this pool to decommission themselves back to the pool
      /// </summary>
      public void ForceDecommissionAll() {
         _elementBuffer.Clear();
         _elementBuffer.AddRange(InUseElements);

         foreach (IGenericPoolElement element in _elementBuffer) {
            element.Decommission();
         }

         InUseElements.Clear();
      }
      
      public int GetTotalObjectCount() {
         return AvailableElements.Count + InUseElements.Count;
      }
   }
}