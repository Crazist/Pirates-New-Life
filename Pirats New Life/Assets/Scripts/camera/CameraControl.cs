using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
	[SerializeField] private int _speed;
	[SerializeField] private int _dragSpeed;
	[SerializeField] private int _minHeight;
	[SerializeField] private int _maxHeight;
	[SerializeField] private int _scrollSpeed;
	[SerializeField] private int _panBorderThickness;
	[SerializeField] private Vector2 _panLimit;
	[SerializeField] private Camera _cameraMain;

	[SerializeField] private Vector3 _originPos;
	[SerializeField] private Vector3 _offset;

	private Vector3 _lastPos = Vector3.zero;
	private int _speedMultiplay = 100;
	private bool _drag = false;

	private const int constY = 0;
	private void Update()
	{
		MoveController();
		DragAndDropCameraMove();
	}
	private void MoveController()
	{
		if (!_drag)
		{
			Vector3 pos = _cameraMain.transform.position;
			if (Input.GetKey("w") || Input.mousePosition.y >= Screen.height - _panBorderThickness)
			{
				pos.z += _speed * Time.deltaTime;
			}
			if (Input.GetKey("s") || Input.mousePosition.y <= _panBorderThickness)
			{
				pos.z -= _speed * Time.deltaTime;
			}
			if (Input.GetKey("d") || Input.mousePosition.x >= Screen.width - _panBorderThickness)
			{
				pos.x += _speed * Time.deltaTime;
			}
			if (Input.GetKey("a") || Input.mousePosition.x <= _panBorderThickness)
			{
				pos.x -= _speed * Time.deltaTime;
			}

			float scroll = Input.GetAxis("Mouse ScrollWheel");
			pos.y -= scroll * _scrollSpeed * _speedMultiplay * Time.deltaTime;

			pos.x = Mathf.Clamp(pos.x, -_panLimit.x, _panLimit.x);
			pos.y = Mathf.Clamp(pos.y, _minHeight, _maxHeight);
			pos.z = Mathf.Clamp(pos.z, -_panLimit.y, _panLimit.y);

			_cameraMain.transform.position = pos;
		}
	}

	private void DragAndDropCameraMove()
	{

		if (Input.GetMouseButton(2))
		{
			float h = _dragSpeed * Input.GetAxis("Mouse X");
			float v = _dragSpeed * Input.GetAxis("Mouse Y");
			_lastPos = _cameraMain.transform.position;
			_offset = new Vector3(h, constY, v);
			_drag = true;
		}
		else
		{
			_drag = false;
		}
		if (_drag)
		{
			_lastPos = Vector3.Lerp(_lastPos, _cameraMain.transform.position - _offset, 0.5f);
			_cameraMain.transform.position = _lastPos;
		}
	}

}