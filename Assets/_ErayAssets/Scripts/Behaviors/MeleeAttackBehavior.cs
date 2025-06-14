using System.Collections;
using UnityEngine;

public class MeleeAttackBehavior : IEnemyAttackBehavior
{
    public void Attack(EnemyController controller)
    {
        controller.FireTimer += Time.deltaTime;

        if (controller.FireTimer < controller.EnemyData.attackInterval)
            return;

        controller.FireTimer = 0f;

        controller.animator.SetTrigger("attackTrigger");

        BaseHealth baseHealth = controller.BaseTarget.GetComponent<BaseHealth>();
        if (baseHealth != null)
        {
            baseHealth.TakeDamage(5); // 5 hasar veriyoruz
            Debug.Log($"{controller.name} base'e 5 hasar verdi!");
        }
        else
        {
            Debug.Log("Base Health bulunamadı");
        }
    }

}

