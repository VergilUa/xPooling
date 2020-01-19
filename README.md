#                             zPooling

## Simplistic, code-driven and optimized solution for any scale of pooling! 
## Pool everything without limits!

### Important:: It will not work without extra scripting involved 

#### How to use and why?
Instead of constantly **Instantiate()**-ing new objects from the prefab (it is very costly operation) and 
**Destroy()**-ing them afterterwards (which is as well), available instances of the objects are re-used.
   
This pooling implementation provides more of a code-driven approach to the pooling, instead of e.g. 
relying to setting instances manually inside the editor or scenes. 
   
This provides way more flexibility, because it allows to write custom logic on spawn / despawn of objects at minimal performance overhead costs.

But, to use pooling, you must have a **GenericPooler** and (optionally) **AutoPooler** setup in the scene.
Objects to be pooled must implement IGenericPoolElement interface, that sets up initial contract (interface).
          
**To instantiate an object from the pool, use:**
  .Pool<T> (to manage entities lifecycle manually) or .AutoPool<T> (to manage entities lifecycle automatically, entity will return to the pool automatially once IsAvailable becomes true. This is also less optimal, but unavoidable in some cases. E.g. AudioSources) extension method on the GameObject prefab. 

Pool will automatically retrieve either older an instance that is available in the pool, or will internally setup a new pool 
(in case its not set up yet), instantiate a new instance of the prefab and grab an entity from it.

In both cases, **.Commission()** method is executed on the entity, allowing to reset / re-initialize state of the object.
       
Instead of doing **Destroy()** on the pooled entity, it is neccessary to define a **.Decommission()** method in the entities class. 
This **.Decommission()** method can be used to reset the entities state before returning it back to the pool. 

**this.ReturnToPool()** / **this.ReturnToAutoPool()** (based on the type of pooler that were used to instantiate the object) 
extension method from within **.Decommission()** must be added, in order to return the object back to the pool.

Entities that are being destroyed (either by Unity, or by mistake) are still could be potentially be referenced within pooler. 
So to prevent referencing entities that are already destroyed, inside entity class **.OnDestroy()** must be implemented. 

In it, **this.RemoveFromPool()** / **this.RemoveFromAutoPool()** must be called.

Entity pool size is managed automatically, and doesn't require anything else.

But in case where a lots of objects has to be instantiated, it is wise use **PoolingExt.SustainPool** / **PoolingExt.SustainAutoPool**
to prepare enough instances of the prefabs.

### Examples available @ zPooling -- Examples

#### For more specific details, feel free to refer the source code, as it is well documented 
(e.g. PoolingExt for the available extension pooling methods)
		
	
#### Compatibility: Compatible with Unity versions that support .Net 4.0
(but with a bit of coding tweaks, can be adopted to the earlier scripting backend versions as well)


#### Suggestions or questions? Ask them at the issue section [here](https://github.com/VergilUa/zPooling/issues), or at [Unity forum](https://forum.unity.com/threads/free-zpooling-code-driven-pooling-framework.812145/)
Feel free to fork and modify, license is MIT.
