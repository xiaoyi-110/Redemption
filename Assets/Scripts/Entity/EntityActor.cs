using UnityEngine;

public class EntityActor : Entity
{
    protected Rigidbody2D Rb { get; private set; }
    public Animator Animator { get; private set; }
    public int FacingDirection { get; private set; } = 1;

    protected override void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
    }

}
