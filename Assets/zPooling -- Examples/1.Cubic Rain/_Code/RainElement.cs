using Pooling;
using UnityEngine;

public class RainElement : MonoBehaviour, IGenericPoolElement {
   [Header("Movement Speed")]
   [SerializeField]
   private float _movementSpeed = 5f;

   [SerializeField]
   private Vector3 _movementDirection = Vector3.down;
   
   [Header("Components")]
   [SerializeField]
   private GameObject _gameObject = default;

   [SerializeField]
   private Transform _transform = default;


   private void FixedUpdate() { _transform.position += _movementSpeed * _movementDirection; }

   #region [IGenericPoolElement implementation]
   
   /// <summary>
   /// Internal reference used by the pool, don't redefine it
   /// </summary>
   public int PoolRef { get; set; }

   /// <summary>
   /// Determines if this element should be automatically returned to the AutoPool
   /// (if .AutoPool is used on the prefab)
   ///
   /// No need to define this as well in this case, as we're using GenericPooler
   /// </summary>
   public bool IsAvailable => false;
   
   /// <summary>
   /// This one is setup automatically by the pool, can be used to check if this object is
   /// being used or not, no need to redefine it 
   /// </summary>
   public bool IsCommissioned { get; set; }
   
   /// <summary>
   /// Determines if this object is uses AutoPool (gets setup automatically upon .AutoPool)
   /// No need to redefine it
   /// </summary>
   public bool UsesAutoPool { get; set; }

   /// <summary>
   /// Your custom commission logic here. Enable object, setup initial values, etc
   /// </summary>
   public void Commission() {
      // Simply enabling the object
      _gameObject.SetActive(true);
   }

   /// <summary>
   /// Your custom decommission logic here. Disable object, reset values etc
   /// In the end .ReturnToPool must be used to return the object to the corresponding pool
   /// (GenericPooler or AutoPooler is determined automatically)
   /// </summary>
   public void Decommission() {
      _gameObject.SetActive(false);
      this.ReturnToPool();
   }

   /// <summary>
   /// Needs to be defined for the cases where object is being destroyed for some reason
   /// (Either unload or anything else.
   /// This is used to cleanup references in the pool and removing this instance from it)
   /// </summary>
   public void OnDestroy() {
      this.RemoveFromPool();
   }
   
   #endregion

#if UNITY_EDITOR
   /// <summary>
   /// Caching gameObject, because extra optimization points ;)
   /// </summary>
   protected virtual void OnValidate() {
      _gameObject = gameObject;
      _transform = transform;
   }
#endif
}