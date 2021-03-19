using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class CharacterMovement : MonoBehaviour
{
    private UserInput _userInput;
    private CharacterController _characterController;
    private AudioSource _audioSource;
    private Vector3 _moveDir = Vector3.zero;
    private bool _jump = false;
    private bool _jumping = false;
    private bool _previouslyGrounded;
    private float _stepCycle;
    private float _nextStep;

    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 7f;
    [SerializeField] private float _jumpSpeed = 5f;
    [SerializeField] private float _stickToGroundForce = 0f;
    [SerializeField] private float _gravityMultiplier = 2f;
    [SerializeField] private float _stepInterval = 5;
    [SerializeField] [Range(0f, 1f)] private float _runstepLenghten = 1f;
    [SerializeField] private AudioClip[] _footstepSounds = default;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip _jumpSound = default;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip _landSound = default;           // the sound played when character touches back on ground.

    private void Start()
    {
        _userInput = GetComponent<UserInput>();
        _characterController = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        PlayerMove(_userInput);
    }

    private void PlayerMove(UserInput input)
    {
        _characterController.enabled = true;

        _jump = input.JumpButton;
        float moveSpeed = input.SprintButton ? _runSpeed : _walkSpeed;

        Vector3 desiredMove = input.VerticalAxis * transform.forward + input.HorizontalAxis * transform.right;

        Physics.SphereCast(transform.position, _characterController.radius, Vector3.down, out RaycastHit hitInfo,
                           _characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        _moveDir.x = desiredMove.x * moveSpeed;
        _moveDir.z = desiredMove.z * moveSpeed;

        if (!_previouslyGrounded && _characterController.isGrounded)
        {
            PlayLandingSound();

            _moveDir.y = 0f;
            _jumping = false;
        }

        if (!_characterController.isGrounded && !_jumping && _previouslyGrounded)
        {
            _moveDir.y = 0f;
        }

        _previouslyGrounded = _characterController.isGrounded;

        if (_characterController.isGrounded)
        {
            _moveDir.y = -_stickToGroundForce;

            if (_jump) //Jumping
            {
                _moveDir.y = _jumpSpeed;
                _jump = false;
                _jumping = true;

                PlayJumpSound();
            }
        }
        else
        {
            _moveDir += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
        }

        _characterController.Move(_moveDir * Time.fixedDeltaTime);
        
        _characterController.enabled = false;

        ProgressStepCycle(moveSpeed, !input.SprintButton);
    }

    private void ProgressStepCycle(float speed, bool isWalking)
    {
        if (_characterController.velocity.sqrMagnitude > 0 && (_userInput.HorizontalAxis != 0 || _userInput.VerticalAxis != 0))
        {
            _stepCycle += (_characterController.velocity.magnitude + (speed * (isWalking ? 1f : _runstepLenghten))) * Time.fixedDeltaTime;
        }

        if (_stepCycle <= _nextStep)
            return;

        _nextStep = _stepCycle + _stepInterval;

        PlayFootStepAudio();
    }

    private void PlayFootStepAudio()
    {
        if (!_characterController.isGrounded)
            return;

        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, _footstepSounds.Length);
        _audioSource.clip = _footstepSounds[n];
        _audioSource.PlayOneShot(_audioSource.clip);

        // move picked sound to index 0 so it's not picked next time
        _footstepSounds[n] = _footstepSounds[0];
        _footstepSounds[0] = _audioSource.clip;
    }

    private void PlayJumpSound()
    {
        _audioSource.clip = _jumpSound;
        _audioSource.PlayOneShot(_audioSource.clip);
    }

    private void PlayLandingSound()
    {
        _audioSource.clip = _landSound;
        _audioSource.PlayOneShot(_audioSource.clip);
        _nextStep = _stepCycle + 0.5f;
    }
}
