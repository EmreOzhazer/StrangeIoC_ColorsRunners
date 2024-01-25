using DG.Tweening;
using Rich.Base.Runtime.Concrete.Injectable.Mediator;
using Runtime.Model.Player;
using Runtime.Model.Stack;
using Runtime.Signals;
using Runtime.Views.Stack;
using UnityEngine;

namespace Runtime.Mediators.Stack
{
    public class StackMediator : MediatorLite
    {
        [Inject] public StackView StackView { get; set; }

        [Inject] public IStackModel Model { get; set; }

        [Inject] public StackSignals StackSignals { get; set; }

        public override void OnRegister()
        {
            base.OnRegister();
            StackSignals.onStackPlayerFollow.AddListener(StackView.OnStackMove);
            StackSignals.onCollectableStack.AddListener(StackView.OnStackCollectable);
            
        }
        
        public override void OnRemove()
        {
            base.OnRemove();

            StackSignals.onStackPlayerFollow.RemoveListener(StackView.OnStackMove);
            StackSignals.onCollectableStack.RemoveListener(StackView.OnStackCollectable);
            
        }
        
        public override void OnEnabled()
        {
            base.OnEnabled();
            StackView.SetStackData(Model.StackData.StackData);
        }
    }
}