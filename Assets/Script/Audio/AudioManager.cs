using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, IObserver
{
    public AudioManager instance { get; private set; }
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioUI masterUI;
    [SerializeField] private AudioUI sfxUI;
    [SerializeField] private AudioUI musicUI;
    [SerializeField] private List<AudioData> listSfx;
    [SerializeField] private List<AudioData> listBGM;
    [SerializeField] private Dictionary<UserAction, AudioData> sfxMap;
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        sfxMap = new Dictionary<UserAction, AudioData>();
        UserAction[] listAction = (UserAction[])Enum.GetValues(typeof(UserAction));
        for (int i = 0; i < listAction.Length; i++)
        {
            sfxMap.Add(listAction[i], listSfx[i]);
        }
        PlayBGM(0);
        SetUI(masterUI, "Master");
        SetUI(sfxUI, "SFX");
        SetUI(musicUI, "Music");
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

    private void SetUI(AudioUI ui, string mixerParameter)
    {
        ui.slider.onValueChanged.RemoveAllListeners();
        ui.slider.onValueChanged.AddListener((float value) => { UpdateSoundMixer(value, ui.value, mixerParameter); });
        ui.slider.value = 0.5f;
    }

    private void UpdateSoundMixer(float sliderValue, TextMeshProUGUI soundValue, string mixerParameter)
    {
        float decibel = Mathf.Clamp(Mathf.Log10(sliderValue) * 20, -80.0f, 0.0f);
        mixer.SetFloat(mixerParameter, decibel);
        soundValue.text = (Mathf.Floor(sliderValue * 100.0f)).ToString() + "%";
    }

    public void OnNotify(UserAction action)
    {
        AudioData actionAudio = sfxMap[action];
        sfxSource.PlayOneShot(actionAudio.clip, actionAudio.volume);
        actionAudio.UpdateLastPlayed(Time.time);
    }
}
