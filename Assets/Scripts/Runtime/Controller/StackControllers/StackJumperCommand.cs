using System.Collections.Generic;
using DG.Tweening;
using strange.extensions.command.impl;
using Runtime.Data.ValueObject;
using Runtime.Signals;
using UnityEngine;
using Rich.Base.Runtime.Signals;

namespace Runtime.Controller.StackControllers
{
    public class StackJumperCommand : Command
    {
        [Inject] public StackData _data { get; set; }
      //  private StackSignals StackSignals;
        [Inject] public List<GameObject> _collectableStack { get; set; }
        [Inject] public Transform _levelHolder{ get; set; }
        [Inject] public int _last{ get; set; }
        [Inject] public int _index{ get; set; }

        public StackJumperCommand(StackData stackData, ref List<GameObject> collectableStack, int last, int index)
        {
            _data = stackData;
            _collectableStack = collectableStack;
            _levelHolder = GameObject.Find("LevelHolder").transform;
            _last = last;
            _index = index;
        }

        public override void Execute()
        {
            for (int i = _last; i > _index; i--)
            {
                _collectableStack[i].transform.GetChild(1).tag = "Collectable";
                _collectableStack[i].transform.SetParent(_levelHolder.transform.GetChild(0));
                _collectableStack[i].transform.DOJump(
                    new Vector3(
                        Random.Range(-_data.JumpItemsClampX, _data.JumpItemsClampX + 1),
                        1.12f,
                        _collectableStack[i].transform.position.z + Random.Range(10, 15)),
                    _data.JumpForce,
                    Random.Range(1, 3), 0.5f
                );
                _collectableStack[i].transform.DOScale(Vector3.one, 0);
                _collectableStack.RemoveAt(i);
                _collectableStack.TrimExcess();
            }
        }
    }
}