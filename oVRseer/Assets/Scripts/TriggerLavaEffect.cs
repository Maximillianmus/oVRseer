using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerLavaEffect : MonoBehaviour
{
    public ParticleSystem lavaSplashEffect;
    public ParticleSystem playerDeathEffect;
    public ParticleSystem smokePuffEffect;

    private void OnTriggerEnter(Collider other)
    {
        ParticleSystem lavaSplash = Instantiate(lavaSplashEffect, other.transform.position, Quaternion.Euler(-90, 0, 0));
        lavaSplash.Play();
        StartCoroutine(destroyParticleSystem(lavaSplash, 2));

        if (other.tag == "PlayerArmature")
        {
            ParticleSystem playerDeath = Instantiate(playerDeathEffect, other.transform.position, Quaternion.Euler(-90, 0, 0));
            playerDeath.Play();
            StartCoroutine(destroyParticleSystem(playerDeath, 5));
        }
        else
        {
            ParticleSystem smokePuff = Instantiate(smokePuffEffect, other.transform.position, Quaternion.Euler(-90, 0, 0));
            smokePuff.Play();
            StartCoroutine(destroyParticleSystem(smokePuff, 5));
        }
    }


    IEnumerator destroyParticleSystem(ParticleSystem ps, int time)
    {
        yield return new WaitForSeconds(time);
        Destroy(ps.gameObject);
    }


}
