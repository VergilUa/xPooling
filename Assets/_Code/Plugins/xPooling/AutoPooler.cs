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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pooling {
    /// <summary>
    /// Pooler that performs check (.IsAvailable) operations on it's controlled IGenericPoolElements and
    /// returns them to pool if they are available.
    ///
    /// Use this one if you don't want your objects to be in charge of .ReturnToPool calls, but rather one single
    /// manager.
    /// </summary>
    public class AutoPooler : GenericPooler {
        [Tooltip("Special case prefab for instantiating and mimicking AudioSources")]
        public GameObject AudioObjectPrefab;

        [Tooltip("Pool performs a check each X seconds and moves back. In seconds.")]
        public float MoveBackToPoolEachX = 5f;

        #region [Properties]

        private Transform _elementParent;

        #endregion

        #region [Fields]

        private float _timer;

        private readonly List<IGenericPoolElement> _cache = new List<IGenericPoolElement>();

        #endregion

        protected override void AfterAwake() {
            this.UseForExtensions();
        }

        private void FixedUpdate() {
            _timer += Time.fixedDeltaTime;

            if (_timer >= MoveBackToPoolEachX) {
                CheckAndReturn();
                _timer = 0;
            }
        }

        private void CheckAndReturn() {
            try {
                // Cannot directly decommission, that would cause modifications to the iterable collection
                foreach (GenericPool pool in PoolerLibrary.Values) {
                    foreach (IGenericPoolElement element in pool.InUseElements) {
                        if (element.IsAvailable) {
                            _cache.Add(element);
                        }
                    }
                }

                foreach (IGenericPoolElement element in _cache) {
                    element.Decommission();
                }

                _cache.Clear();
            } catch (Exception ex) {
                Debug.LogError($"AutoPooler({name}):: Internal object error thrown: {ex.Message}");
            }
        }

        protected override void OnUnityInstantiatePostProcess<T>(T obj) {
            // Mark as from AutoPool
            obj.UsesAutoPool = true;
        }
    }
}