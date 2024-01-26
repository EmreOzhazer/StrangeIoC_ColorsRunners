using strange.extensions.signal.impl;
using UnityEngine;


namespace Runtime.Signals
{
    public class StackSignals
    {
        
       public Signal onAddStack = new Signal();
       public Signal<GameObject> onCollectableStack = new Signal<GameObject>();
       public static Signal<GameObject> onInteractionObstacle = new Signal<GameObject>();
       public Signal onInteractionCollectable = new Signal();
       public Signal<Vector2> onStackPlayerFollow = new Signal<Vector2>();
       public Signal<Vector2> onStackMover = new Signal<Vector2>();
       
        
    }
}