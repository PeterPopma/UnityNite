using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities
{
    public static class TransformUtilities
    {
        public static Transform RecursiveFindChild(Transform parent, string childName)
        {
            Transform result = null;

            foreach (Transform child in parent)
            {
                if (child.name == childName)
                    result = child.transform;
                else
                    result = RecursiveFindChild(child, childName);

                if (result != null) break;
            }

            return result;
        }

        public static bool CheckHit(Transform hitTransForm, Vector3 hitPosition, float animationDelay = 0f)
        {
            if (hitTransForm != null)
            {
                if (hitTransForm.GetComponent<Target>() != null)
                {
                    TargetHit(hitTransForm, hitPosition);
                }
                if (hitTransForm.GetComponent<Enemy>() != null && hitTransForm.gameObject.GetComponent<Rigidbody>() == null)
                {
                    EnemyHit(hitTransForm, hitPosition);
                    return true;
                }
            }
            return false;
        }

        public static void TargetHit(Transform hitTransForm, Vector3 hitPosition)
        {
            Target target = hitTransForm.gameObject.GetComponent<Target>();
            target.Hit(hitPosition);
            Rigidbody rigidbody = hitTransForm.gameObject.GetComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.None;
            rigidbody.AddExplosionForce(700f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y, hitTransForm.transform.position.z), 3f);
            Vector3 randomRotation = new Vector3(UnityEngine.Random.Range(30f, 600f), UnityEngine.Random.Range(30f, 600f), UnityEngine.Random.Range(30f, 600f));
            rigidbody.AddRelativeTorque(randomRotation, ForceMode.Impulse);
            rigidbody.useGravity = true;
        }

        public static void EnemyHit(Transform hitTransForm, Vector3 hitPosition)
        {
            Enemy enemy = hitTransForm.gameObject.GetComponent<Enemy>();
            enemy.Hit(hitPosition);
            Rigidbody rigidbody = hitTransForm.gameObject.AddComponent<Rigidbody>();
            rigidbody.AddExplosionForce(700f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y - 1, hitTransForm.transform.position.z), 4f);
            rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f)), ForceMode.VelocityChange);
            rigidbody.useGravity = true;
            // Detach gun
            Transform gun = RecursiveFindChild(hitTransForm.gameObject.transform, "AKM");
            gun.parent = null;
            rigidbody = gun.gameObject.AddComponent<Rigidbody>();
            rigidbody.AddExplosionForce(700f, new Vector3(hitTransForm.transform.position.x, hitTransForm.transform.position.y - 1, hitTransForm.transform.position.z), 4f);
            rigidbody.AddTorque(new Vector3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f)), ForceMode.VelocityChange);
            rigidbody.useGravity = true;
            gun.gameObject.AddComponent<BoxCollider>();
            gun.gameObject.AddComponent<DeleteAfterDelay>();
        }
    }
}
