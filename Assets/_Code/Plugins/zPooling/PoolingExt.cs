using UnityEngine;

namespace Pooling {
   /// <summary>
   /// Extension methods for pooling usage.
   /// Make sure to use at least once .Cache method to cache reference to the AutoPooler and GenericPooler 
   /// </summary>
   public static class PoolingExt {
      #region [Properties]

      public static AutoPooler AutoPooler {
         get
         {
            Debug.Assert(_autoPooler != null,
                         "PoolingExt:: AutoPooler not set. Make sure you have AutoPooler in the scene");
            return _autoPooler;
         }
      }

      private static AutoPooler _autoPooler;

      private static GenericPooler GenericPooler {
         get
         {
            Debug.Assert(_genericPooler != null,
                         "PoolingExt:: GenericPooler not set. Make sure you have GenericPooler in the scene");
            return _genericPooler;
         }
      }

      private static GenericPooler _genericPooler;

      private static PoolerPreWarmer PoolPreWarmer {
         get
         {
            Debug.Assert(_poolPreWarmer != null,
                         "PoolingExt:: PoolPreWarmer not set. Make sure you have PoolPreWarmer in the scene");
            return _poolPreWarmer;
         }
      }

      private static PoolerPreWarmer _poolPreWarmer;

      #endregion

      /// <summary>
      /// Sets this generic pooler to be used for extension calls
      /// </summary>
      /// <param name="pooler"></param>
      public static void UseForExtensions(this GenericPooler pooler) { _genericPooler = pooler; }

      /// <summary>
      /// Sets this auto pooler to be used for extension calls
      /// </summary>
      /// <param name="autoPooler"></param>
      public static void UseForExtensions(this AutoPooler autoPooler) { _autoPooler = autoPooler; }

      /// <summary>
      /// Sets this pooler prewarmer to be used for extension calls
      /// </summary>
      /// <param name="poolerPreWarmer"></param>
      public static void UseForExtensions(this PoolerPreWarmer poolerPreWarmer) { _poolPreWarmer = poolerPreWarmer; }

      #region [Generic Pooler]

      /// <summary>
      /// Performs pooled instantiation on a specific prefab, and returns requested T MonoBehaviour from it
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <returns>Instantiated object</returns>
      public static T Pool<T>(this GameObject prefab) where T : MonoBehaviour, IGenericPoolElement {
         return GenericPooler.InstantiateFromPool<T>(prefab);
      }

      #endregion

      #region [AutoPooler]

      /// <summary>
      /// Performs pooled instantiation on a specific prefab, and returns requested T MonoBehaviour from it
      /// This one passes to AutoPooler instead (which automatically checks if object is .IsAvaiable)
      /// </summary>
      /// <param name="prefab">GameObject prefab</param>
      /// <returns>Instantiated object</returns>
      public static T AutoPool<T>(this GameObject prefab) where T : MonoBehaviour, IGenericPoolElement {
         return AutoPooler.InstantiateFromPool<T>(prefab);
      }

      /// <summary>
      /// Prewarms specific prefab, sustaining specific object count for GenericPooler
      /// This operation is costly 
      /// </summary>
      /// <param name="prefab"></param>
      /// <param name="objCount"></param>
      /// <param name="preWarmPos"></param>
      public static void SustainPool(this GameObject prefab, int objCount, Vector3 preWarmPos) {
         GenericPooler.SustainPool(prefab, objCount, preWarmPos, false);
      }

      /// <summary>
      /// Prewarms specific prefab, sustaining specific object count for AutoPooler
      /// This operation is costly 
      /// </summary>
      /// <param name="prefab"></param>
      /// <param name="objCount"></param>
      /// <param name="preWarmPos"></param>
      public static void SustainAutoPool(this GameObject prefab, int objCount, Vector3 preWarmPos) {
         AutoPooler.SustainPool(prefab, objCount, preWarmPos, true);
      }

      /// <summary>
      /// Adds a prewarm data for the PoolerPreWarmer to the AutoPooler
      /// Use this if you want to set some pool data entry for pool control
      /// </summary>
      /// <param name="prefab"></param>
      /// <param name="objCount"></param>
      public static void AddEntryAuto(this GameObject prefab, int objCount) {
         PoolPreWarmer.AddPrewarmEntry(new PoolData {
                                                       Prefab = prefab,
                                                       NumberOfObjects = objCount,
                                                       PoolType = PoolType.AutoPooler
                                                    });
      }

      #endregion

      #region [Generic]

      /// <summary>
      /// Adds a prewarm data for the PoolerPreWarmer to the GenericPooler
      /// Use this if you want to set some pool data entry for pool control
      /// </summary>
      /// <param name="prefab"></param>
      /// <param name="objCount"></param>
      public static void AddEntryGeneric(this GameObject prefab, int objCount) {
         PoolPreWarmer.AddPrewarmEntry(new PoolData {
                                                       Prefab = prefab,
                                                       NumberOfObjects = objCount,
                                                       PoolType = PoolType.GenericPooler
                                                    });
      }

      /// <summary>
      /// Returns IGenericPoolElement back to the respective GenericPooler
      /// </summary>
      public static void ReturnToPool(this IGenericPoolElement element) {
         Debug.Assert(element != null, "Element is null");

         if (element.UsesAutoPool) {
            AutoPooler.ReturnToPool(element);
         } else {
            GenericPooler.ReturnToPool(element);
         }
      }

      /// <summary>
      /// Removes element from the pool completely.
      /// Use this if .Destroy needed to be called on the pooled objects that's being destroyed
      ///
      /// Automatically detects which pool to use
      /// </summary>
      /// <param name="element"></param>
      public static void RemoveFromPool(this IGenericPoolElement element) {
         Debug.Assert(element != null, "Element is null");

         if (element.UsesAutoPool) {
            if (_autoPooler != null) _autoPooler.RemoveFromPool(element);
         } else {
            if (_genericPooler != null) _genericPooler.RemoveFromPool(element);
         }
      }

      #endregion
   }
}