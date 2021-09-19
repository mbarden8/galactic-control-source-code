using UnityEngine;

/**
 * This script was put together at the last minute for audio so the
 * functionality is not really flexible, but it gets the job done
 * considering there are not many sound effects / music in this game.
 */
public class AudioManager : MonoBehaviour
{

    public AudioSource[] audioSources;
    public AudioClip shootAudio;
    public AudioClip powerupSpawn;
    public AudioClip powerupPickup;
    public AudioClip playerHit;
    public AudioClip shipExplosion;
    public AudioClip playerDeath;
    public float laserShootVolume = 0.5f;
    public float beauExplode = 0.4f;
    public float powerUpPickupVol = 0.7f;
    public float shipExplode = 0.6f;
    public float playerDeathVol = 0.6f;
    public float windupAudioLength = 2f;
    public float laserLength = 2.25f;

    public void PlayBeaufortExplosion()
    {
        audioSources[1].PlayOneShot(audioSources[0].clip, beauExplode);
    }

    public void PlayEnemyShipExplode()
    {
        audioSources[1].PlayOneShot(shipExplosion, shipExplode);
    }

    public void PlayPlayerDeath()
    {
        audioSources[1].PlayOneShot(playerDeath, playerDeathVol);
    }

    public void PlayShootNoise()
    {
        audioSources[1].PlayOneShot(shootAudio, laserShootVolume);
    }

    public void PlayPoweurpSpawn()
    {
        audioSources[1].PlayOneShot(powerupSpawn);
    }

    public void PlayPlayerHit()
    {
        audioSources[1].PlayOneShot(playerHit);
    }

    public void PlayPowerupPickup()
    {
        audioSources[1].PlayOneShot(powerupPickup);
    }

    public void PlayLaserWindup()
    {
        audioSources[2].Play();
        Invoke("PlayLaserActive", windupAudioLength);
    }

    private void PlayLaserActive()
    {
        audioSources[2].Stop();
        audioSources[3].Play();
        Invoke("EndLaserAudio", laserLength);
    }

    private void EndLaserAudio()
    {
        audioSources[3].Stop();
    }
}
