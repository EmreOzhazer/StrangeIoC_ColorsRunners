using System.Collections.Generic;
using DG.Tweening;
using strange.extensions.command.impl;
using Runtime.Data.ValueObject;
using Runtime.Signals;
using UnityEngine;
using Rich.Base.Runtime.Signals;
using Runtime.Mediators.Stack;

namespace Runtime.Controller.StackControllers
{
    public class ItemAdderOnStackCommand : Command
    {
        [Inject] public StackMediator _stackMediator { get; set; }
       
        [Inject] public List<GameObject> _collectableStack{ get; set; }
        [Inject] public StackData _data{ get; set; }
        [Inject] public GameObject _collectableGameObject{ get; set; }

        public ItemAdderOnStackCommand(StackMediator stackMediator, ref List<GameObject> collectableStack,
            ref StackData stackData, GameObject collectableGameObject)
        {
            _stackMediator = stackMediator;
            _collectableStack = collectableStack;
            _data = stackData;
            _collectableGameObject = collectableGameObject;
        }

        public override void Execute()
        {
            
        }
    }
}
