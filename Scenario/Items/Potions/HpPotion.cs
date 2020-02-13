using UnityEngine;
using System.Collections;

public class HpPotion : Item {

    public float value;

    public override bool useItem()
    {
        if (user.GetComponent<Character>() != null)
        {
            if (user.GetComponent<Character>().hp < user.GetComponent<Character>().maxHP)
            {
                user.GetComponent<Character>().ChangeHP(value);
                return true;
            }
                
        }
        return false;
    }
}
