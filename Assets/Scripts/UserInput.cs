using UnityEngine;

public class UserInput : MonoBehaviour
{
    public float HorizontalAxis { get; private set; }
    public float VerticalAxis { get; private set; }
    public bool JumpButton { get; private set; }
    public bool SprintButton { get; private set; }
    public bool UseButton { get; private set; }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        HorizontalAxis = Input.GetAxis("Horizontal");
        VerticalAxis = Input.GetAxis("Vertical");
        JumpButton = Input.GetButton("Jump");
        SprintButton = Input.GetButton("Sprint");
        UseButton = Input.GetButtonDown("Use");
    }
}
