using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, IObserver
{
    public static AudioManager instance { get; private set; }
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private List<AudioData> listSfx;
    [SerializeField] private List<AudioData> listBGM;
    [SerializeField] private Dictionary<UserAction, AudioData> sfxMap;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    private float minDecibel = -80.0f;
    private float maxDecibel = 0.0f;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(instance);
        sfxMap = new Dictionary<UserAction, AudioData>();
        UserAction[] listAction = (UserAction[])Enum.GetValues(typeof(UserAction));
        for (int i = 0; i < listAction.Length; i++)
        {
            sfxMap.Add(listAction[i], listSfx[i]);
        }
        PlayBGM(0);
    }

    private void OnEnable()
    {
        GameManager.instance.AddObserver(this);
    }

    private void OnDisable()
    {
        GameManager.instance.RemoveObserver(this);
    }

    public void PlayBGM(int index)
    {
        bgmSource.clip = listBGM[index].clip;
        bgmSource.volume = listBGM[index].volume;
        bgmSource.Play();
    }

    public void InitializeAudioUI(AudioUI ui, string mixerParameter)
    {
        ui.slider.onValueChanged.RemoveAllListeners();
        ui.slider.onValueChanged.AddListener((float value) => { UpdateSoundMixer(value, ui.value, mixerParameter); });
        float currentMixerValue;
        mixer.GetFloat(mixerParameter, out currentMixerValue);
        ui.slider.value = DecibelToFloat(currentMixerValue);
    }

    private void UpdateSoundMixer(float sliderValue, TextMeshProUGUI soundValue, string mixerParameter)
    {
        float decibel = Mathf.Clamp(Mathf.Log10(sliderValue) * 20, -80.0f, 0.0f);
        mixer.SetFloat(mixerParameter, decibel);
        soundValue.text = (Mathf.Floor(sliderValue * 100.0f)).ToString() + "%";
    }

    private float DecibelToFloat(float value)
    {
        return (value - minDecibel) / (maxDecibel - minDecibel);
    }

    public void OnNotify(UserAction action)
    {
        AudioData actionAudio = sfxMap[action];
        sfxSource.PlayOneShot(actionAudio.clip, actionAudio.volume);
        actionAudio.UpdateLastPlayed(Time.time);
    }
}
