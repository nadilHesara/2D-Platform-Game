using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] private AudioSource[] sfx;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if(instance == null)
            instance = this;
        else
            Destroy(this.gameObject);   
    }


    public void PlaySFX(int sfxToPlay, bool randomPitch=true)
    {
        if(sfxToPlay >= sfx.Length)
            return;

        if(randomPitch )    
            sfx[sfxToPlay].pitch = Random.Range(0.9f, 1.1f);

        sfx[sfxToPlay].Play();
    }

    public void StopSFX(int sfxToStop) => sfx[sfxToStop].Stop();
}
