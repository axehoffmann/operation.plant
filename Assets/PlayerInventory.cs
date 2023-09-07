using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform radioBindLoc;

    public Transform RequestBind(Bindable.BindLocation loc)
    {
        if (loc == Bindable.BindLocation.Radio)
            return radioBindLoc;

        return null;
    }
}
