#                             xPooling

## Simplistic, code-driven and optimized solution for any scale of pooling! 
## Pool everything without limits!

### Important:: It will not work without extra scripting involved 

#### How to use and why?
Instead of constantly **Instantiate()**-ing new objects from the prefab and **Destroy()**-ing them afterterwards (both are costly operations), available instances of the objects are re-used.
   
This pooling implementation provides more of a code-driven approach to the pooling, instead of e.g.relying to setting instances manually inside the editor or scenes. 
   
This provides way more flexibility, because it allows to write custom logic on spawn / despawn of objects at minimal performance overhead costs.

But, to use pooling, you must have a **GenericPooler** and (optionally) **AutoPooler** setup in the scene.
Objects to be pooled must implement **IGenericPoolElement** interface, that sets up initial contract (interface).

Remark: "Entity" in manual is some MonoBehaviour that implements IGenericPoolElement interface and have some logic; (not an actual Entity from Entities package / ECS)


**To instantiate an object from the pool, use extension methods on prefab:**  
```T instance = *gameObjectPrefab*.Pool<T>()``` (to manage entities lifecycle manually)  
------------------------- or  --------------------------------  
```T instance = *gameObjectPrefab*.AutoPool<T>()``` (to manage entities lifecycle automatically)  
Make sure your GameObject prefab has IGenericPoolElement MonoBehaviour attached to it.
  
Entity pooled with .AutoPool will return to the pool automatially once IsAvailable becomes true.  
This is also less optimal, but unavoidable in some cases. E.g. for AudioSources or ParticleSystems;  
  
Both types of pools will automatically retrieve either older an instance that is available in the pool, or will internally setup a new pool (in case its not set up yet), instantiate a new instance of the prefab and grab an entity **T** from it using **TryGetComponent**.
  
In both cases of .Pool / .AutoPool, **.Commission()** method is automatically called on the entity, allowing to reset / re-initialize state of the object.
E.g.:
```
      public void Commission() => _gameObject.SetActive(true);
```
  
Instead of doing **Destroy()** use **Decommission()** on the pooled entity. 
**.Decommission()** method can be used to reset the entities state before returning it back to the pool. 
  
**this.ReturnToPool()** extension method from within **.Decommission()** must be added, in order to return the object back to the pool correctly;
  
Basic implementation could look like this:
```
      public void Decommission() {
         gameObject.SetActive(false);
         this.ReturnToPool();
      }
```
  
Note that both .Commission() and .Decommision() can be used to change entity state to whatever is desired.
  
Entities that are being destroyed (either by Unity, or by mistake) are still could be potentially be referenced within pooler.  
So to prevent referencing entities that are already destroyed, inside entity class **.OnDestroy()** must be implemented.  
  
In it, **this.RemoveFromPool()** must be called, e.g.:
```
public void OnDestroy() => this.RemoveFromPool();
```
Entity pool size is managed automatically, and doesn't require anything else.
In cases where there's a lot of objects to be instantiated, it is wise use **PoolingExt.SustainPool** / **PoolingExt.SustainAutoPool** to prepare enough instances of the prefabs.


**Other properties:**  
**IsCommissioned** is set internally by the poolers before Commission or Decommission is called.  
Can be used check whether entity was commissioned and not returned to the pool yet.

**PoolRef** is internal id for objects pool, no need to set or modify it.

**IsAvailable** is a rule for AutoPooler to Decommission entity when it becomes true. 
AutoPooler automatically performs checks **IsAvailable** on each IGenericPoolElement that is added via **.AutoPool()**. 
If **IsAvailable** is true - then Decommission is called.

Check rate is set via **AutoPooler** script, usually 0.5-1s should be enough.
If your entity doesn't need it, you can define it like so:
```
public bool IsAvailable => false;
```

**UsesAutoPool** is set automatically upon .Pool() / .AutoPool() call, and used internally to defer which pool should be used for **.ReturnToPool()** / **.RemoveFromPool()**.

### Examples available @ xPooling -- Examples

#### For more specific details, feel free to refer the source code, as it is well documented 
(e.g. PoolingExt for the available extension pooling methods)
		
	
#### Compatibility: Compatible with Unity versions that support .Net 4.0
(but with a bit of coding tweaks, can be adopted to the earlier scripting backend versions as well)


#### Suggestions or questions? Ask them at the issue section [here](https://github.com/VergilUa/xPooling/issues), or at [Unity forum](https://forum.unity.com/threads/free-xpooling-code-driven-pooling-framework.812145/)
Feel free to fork and modify, license is MIT.
