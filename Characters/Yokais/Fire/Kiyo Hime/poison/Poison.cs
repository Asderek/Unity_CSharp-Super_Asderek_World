using UnityEngine;
using System.Collections;
using Assets.Scripts;

public class Poison : SimpleCollider {

    protected virtual void Update()
    {
        if (GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            Destroy(gameObject);
        }
    }

  protected override void HitCharacterAttack()
    {
        if (UIManager.GetInstance().getSelectedWeapon().toElement() == Commandments.Element.WIND)
        {
            Destroy(gameObject);
        }
    }

  protected override void HitCharacter(GameObject target)
    {
        target.GetComponent<Character>().ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), repelForce, damage, Random.Range(0, 100.0f) < critChance, myElement);
    }

  void OnTriggerStay(Collider colisor)
  {
      if (colisor.gameObject.tag == "PlayerSpriteTag")
      {
          if (!colisor.isTrigger)
          {
              HitCharacter(colisor.gameObject.transform.parent.gameObject);
          }
      }
  }
}
