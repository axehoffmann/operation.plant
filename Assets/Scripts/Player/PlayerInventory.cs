using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Transform radioBindLoc;
    [SerializeField] private Transform weaponBindLoc;

    public Transform RequestBind(Bindable.BindLocation loc)
    {
        if (loc == Bindable.BindLocation.Radio)
            return radioBindLoc;

        if (loc == Bindable.BindLocation.Pipe)
            return weaponBindLoc;

        return null;
    }
}
