using UnityEngine;


[System.Serializable]
public class AudioData
{
    public AudioClip clip;
    public float volume;
    [SerializeField] private float cooldown;
    private float lastPlayed;
    public AudioData(AudioClip clip, float volume, float cooldown)
    {
        this.clip = clip;
        this.volume = volume;
        this.cooldown = cooldown;
        this.lastPlayed = 0.0f;
    }

    public void UpdateLastPlayed(float time)
    {
        lastPlayed = time;
    }


}
