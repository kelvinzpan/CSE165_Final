using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public ParticleSystem trail;
    public ParticleSystem explosion;
    public AudioSource explosionSound;
    public AudioSource travlingSound;
    public float height;
    public float speed;
    public float damage;
    public GameObject hitbox;
    public float size;
    public float particleSize;

    private bool exploded = false;

    const string LAYER_SOLDIER = "Soldier";

    // Start is called before the first frame update
    void Start()
    {
        
        this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
        trail.Play();
        explosion.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (exploded && explosion.isStopped)
        {
            travlingSound.Stop();
            explosionSound.Stop();
            Destroy(this.gameObject);
        }
        else if (!exploded && this.transform.position.y <= 1.0f)
        {
            if (!explosionSound.isPlaying) {
                travlingSound.Stop();
                explosionSound.Play();
            }
                
            trail.Stop();
            explosion.Play();
            exploded = true;

            List<Collider> colliders = hitbox.GetComponent<OverlapBox>().GetColliders();
            foreach (Collider collider in colliders)
            {
                if (collider)
                {
                    collider.gameObject.GetComponent<Soldier>().TakeDamage(damage);
                }
            }
        }
        else
        {
            if (!travlingSound.isPlaying && !explosionSound.isPlaying)
                travlingSound.Play();
            this.transform.position -= new Vector3(0.0f, speed * Time.deltaTime, 0.0f);
        }
    }

    public void SetForce(float force) // force is float between 0 and 1
    {
        trail.startSize = particleSize * (1.0f + force * 2.0f);
        explosion.startSize = particleSize * (1.0f + force * 2.0f);
        speed = speed * (1.0f + force * 2.0f);
        size = size * (1.0f + force * 1.5f);
        damage = damage * (1.0f + force);
        hitbox.transform.localScale = new Vector3(size, size, size);
    }
}
