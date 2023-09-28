using UnityEngine;
using Zenject;

namespace Core.Framework.Utilities
{
	[RequireComponent(typeof(Camera))]
	public class CameraController : MonoBehaviour
	{
		
		[SerializeField]
		private Vector3 CameraOffset;
		[SerializeField]
		private float CameraSpeed = 0.3f;

		[SerializeField]
		[Range(5, 10)]
		private float MinimumZoom = 5f;
		[SerializeField]
		[Range(10, 30)]
		private float MaximumZoom = 10f;
		[SerializeField]
		private float ZoomSpeed = 0.4f;
		[SerializeField]
		private Vector2 Boundary;

		private Transform _target;

		private float _offsetZ;
		private Vector3 _currentVelocity;

		private float _shakeIntensity;
		private float _shakeDecay;
		private float _shakeDuration;

		private int _currentZoomLevel; // 1-10
		private float _currentZoom;
		private Camera _camera;
		public Camera Camera => _camera;
		private bool _followsPlayer;

		[Inject]
		public void Construct()
        {
			_camera = gameObject.GetComponent<Camera>();
		}

        private void Initialize()
        {			
			_currentZoom = MinimumZoom;
			_offsetZ = (transform.position - _target.position).z;
			TeleportCameraToTarget();
			Zoom();
		}

		public void StartFollow(Transform target)
        {
			_followsPlayer = true;
			_target = target;
			Initialize();
		}

		public void StopFollow()
        {
			_followsPlayer = false;
			_target = null;
		}

        private void LateUpdate()
		{
			if (!_followsPlayer || _target == null)
				return;
			Zoom();
			FollowPlayer();
		}

		public virtual void Shake(Vector3 shakeParameters)
		{
			_shakeIntensity = shakeParameters.x;
			_shakeDuration = shakeParameters.y;
			_shakeDecay = shakeParameters.z;
		}

		private void FollowPlayer()
		{
			Vector3 aheadTargetPos = _target.position + Vector3.forward * _offsetZ + CameraOffset;
			Vector3 newCameraPosition = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref _currentVelocity, CameraSpeed);
			Vector3 shakeFactorPosition = Vector3.zero;

			if (_shakeDuration > 0)
			{
				shakeFactorPosition = Random.insideUnitSphere * _shakeIntensity * _shakeDuration;
				_shakeDuration -= _shakeDecay * Time.deltaTime;
			}

			newCameraPosition = newCameraPosition + shakeFactorPosition;
			newCameraPosition = BoundaryCalculate(newCameraPosition);
			transform.position = newCameraPosition;
		}

		
		private void Zoom()
		{
			float currentVelocity = 0f;

			_currentZoom = Mathf.SmoothDamp(
				_currentZoom, 
				(_currentZoomLevel / 10) * (MaximumZoom - MinimumZoom) + MinimumZoom, 
				ref currentVelocity, 
				ZoomSpeed);

			_camera.orthographicSize = _currentZoom;
		}

		private void OnCameraShakeEvent(
			float duration, 
			float amplitude, 
			float frequency)
		{
			Vector3 parameters = new Vector3(amplitude, duration, frequency);
			Shake(parameters);
		}

		private void TeleportCameraToTarget()
		{
			transform.position = _target.position + Vector3.forward * _offsetZ + CameraOffset;
		}

		private Vector3 BoundaryCalculate(Vector3 newPosition)
        {
			float vertExtent =_camera.orthographicSize;
			float horzExtent = vertExtent * Screen.width / Screen.height;

			// Calculations assume map is position at the origin
			float minX = horzExtent - Boundary.x / 2f;
			float maxX = Boundary.x / 2 - horzExtent;
			float minY = vertExtent - Boundary.y / 2;
			float maxY = Boundary.y / 2 - vertExtent;

			var x = Mathf.Clamp(newPosition.x, minX, maxX);
			var y = Mathf.Clamp(newPosition.y, minY, maxY);
			return new Vector3(x, y, -10);
		}

        private void OnDrawGizmos()
        {
			Gizmos.DrawWireCube(transform.position, Boundary);
        }
    }
}