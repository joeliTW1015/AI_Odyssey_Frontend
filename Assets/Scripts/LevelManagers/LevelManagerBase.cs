using UnityEngine;

public abstract class LevelManagerBase : MonoBehaviour
{
    [Header("Interactive Interface")]
    [SerializeField] GameObject interactiveInterface;

    public virtual void ActivateInteractiveInterface()
    {
        interactiveInterface.SetActive(true);
    }
}
