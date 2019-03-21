using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteor : MonoBehaviour
{
    public ParticleSystem trail;
    public ParticleSystem explosion;
    public float height;
    public float speed;
    public float damage;
    public GameObject hitbox;
    public float size;

    private bool exploded = false;

    const string LAYER_SOLDIER = "Soldier";

    // Start is called before the first frame update
    void Start()
    {
        hitbox.transform.localScale = new Vector3(size, size, size);
        this.transform.position = new Vector3(this.transform.position.x, height, this.transform.position.z);
        trail.Play();
        explosion.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (exploded && explosion.isStopped)
        {
            Destroy(this.gameObject);
        }
        else if (!exploded && this.transform.position.y <= 1.0f)
        {
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
            this.transform.position -= new Vector3(0.0f, speed * Time.deltaTime, 0.0f);
        }
    }
}
