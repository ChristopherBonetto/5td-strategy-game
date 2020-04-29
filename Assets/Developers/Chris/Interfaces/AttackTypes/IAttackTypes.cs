using UnityEngine;

public interface IAttackTypes
{
    void AreaAttack(Transform spawnPointDetectionArea, float ViewRadius, float ViewAngle, Collider[] detectedCollider, int numberOfCollisions, LayerMask detectionMask, int damage);
    bool SingleAttack(GameObject inFocusObj, int inDamage);
    bool CanAttack(float fireRate);
}

