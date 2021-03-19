using UnityEngine;

public class CharacterLook : MonoBehaviour
{
    private Quaternion _characterTargetRot;
    private Quaternion _cameraTargetRot;
    private bool _cursorIsLocked = true;

    [SerializeField] private Camera _camera = default;
    [SerializeField] private float _sensitivityX = 2f;
    [SerializeField] private float _sensitivityY = 2f;
    [SerializeField] private float _minimumX = -90f;
    [SerializeField] private float _maximumX = 90f;
    [SerializeField] private float _smoothTime = 5f;
    [SerializeField] private bool _smooth = false;
    [SerializeField] private bool _clampVerticalRotation = true;
    [SerializeField] private bool _lockCursor = true;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        _camera = Camera.main;
        _characterTargetRot = transform.localRotation;
        _cameraTargetRot = _camera.transform.localRotation;
    }

    private void Update()
    {
        LookRotation(transform, _camera.transform);
    }

    private void LookRotation(Transform character, Transform camera)
    {
        float yRot = Input.GetAxis("Mouse X") * _sensitivityX;
        float xRot = Input.GetAxis("Mouse Y") * _sensitivityY;

        _characterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
        _cameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

        if (_clampVerticalRotation)
        {
            _cameraTargetRot = ClampRotationAroundXAxis(_cameraTargetRot);
        }

        if (_smooth)
        {
            character.localRotation = Quaternion.Slerp(character.localRotation, _characterTargetRot,
                _smoothTime * Time.deltaTime);
            camera.localRotation = Quaternion.Slerp(camera.localRotation, _cameraTargetRot,
                _smoothTime * Time.deltaTime);
        }
        else
        {
            character.localRotation = _characterTargetRot;
            camera.localRotation = _cameraTargetRot;
        }

        UpdateCursorLock();
    }

    private void UpdateCursorLock()
    {
        if (_lockCursor)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _cursorIsLocked = false;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                _cursorIsLocked = true;
            }

            if (_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    private Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, _minimumX, _maximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
