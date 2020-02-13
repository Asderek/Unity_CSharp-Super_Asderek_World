using UnityEngine;
using System.Collections;

public class ManaPotion : Item {

    public float value;

    public override bool useItem()
    {
        if (user.GetComponent<Asderek>() != null)
        {
            return user.GetComponent<Asderek>().ChangeMP(value);
        }
        return false;
    }
}