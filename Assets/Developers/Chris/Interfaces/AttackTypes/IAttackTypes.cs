using UnityEngine;

public interface IAttackTypes
{
    void AreaAttack(Transform spawnPointDetectionArea, float ViewRadius, float ViewAngle, Collider[] detectedCollider, int numberOfCollisions, LayerMask detectionMask, int damage);
    bool SingleAttack(EntityBehavior inEntity, int inDamage);
    bool CanAttack(float fireRate);
}

