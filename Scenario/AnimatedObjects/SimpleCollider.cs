using System.Collections;
using UnityEngine;
using Assets.Scripts;

	public abstract class SimpleCollider: MonoBehaviour
	{

        public GameObject owner;
        public float repelForce = 30;
        public float damage = 5;
        public Commandments.Element myElement;
        public float critChance;

        protected Collider2D colisor;

        void OnTriggerStay2D(Collider2D colisor)
        {
            this.colisor = colisor;

            if (owner != null)
            {
                if (colisor.gameObject.tag == owner.tag)
                {
                    HitOwner();
                    return;
                }
            }

            if (colisor.gameObject.tag == "PlayerSpriteTag")
            {
                if (!colisor.isTrigger)
                {
                    HitCharacter(colisor.gameObject.transform.parent.gameObject);
                }
                else
                {
                    HitCharacterAttack();
                }
            }
            else
            {
                if(colisor.gameObject.tag != "MainCamera")
                    HitSomething();
            }

            
        }

        protected virtual void HitCharacterAttack()
        {
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void HitCharacter( GameObject target)
        {
            target.GetComponent<Character>().ReceiveDamage(Utilities.standardVector(colisor.transform.position.x - transform.position.x), repelForce, damage, Random.Range(0, 100.0f) < critChance, myElement);
            if (gameObject)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void HitSomething() {}

        protected virtual void HitOwner(){}

        
	}
