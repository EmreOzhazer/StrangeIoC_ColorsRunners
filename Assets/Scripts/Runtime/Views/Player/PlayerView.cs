using DG.Tweening;
using Rich.Base.Runtime.Abstract.View;
using Runtime.Data.UnityObject;
using Runtime.Data.ValueObject;
using Runtime.Key;
using Runtime.Enums;
using Runtime.Signals;
using Runtime.Views.Stack;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.Views.Player
{
    public class PlayerView : RichView
    {
        #region Self Variables

        #region Public Variables

        public UnityAction onReset = delegate { };
        public UnityAction<Transform, Transform> onStageAreaEntered = delegate { };
        public UnityAction onFinishAreaEntered = delegate { };
        public UnityAction onRedWallPassed = delegate { };
        public UnityAction onBlueWallPassed = delegate { };
        public UnityAction onGreenWallPassed = delegate { };
        public UnityAction<GameObject> onStackCollectable = delegate { };
        public UnityAction<GameObject> onInteractionObstacle =  delegate { };
        
        public UnityAction<Vector2> onSetPosStack = delegate { };
        public UnityAction<Vector2> onStackPlayerFollow = delegate { };
        
        #endregion

        #region Serialized Variables
        [Inject] public StackView StackView { get; set; }
        [SerializeField] private new Rigidbody rigidbody;
        [SerializeField] private new Renderer renderer;
        [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
        [SerializeField] private TextMeshPro scaleText;
        [SerializeField] private ParticleSystem confettiParticle;

        #endregion

        #region Private Variables

        [ShowInInspector] private bool _isReadyToMove, _isReadyToPlay;
        [ShowInInspector] private float _xValue;

        private float2 _clampValues;
        [ShowInInspector] private PlayerData _playerData;
        [ShowInInspector] private CollectableColorTypes playerColorType;
        [ShowInInspector] private CollectableColorData _collectableColorData;
        [ShowInInspector] private CollectableData _dataCollectable;
        private readonly string _collectableDataPath = "Data/CD_Collectable";


        private readonly string _stageArea = "StageArea";
        private readonly string _finish = "FinishArea";
        private readonly string _miniGame = "MiniGameArea";
        private readonly string _collectable = "Collectable";
        private readonly string _obstacle = "Obstacle";
        #endregion

        #region Color Changer Gates Tags

        private readonly string _redWall = "Red Wall";
        private readonly string _blueWall = "Blue Wall";
        private readonly string _greenWall = "Green Wall";

        #endregion
        #endregion

        private CollectableData GetCollectableData() => Resources.Load<CD_Collectable>(_collectableDataPath).Data;
        public void SetPlayerData(PlayerData playerData)
        {
            _playerData = playerData;
        }
        internal void SetColorData(CollectableColorData colorData)
        {
            _collectableColorData = colorData;
        }
        public void OnInputDragged(HorizontalInputParams horizontalInputParams)
        {
            _xValue = horizontalInputParams.HorizontalValue;
            _clampValues = horizontalInputParams.ClampValues;
        }

        public void OnInputReleased()
        {
            IsReadyToMove(false);
        }

        public void OnInputTaken()
        {
            IsReadyToMove(true);
        }

        private void FixedUpdate()
        {
            if (!_isReadyToPlay)
            {
                StopPlayer();
                return;
            }
            if (_isReadyToMove)
            {
                MovePlayer();
            }
            else
            {
                StopPlayerHorizontally();
            }
            SetStackPos();
        }

        private void StopPlayer()
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        private void StopPlayerHorizontally()
        {
            rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, _playerData.MovementData.ForwardSpeed);
            rigidbody.angularVelocity = Vector3.zero;
        }

        private void MovePlayer()
        {
            var velocity = rigidbody.velocity;
            velocity = new Vector3(_xValue * _playerData.MovementData.SidewaysSpeed, velocity.y, _playerData.MovementData.ForwardSpeed);
            rigidbody.velocity = velocity;
            var position1 = rigidbody.position;
            Vector3 position;
            position = new Vector3(Mathf.Clamp(position1.x, _clampValues.x, _clampValues.y),
                (position = rigidbody.position).y, position.z);
            rigidbody.position = position;
        }

        internal void IsReadyToPlay(bool condition)
        {
            _isReadyToPlay = condition;
        }

        public void IsReadyToMove(bool condition)
        {
            _isReadyToMove = condition;
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_stageArea))
            {
                onStageAreaEntered?.Invoke(transform, other.transform.parent.transform);

                IsReadyToPlay(false);
            }

            if (other.CompareTag(_obstacle))
            {
                StackView.onInteractionObstacle?.Invoke(transform.parent.gameObject);
            }
            
            if (other.CompareTag(_finish))
            {
                onFinishAreaEntered?.Invoke();
                return;
            }
            if (other.gameObject.name == _collectable)
            {
                onStackCollectable?.Invoke(other.transform.gameObject);
                
            }
            if (other.CompareTag(_blueWall))
            {
                onBlueWallPassed?.Invoke();
            }

            if (other.CompareTag(_greenWall))
            {
                onGreenWallPassed?.Invoke();
            }

            if (other.CompareTag(_redWall))
            {
                onRedWallPassed?.Invoke();
            }
            
            if (other.CompareTag(_miniGame))
            {
                //Write the MiniGame Mechanics
            }
        }
        internal void SetStackPos()
        {
            var position = transform.position;
            Vector2 pos = new Vector2(position.x, position.z);
            onSetPosStack?.Invoke(pos);
        }
        internal void UpgradePlayerVisual(CollectableColorTypes typeValue)
        {
            playerColorType = typeValue;
            UpgradePlayerVisualColor((int)typeValue);
        }
        
        internal void UpgradePlayerVisualColor(int value)
        {
            _dataCollectable = GetCollectableData();
            SetColorData(_dataCollectable.ColorData);
            skinnedMeshRenderer.material = _collectableColorData.MaterialsList[value];
        }
        
        internal void ScaleUpPlayer()
        {
            renderer.gameObject.transform.DOScaleX(_playerData.MeshData.ScaleCounter, 1).SetEase(Ease.Flash);
        }

        internal void ShowUpText()
        {
            scaleText.gameObject.SetActive(true);
            scaleText.DOFade(1, 0f).SetEase(Ease.Flash).OnComplete(() => scaleText.DOFade(0, 0).SetDelay(.65f));
            scaleText.rectTransform.DOAnchorPosY(.85f, .65f).SetRelative(true).SetEase(Ease.OutBounce).OnComplete(() =>
                scaleText.rectTransform.DOAnchorPosY(-.85f, .65f).SetRelative(true));
        }

        internal void PlayConfettiParticle()
        {
            confettiParticle.Play();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var transform1 = transform;
            var position1 = transform1.position;

            Gizmos.DrawSphere(new Vector3(position1.x, position1.y - 1f, position1.z + .9f), 1.7f);
        }

        internal void OnReset()
        {
            onReset?.Invoke();
            StopPlayer();
            _isReadyToMove = false;
            _isReadyToPlay = false;
            renderer.gameObject.transform.DOScaleX(1, 1).SetEase(Ease.Linear);
        }
    }
}