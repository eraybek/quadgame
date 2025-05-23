using UnityEngine;

public class MeleeAttackBehavior : IEnemyAttackBehavior
{
    public void Attack(EnemyController controller)
    {
        float distance = Vector3.Distance(controller.transform.position, controller.BaseTarget.position);
        if (distance <= controller.EnemyData.stopRange + 0.5f)
        {
            // Buraya hasar verme veya etkileşim eklenebilir
            Debug.Log($"{controller.name} yakın dövüş saldırısı yaptı!");
        }
    }
}

