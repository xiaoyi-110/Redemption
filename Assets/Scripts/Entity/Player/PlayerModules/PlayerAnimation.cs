using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private readonly int speedHash = Animator.StringToHash("Speed");
    private readonly int inputXHash = Animator.StringToHash("InputX");
    private readonly int inputYHash = Animator.StringToHash("InputY");
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void UpdateMovement(Vector2 movement)
    {
        animator.SetFloat(inputXHash, movement.x);
        animator.SetFloat(inputYHash, movement.y);
        animator.SetFloat(speedHash, movement.magnitude);
    }

    public void SetBool(string boolName, bool value)
    {
        animator.SetBool(Animator.StringToHash(boolName), value);
    }

}