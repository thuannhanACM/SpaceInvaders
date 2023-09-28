using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Core.Framework.Utilities
{
	[RequireComponent(typeof(Rect))]
	[RequireComponent(typeof(CanvasGroup))]
	public class TouchJoystick : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerUpHandler
	{
		public Vector2 JoystickValue => _joystickValue;

		[SerializeField]
		private float _pressedOpacity = 0.5f;
		[SerializeField]
		private bool _horizontalAxisEnabled = true;
		[SerializeField]
		private bool _verticalAxisEnabled = true;
		[SerializeField]
		private float _maxRange = 1.5f;
		[SerializeField]
		private Transform _knob;

		private Vector2 _neutralPosition;
		private Vector2 _joystickValue;
		private float _initialZPosition;
	    private CanvasGroup _canvasGroup;
		private float _initialOpacity;
		private Vector3 _initialPos;
		private float _canvasScaleFactor;

        [Inject, System.Obsolete("For Inject purpose only")]
		public void Construct()
		{
			Initialize();			
		}

		public void Initialize()
		{
			Canvas c = GetComponentInParent<Canvas>();
			_canvasGroup = GetComponent<CanvasGroup>();
			
			_neutralPosition = transform.position;
			_initialZPosition = transform.position.z;
			_initialOpacity = _canvasGroup.alpha;
			_canvasScaleFactor = c.scaleFactor;
			_maxRange = _maxRange * _canvasScaleFactor;
		}

		public virtual void OnDrag(PointerEventData eventData)
        {
            _canvasGroup.alpha = _pressedOpacity;
            Vector2 newTargetPosition = CalculateNewPos(eventData);

            _joystickValue.x = EvaluateInputValue(newTargetPosition.x);
            _joystickValue.y = EvaluateInputValue(newTargetPosition.y);

            SetKnobPosition(newTargetPosition);
        }

        private void SetKnobPosition(Vector2 newTargetPosition)
        {
            Vector3 newJoystickPosition = _neutralPosition + newTargetPosition;
            newJoystickPosition.z = _initialZPosition;

            _knob.position = newJoystickPosition;
        }

        private Vector2 CalculateNewPos(PointerEventData eventData)
        {
            var newTargetPosition = eventData.position;
            newTargetPosition = Vector2.ClampMagnitude(newTargetPosition - _neutralPosition, _maxRange);

            if (!_horizontalAxisEnabled)
                newTargetPosition.x = 0;
            if (!_verticalAxisEnabled)
                newTargetPosition.y = 0;
            return newTargetPosition;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            ResetStick();
            _canvasGroup.alpha = _initialOpacity;
        }

        private void ResetStick()
        {
            _knob.position = new Vector3(_neutralPosition.x, _neutralPosition.y, _initialZPosition);
            _joystickValue.x = 0f;
            _joystickValue.y = 0f;
        }

        private float EvaluateInputValue(float vectorPosition)
		{
			return Mathf.InverseLerp(0, _maxRange, Mathf.Abs(vectorPosition)) * Mathf.Sign(vectorPosition);
		}

		private void OnEnable()
		{
			Initialize();
			_canvasGroup.alpha = _initialOpacity;
		}

		public virtual void OnPointerDown(PointerEventData data)
		{
			_initialPos = transform.position;
			Vector3 pos = data.position;
			pos.z = _initialZPosition;
			_neutralPosition = pos;
			transform.position = pos;
		}

		public virtual void OnPointerUp(PointerEventData data)
		{
			_neutralPosition = _initialPos;
			transform.position = _initialPos;
		}
	}
}
