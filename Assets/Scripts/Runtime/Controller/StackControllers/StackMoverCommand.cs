using System.Collections.Generic;
using Runtime.Data.ValueObject;
using strange.extensions.command.impl;
using UnityEngine;

namespace Runtime.Controller.StackControllers
{
    public class StackMoverCommand : Command
    {
        [Inject] public StackData _data { get; set; }
        [Inject] public List<GameObject> _collectableStack { get; set; }

        public StackMoverCommand(StackData stackData, List<GameObject> collectableStack)
        {
            _data = stackData;
            _collectableStack = collectableStack;
        }

        public override void Execute()
        {
            // You might want to pass the required parameters to this method if needed
            float directionX = 0.0f;  // Example value, replace with actual value
            List<GameObject> collectableStack = new List<GameObject>();  // Example list, replace with actual list

            float direct = Mathf.Lerp(collectableStack[0].transform.localPosition.x, directionX, _data.LerpSpeed);
            collectableStack[0].transform.localPosition = new Vector3(direct, 1f, 0.335f);
            StackItemsLerpMove(collectableStack);
        }
        
        private void StackItemsLerpMove(List<GameObject> collectableStack)
        {
            for (int i = 1; i < collectableStack.Count; i++)
            {
                Vector3 pos = collectableStack[i].transform.localPosition;
                pos.x = collectableStack[i - 1].transform.localPosition.x;
                float direct = Mathf.Lerp(collectableStack[i].transform.localPosition.x, pos.x, _data.LerpSpeed);
                collectableStack[i].transform.localPosition = new Vector3(direct, pos.y, pos.z);
            }
        }
    }
}