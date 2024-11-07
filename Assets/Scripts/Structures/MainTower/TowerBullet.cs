using UnityEngine;

public class TowerBullet : MonoBehaviour
{
    #region Variables

    public float Speed;
    public Transform target;
    public GameObject impactParticle;

    public Vector3 impactNormal;
    Vector3 lastBulletPosition;
    public TowerTurret twr;
    float i = 0.05f; 

    public AudioClip sound;
    private AudioSource audioSource;

    #endregion

    #region Unity Methods

    void Update()
    {
        //Bullet follows target
        if (target)
        {
            transform.LookAt(target);
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * Speed);
            lastBulletPosition = target.transform.position;
        }
        else//Move bullet (enemy was disapeared)
        {
            transform.position = Vector3.MoveTowards(transform.position, lastBulletPosition, Time.deltaTime * Speed);

            if (transform.position == lastBulletPosition)
            {
                Destroy(gameObject, i);

                //Bullet hit ( enemy was disapeared )
                if (impactParticle != null)
                {
                    impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;  // Tower`s hit
                    Destroy(impactParticle, 3);
                    return;
                }

            }
        }
    }

    //Bullet hit enemy
    void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.transform == target)
        {
            //Apply Damage to enemy
            target.GetComponent<EnemyController>().Damage(twr.GetDamage());

            //Destroy bullet
            Destroy(gameObject, i); 

            //Effects
            impactParticle = Instantiate(impactParticle, target.transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;
            impactParticle.transform.parent = target.transform;

            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(sound);

            Destroy(impactParticle, 3);

            return;
        }
    }
    #endregion
}



