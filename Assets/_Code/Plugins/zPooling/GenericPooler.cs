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
   /// Class manager for all Generic Pooling procedures.
   /// </summary>
   public class GenericPooler : MonoBehaviour {
      #region [Fields]

      protected readonly Dictionary<GameObject, GenericPool> PoolerLibrary =
         new Dictionary<GameObject, GenericPool>();

      protected readonly Dictionary<int, GenericPool> HashReference = new Dictionary<int, GenericPool>();

      #endregion

      protected virtual void Awake() {
         AfterAwake();
      }

      protected virtual void AfterAwake() { this.UseForExtensions(); }

      #region [Pool instantiation methods]

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      private T TakeFromPool<T>(GameObject prefab) where T : MonoBehaviour, IGenericPoolElement {
         if (!PoolerLibrary.TryGetValue(prefab, out GenericPool manipulatedPool)) {
            manipulatedPool = InstantiatePool(prefab);
         }

         T returnedObject = ObtainElement<T>(manipulatedPool);
#if DEBUG
         if (returnedObject == null) {
            Debug.LogError("GenericPooler<"
                           + typeof(T)
                           + ">:: InstantiateFromPool() "
                           + "- Type mismatch! Cannot instantiate object: "
                           + prefab
                           + " even with unity instantiate!");
            return null;
         }
#endif

         return returnedObject;
      }

      // Multiple methods overloads are created to improve performance. It's not necessary, but nice to have
      // -- Vergil

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      public virtual T InstantiateFromPool<T>(GameObject prefab) where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      // Multiple methods overloads are created to improve performance. It's not necessary, but nice to have
      // -- Vergil

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="position">Position where it should be instantiated</param>
      public virtual T InstantiateFromPool<T>(GameObject prefab, Vector3 position)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);
         returnedObject.transform.position = position;

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;
         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="rotation">Rotation of instantiated transform</param>
      public virtual T InstantiateFromPool<T>(GameObject prefab, Quaternion rotation)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);
         returnedObject.transform.rotation = rotation;

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;
         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="position">Position where it should be instantiated</param>
      /// <param name="rotation">Rotation of instantiated transform</param>
      public virtual T InstantiateFromPool<T>(GameObject prefab, Vector3 position, Quaternion rotation)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);
         returnedObject.transform.SetPositionAndRotation(position, rotation);

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="parent">Optional parent object</param>
      /// <param name="worldPositionStays">Optional worldPositionStays for internal SetParent call</param>
      public virtual T InstantiateFromPool<T>(GameObject prefab, Transform parent, bool worldPositionStays = true)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);
         returnedObject.transform.SetParent(parent, worldPositionStays);

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="position">Position where it should be instantiated</param>
      /// <param name="rotation">Rotation of instantiated transform</param>
      /// <param name="parent">Optional parent object</param>
      /// <param name="worldPositionStays">Optional worldPositionStays for internal SetParent call</param>
      /// <returns>Instantiated object</returns>
      public virtual T InstantiateFromPool<T>(GameObject prefab,
                                              Vector3 position,
                                              Quaternion rotation,
                                              Transform parent,
                                              bool worldPositionStays = true)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);

         Transform trm = returnedObject.transform;
         trm.SetParent(parent, worldPositionStays);
         returnedObject.transform.SetPositionAndRotation(position, rotation);

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="position">Position where it should be instantiated</param>
      /// <param name="parent">Optional parent object</param>
      /// <param name="worldPositionStays">Optional worldPositionStays for internal SetParent call</param>
      /// <returns>Instantiated object</returns>
      public virtual T InstantiateFromPool<T>(GameObject prefab,
                                              Vector3 position,
                                              Transform parent,
                                              bool worldPositionStays = true)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);

         Transform trm = returnedObject.transform;
         trm.SetParent(parent, worldPositionStays);
         trm.position = position;

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      /// <summary>
      /// Instantiates object using pooling technique
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <param name="rotation">Rotation of instantiated transform</param>
      /// <param name="parent">Optional parent object</param>
      /// <param name="worldPositionStays">Optional worldPositionStays for internal SetParent call</param>
      /// <returns>Instantiated object</returns>
      public virtual T InstantiateFromPool<T>(GameObject prefab,
                                              Quaternion rotation,
                                              Transform parent,
                                              bool worldPositionStays = true)
         where T : MonoBehaviour, IGenericPoolElement {
         T returnedObject = TakeFromPool<T>(prefab);

         Transform trm = returnedObject.transform;
         trm.SetParent(parent, worldPositionStays);
         trm.rotation = rotation;

         returnedObject.Commission();
         returnedObject.IsCommissioned = true;

         return returnedObject;
      }

      #endregion

      /// <summary>
      /// Instantiates an actual pool object to contain data about the pool
      /// </summary>
      protected GenericPool InstantiatePool(GameObject prefab) {
         GenericPool pool = new GenericPool {Prefab = prefab};

         GameObject poolParentGO = new GameObject();
#if UNITY_EDITOR
         poolParentGO.name = prefab.name + " (Elements)";
#endif
         Transform poolParentTrm = poolParentGO.transform;
         poolParentTrm.SetParent(transform);
         pool.Parent = poolParentTrm;

         PoolerLibrary.Add(prefab, pool);
         HashReference.Add(pool.GetHashCode(), pool);
         return pool;
      }

      /// <summary>
      /// Obtains available element or instantiates a new one
      /// </summary>
      /// <param name="pool"></param>
      /// <returns></returns>
      protected virtual T ObtainElement<T>(GenericPool pool) where T : MonoBehaviour {
         if (pool.HasAvailableElements) {
            return pool.PoolFirst<T>();
         }

         // Instantiating a new one
         return UnityInstantiate<T>(pool);
      }

      /// <summary>
      /// This is how pool object is instantiated by default
      /// Override it if you want to add some changes to the instantiation procedure
      /// </summary>
      /// <returns>Instantiated object of prefab</returns>
      protected virtual T UnityInstantiate<T>(GenericPool pool) where T : MonoBehaviour {
         GameObject pooledObject = Instantiate(pool.Prefab, pool.Parent);
#if UNITY_EDITOR && DEBUG_ZPOOLING
         pooledObject.name = pool.Prefab.name + " " + pool.TotalSpawned;
         pool.TotalSpawned++;
#endif
         if (pooledObject != null) {
            // Adding to the pool as instance of generic pool element
            IGenericPoolElement element = pooledObject.GetComponent<IGenericPoolElement>()
                                          ?? pooledObject.GetComponentInChildren<IGenericPoolElement>();
            if (element != null) {
               pool.AddToPool(element);
               OnUnityInstantiatePostProcess(element);
            }
#if DEBUG
            else {
               Debug.LogError("Unable to find component ("
                              + typeof(T)
                              + ") on generic pool element instance - "
                              + pooledObject);
            }
#endif
            return pooledObject.GetComponent<T>();
         }

         return null;
      }

      /// <summary>
      /// Can be used for the inherited pools to add some custom logic once new instance of pooled object is generated
      /// </summary>
      protected virtual void OnUnityInstantiatePostProcess<T>(T obj) where T : IGenericPoolElement { }

      /// <summary>
      /// Sustains pool at specific object count. If it's bigger than set for the prefab - destroys the overhead,
      /// otherwise instantiates prefabs and decommissions them right away, for them to be usable in the future.
      /// </summary>
      public void SustainPool(GameObject prefab, int objCount, Vector3 prewarmPos, bool autoPool) {
         // Doesn't have a proper pool at all
         if (!PoolerLibrary.TryGetValue(prefab, out GenericPool manipulatedPool)) {
            InitializeObjects(prefab, objCount, prewarmPos, autoPool);
            return;
         }

         // Exactly what we need
         if (manipulatedPool.AvailableElementsCount == objCount) {
            return;
         }

         // Too many objects
         if (manipulatedPool.AvailableElementsCount > objCount) {
            manipulatedPool.TrimTo(objCount);
            return;
         }

         // Not enough objects
         InitializeObjects(prefab, objCount - manipulatedPool.AvailableElementsCount, prewarmPos, autoPool);
      }

      /// <summary>
      /// Spawns N of objects and decommissions them for future use
      /// </summary>
      private void InitializeObjects(GameObject prefab, int objCount, Vector3 prewarmPos, bool isAuto) {
         if (!PoolerLibrary.TryGetValue(prefab, out GenericPool manipulatedPool)) {
            manipulatedPool = InstantiatePool(prefab);
         }

         int hash = manipulatedPool.GetHashCode();
         for (int i = 0; i < objCount; i++) {
            GameObject instance = Instantiate(prefab);
            instance.SetActive(false);

            instance.transform.position = prewarmPos;

            IGenericPoolElement element = instance.GetComponent<IGenericPoolElement>();
            element.UsesAutoPool = isAuto;
            element.PoolRef = hash;
            element.ReturnToPool();
         }
      }

      /// <summary>
      /// Returns object back to the pool for it to be re-used
      /// </summary>
      public void ReturnToPool(IGenericPoolElement element) {
         HashReference.TryGetValue(element.PoolRef, out GenericPool genericPool);

#if DEBUG
         if (genericPool == null) {
            Debug.LogError($"GenericPooler Pool ReUse() call failed on {element.gameObject}. "
                           + "Make sure you're using correct poolHashRef to the correct pool (*.Instance mismatch?)",
                           element.gameObject);
         }
#endif

         Transform genericObjTrm = element.transform;
         Transform genericPoolParent = genericPool.Parent;
         genericObjTrm.SetParent(genericPoolParent);

         genericPool.ReturnToPool(element);
      }

      /// <summary>
      /// Returns object back to the pool for it to be re-used
      /// </summary>
      public virtual void RemoveFromPool(IGenericPoolElement genericObject) {
         HashReference.TryGetValue(genericObject.PoolRef, out GenericPool genericPool);

         genericPool?.RemoveFromPool(genericObject);
      }

      /// <summary>
      /// Decommissions all object back to the pool. Useful if cleanup is needed, e.g. when loading save state
      /// </summary>
      public void ForceDecommissionAll() {
         foreach (GenericPool pool in PoolerLibrary.Values) {
            pool.ForceDecommissionAll();
         }
      }
   }
}