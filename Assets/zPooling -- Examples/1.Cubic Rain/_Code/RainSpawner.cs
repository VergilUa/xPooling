using Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Script used to spawn RainElement prefabs at specific time rate using pooling
/// </summary>
public class RainSpawner : MonoBehaviour {
   [SerializeField]
   private float _spawnDelay = 3f;

   [SerializeField]
   private int _elementsPerSpawn = 4;

   [SerializeField]
   private Transform _spawnBoundsTrm;

   [SerializeField]
   private Vector3 _spawnCenter;

   [SerializeField]
   private Vector3 _spawnExtents;

   [Header("Prefab")]
   [SerializeField]
   private GameObject _rainElementPrefab;

   #region [Properties]

   private Vector3 RandomSpawnPos {
      get
      {
         Vector3 rndOffset = new Vector3(Random.Range(-_spawnExtents.x, _spawnExtents.x),
                                         Random.Range(-_spawnExtents.y, _spawnExtents.y),
                                         Random.Range(-_spawnExtents.z, _spawnExtents.z));
         return _spawnCenter + rndOffset;
      }
   }

   #endregion

   #region [Fields]

   private float _spawnTimer;

   #endregion

   private void FixedUpdate() {
      _spawnTimer -= Time.deltaTime;

      if (_spawnTimer > 0) return;
      
      for (int i = 0; i < _elementsPerSpawn; i++) {
         DoSpawn();
      }

      _spawnTimer = _spawnDelay;
   }

   private void DoSpawn() {
      // Use .Pool instead of Instantiate on the prefab
      // if extension is missing don't forget to include "using Pooling"

      // element can be further be used like so
      RainElement element = _rainElementPrefab.Pool<RainElement>();
      element.SetPosition(RandomSpawnPos);

      // .Pool can also receive lots of other parameters, check PoolingExt.cs for more examples, e.g.
      // _rainElementPrefab.Pool<RainElement>(spawnPos, spawnRot);
   }

#if UNITY_EDITOR
   protected void OnDrawGizmos() {
      if (_spawnBoundsTrm == null) return;

      Gizmos.color = Color.cyan;
      Gizmos.DrawWireCube(_spawnBoundsTrm.position, _spawnBoundsTrm.localScale);
   }

   protected virtual void OnValidate() {
      if (_spawnBoundsTrm == null) _spawnBoundsTrm = transform;
      if (_spawnDelay < 0.0001f) _spawnDelay = 0.0001f;

      if (_spawnBoundsTrm != null) {
         _spawnCenter = _spawnBoundsTrm.position;
         _spawnExtents = _spawnBoundsTrm.localScale / 2;
      }
   }
#endif
}