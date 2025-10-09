using System;
public class OnItemDestroyRequestEventArgs :EventArgs
{
    public InteractableObject Interactable { get; private set; }
    private OnItemDestroyRequestEventArgs(InteractableObject interactable)
    {
        Interactable = interactable;
    }
    public static OnItemDestroyRequestEventArgs Create(InteractableObject interactable)
    {
        return new OnItemDestroyRequestEventArgs(interactable);
    }
}
