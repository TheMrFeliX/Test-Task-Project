using UnityEngine;

public class CharacterObjectInteraction : MonoBehaviour
{
    private UserInput _userInput;
    private InteractableObject _pickedObject;
    private bool _isPicked = false;

    [SerializeField] private Camera _camera = default;
    [SerializeField] private Transform _pickedTransform = default;
    [SerializeField] private float _useDistance = 10f;
    [SerializeField] private float _kickPower = 10f;

    private void Start()
    {
        _userInput = GetComponent<UserInput>();
        _camera = Camera.main;
    }

    private void Update()
    {
        UseObject();
    }

    private void UseObject()
    {
        if (!_userInput.UseButton)
            return;

        if (_isPicked)
        {
            PutPickedObject();
            return;
        }

        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out RaycastHit hit, _useDistance))
        {
            if (hit.transform == null)
                return;

            if (!hit.transform.TryGetComponent<InteractableObject>(out var interactableObject))
                return;

            switch (interactableObject.ObjectType)
            {
                case InteractableObject.Type.Pickupable:
                    {
                        PickUpObject(interactableObject);
                        break;
                    }
                case InteractableObject.Type.Takeable:
                    {
                        TakeObject(interactableObject);
                        break;
                    }
                case InteractableObject.Type.Kickable:
                    {
                        KickObject(interactableObject, hit.point);
                        break;
                    }
            }
        }
    }

    private void PutPickedObject()
    {
        _pickedObject.transform.SetParent(null);
        _pickedObject.transform.position = transform.position + transform.forward * 2f;
        _pickedObject.gameObject.SetActive(true);
        _isPicked = false;

        if (!_pickedObject.transform.TryGetComponent<Rigidbody>(out var rb))
            return;

        rb.isKinematic = false;
    }

    private void PickUpObject(InteractableObject interactableObject)
    {
        Quaternion lookRotation = Quaternion.LookRotation(_camera.transform.forward);
        lookRotation.eulerAngles = new Vector3(0, lookRotation.eulerAngles.y, 0);

        interactableObject.transform.position = _pickedTransform.position;
        interactableObject.transform.eulerAngles = lookRotation.eulerAngles;
        interactableObject.transform.SetParent(_pickedTransform);

        _pickedObject = interactableObject;
        _isPicked = true;

        if (!interactableObject.transform.TryGetComponent<Rigidbody>(out var rb))
            return;

        rb.isKinematic = true;
    }

    private void TakeObject(InteractableObject interactableObject)
    {
        PickUpObject(interactableObject);

        interactableObject.gameObject.SetActive(false);
    }

    private void KickObject(InteractableObject interactableObject, Vector3 point)
    {
        if (!interactableObject.transform.TryGetComponent<Rigidbody>(out var rb))
            return;

        Vector3 force = _camera.transform.forward * _kickPower;
        rb.AddForceAtPosition(force, point, ForceMode.Impulse);
    }
}
