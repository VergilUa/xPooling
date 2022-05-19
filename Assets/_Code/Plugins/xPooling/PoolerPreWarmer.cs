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
   /// Behaviour to prepare objects for pooling, and also sustain pool at certain numbers of objects on scene
   /// transition
   /// </summary>
   public class PoolerPreWarmer : MonoBehaviour {
      /// <summary>
      /// Position at which objects are initialized
      /// </summary>
      [Tooltip("Position at which objects are initialized")]
      public Vector3 InitializeAtPos = new Vector3(2000f, 2000f, 2000f);

      [Header("Prewarm Data")]
      [SerializeField]
      private List<PoolData> _cookObjects = new List<PoolData>();

      #region [Fields]

      private readonly Dictionary<GameObject, PoolData> _lookup = new Dictionary<GameObject, PoolData>();

      #endregion

      private void Awake() {
         this.UseForExtensions();
         
         InitLookup();
      }

      private void InitLookup() {
         foreach (PoolData poolData in _cookObjects) {
            GameObject seekPrefab = poolData.Prefab;

            if (_lookup.ContainsKey(seekPrefab)) {
#if DEBUG
               Debug.LogWarning("PoolerPreWarmer:: Duplicate of prefab "
                                + seekPrefab
                                + ". Only first one will be used");
#endif
               continue;
            }

            _lookup.Add(seekPrefab, poolData);
         }
      }

      /// <summary>
      /// Adds a prewarm entry to the list and also performs prewarming of passed data 
      /// </summary>
      /// <param name="poolData"></param>
      public void AddPrewarmEntry(PoolData poolData) {
         Prewarm(poolData);

         GameObject seekPrefab = poolData.Prefab;

         // Replace existing data with new one
         if (_lookup.ContainsKey(seekPrefab)) {
            FindAndReplaceInList(poolData);
            _lookup[seekPrefab] = poolData;
         } else {
            _lookup.Add(seekPrefab, poolData);
         }
      }

      private void Prewarm(PoolData data) {
         switch (data.PoolType) {
            case PoolType.GenericPooler:
               data.Prefab.SustainPool(data.NumberOfObjects, InitializeAtPos);
               break;
            case PoolType.AutoPooler:
               data.Prefab.SustainAutoPool(data.NumberOfObjects, InitializeAtPos);
               break;
         }
      }

      private void FindAndReplaceInList(PoolData data) {
         GameObject seekPrefab = data.Prefab;

         for (int i = 0; i < _cookObjects.Count; i++) {
            PoolData pd = _cookObjects[i];

            if (pd.Prefab == seekPrefab) {
               _cookObjects[i] = data;
               return;
            }
         }
      }
   }

   [System.Serializable]
   public struct PoolData {
      public GameObject Prefab;
      public int NumberOfObjects;
      public PoolType PoolType;
   }

   [System.Serializable]
   public enum PoolType {
      None = 0,
      GenericPooler = 1,
      AutoPooler = 2,
   }
}