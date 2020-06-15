using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


using UnityEngine.Audio;

/// <summary>
/// フェード付きBGMとSE再生に対応したaudioマネージャー
/// </summary>
public class AudioManeger : SingletonMonoBehaviour<AudioManeger>
{

    [SerializeField, Range(0f, 1f), Header("フェードイン完了時のBGM音量です")]
    private float TargetVolume = 1f;//BGM再生音量

    [Header("フェードにかかる時間")]
    public float TimeToFade = 2.0f;//フェードにかかる時間
    [Header("重ねる割合　１で同時に実行０でアウトが終わったらイン")]
    public float CrossFadeRatio = 1f;//重ねる割合　１で同時に実行０でアウトが終わったらイン
    [SerializeField, Header("同時にならせる最大SE数")]
    private int MaxSE;
    private int MAxVoice = 1;
    [SerializeField, Header("bgmミキサーをセット")]
    private AudioMixerGroup bgmMixerGroup;
    [SerializeField, Header("seミキサーをセット")]
    private AudioMixerGroup seMixerGroup;
    [SerializeField, Header("Voiceミキサーをセット")]
    private AudioMixerGroup voiceMixerGroup;
    [NonSerialized]
    public AudioSource CurrentAudioSource = null;
    public AudioSource SubAudioSource/// FadeOut中、もしくは再生待機中のAudioSource
    {
        get
        {
            //bgmSourcesのうち、CurrentAudioSourceでない方を返す
            if (this.AudioSources == null)
                return null;
            foreach (AudioSource s in this.AudioSources)
            {
                if (s != this.CurrentAudioSource)
                {
                    return s;
                }
            }
            return null;
        }
    }


    private List<AudioSource> AudioSources = null;// BGMを再生するためのAudioSourceです。クロスフェードを実現するための２つの要素を持ちます。
    private List<AudioSource> seSources = null; //SEを再生するためのAudioSourceです。
    private List<AudioSource> voiceSources = null;
    private Dictionary<string, AudioClip> BgmAudioClipDict = null;// 再生可能なBGM(AudioClip)のリストです。実行時に Resources/Audio/BGM フォルダから自動読み込みされます。
    private Dictionary<string, AudioClip> SeAudioClipDict = null;// 再生可能なSE(AudioClip)のリストです。実行時に Resources/Audio/SE フォルダから自動読み込みされます。
    private Dictionary<string, AudioClip> VoiceAudioClipDict = null;
    private Dictionary<string, AudioClip> SVoiceAudioClipDict = null;
    private IEnumerator fadeOutCoroutine; //コルーチン中断に使用

    private IEnumerator fadeInCoroutine;// コルーチン中断に使用

    private bool isLoading;

    public bool IsLoading
    {
        get
        {
            return isLoading;
        }
    }

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            //Destroy(this.gameObject);
            Debug.LogError(
               "AudioManeger" +
                " は既に他のGameObjectにアタッチされているため、コンポーネントを破棄しました." +
                " アタッチされているGameObjectは " + Instance.gameObject.name + " です.");
            return;
        }

        DontDestroyOnLoad(this.gameObject);

        //AudioSourceを２つ用意。クロスフェード時に同時再生するために２つ用意する。
        this.AudioSources = new List<AudioSource>();
        this.seSources = new List<AudioSource>();
        this.voiceSources = new List<AudioSource>();
        this.AudioSources.Add(this.gameObject.AddComponent<AudioSource>());
        this.AudioSources.Add(this.gameObject.AddComponent<AudioSource>());
        foreach (AudioSource s in this.AudioSources)
        {
            s.playOnAwake = false;
            s.volume = 0f;
            s.loop = true;
            s.outputAudioMixerGroup = bgmMixerGroup;
        }
        StartCoroutine(Load());
        //SearchBGM();
        //SerchSE();
        //SerchVoice();
        //有効なAudioListenerが一つも無い場合は生成する。（大体はMainCameraについてる）
        if (FindObjectsOfType(typeof(AudioListener)).All(o => !((AudioListener)o).enabled))
        {
            this.gameObject.AddComponent<AudioListener>();
        }
    }

    private void SerchSE()
    {
        //[Resources/Audio/SE]フォルダからBGMを探す
        this.SeAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip se in Resources.LoadAll<AudioClip>("Audio/SE"))
        {


            this.SeAudioClipDict.Add(se.name, se);
        }
    }

    private void SearchBGM()
    {
        //[Resources/Audio/BGM]フォルダからBGMを探す
        this.BgmAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Audio/BGM"))
        {
            this.BgmAudioClipDict.Add(bgm.name, bgm);
        }
    }
    private void SerchVoice()
    {

        this.VoiceAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Audio/Voice"))
        {
            this.VoiceAudioClipDict.Add(bgm.name, bgm);
        }
        
    
    }

    /// <summary>
    /// 多分固まるけど
    /// </summary>
    /// <returns></returns>
    public IEnumerator Load()
    {
        isLoading = true;
        yield return new WaitForSecondsRealtime(0.5f);

        this.BgmAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Audio/BGM"))
        {
            this.BgmAudioClipDict.Add(bgm.name, bgm);
        }
        yield return new WaitForFixedUpdate();
        this.SeAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip se in Resources.LoadAll<AudioClip>("Audio/SE"))
        {
            this.SeAudioClipDict.Add(se.name, se);
        }
        yield return new WaitForFixedUpdate();
        this.VoiceAudioClipDict = new Dictionary<string, AudioClip>();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Audio/Voice"))
        {
            this.VoiceAudioClipDict.Add(bgm.name, bgm);
        }
        yield return new WaitForFixedUpdate();
        foreach (AudioClip bgm in Resources.LoadAll<AudioClip>("Audio/Shop"))
        {
            this.VoiceAudioClipDict.Add(bgm.name, bgm);
        }
        yield return new WaitForFixedUpdate();
        isLoading = false;
    }

    #region BGM
    /// <summary>
    /// BGMを再生します。
    /// </summary>
    /// <param name="bgmName">BGM名</param>
    public void PlayBgm(string bgmName)
    {
        if (!this.BgmAudioClipDict.ContainsKey(bgmName))
        {
            Debug.LogError(string.Format("BGM名[{0}]が見つかりません。", bgmName));
            return;
        }

        if ((this.CurrentAudioSource != null)
            && (this.CurrentAudioSource.clip == this.BgmAudioClipDict[bgmName]))
        {
            //すでに指定されたBGMを再生中
            return;
        }

        //クロスフェード中なら中止
        stopFadeOut();
        stopFadeIn();

        //再生中のBGMをフェードアウト開始
        this.BgmStop();

        float fadeInStartDelay = this.TimeToFade * (1.0f - this.CrossFadeRatio);

        //BGM再生開始
        this.CurrentAudioSource = this.SubAudioSource;
        this.CurrentAudioSource.clip = this.BgmAudioClipDict[bgmName];
        this.fadeInCoroutine = fadeIn(this.CurrentAudioSource, this.TimeToFade, this.CurrentAudioSource.volume, this.TargetVolume, fadeInStartDelay);
        StartCoroutine(this.fadeInCoroutine);
    }
    /// <summary>
    /// BGMを停止します。
    /// </summary>
    public void BgmStop()
    {
        if (this.CurrentAudioSource != null)
        {
            this.fadeOutCoroutine = fadeOut(this.CurrentAudioSource, this.TimeToFade, this.CurrentAudioSource.volume, 0f);
            StartCoroutine(this.fadeOutCoroutine);
        }
    }
    /// <summary>
    /// BGMをただちに停止します。
    /// </summary>
    public void StopBgmImmediately()
    {
        this.fadeInCoroutine = null;
        this.fadeOutCoroutine = null;
        foreach (AudioSource s in this.AudioSources)
        {
            s.Stop();
        }
        this.CurrentAudioSource = null;
    }
    /// <summary>
    /// BGMをフェードインさせながら再生を開始します。
    /// </summary>
    /// <param name="bgm">AudioSource</param>
    /// <param name="timeToFade">フェードインにかかる時間</param>
    /// <param name="fromVolume">初期音量</param>
    /// <param name="toVolume">フェードイン完了時の音量</param>
    /// <param name="delay">フェードイン開始までの待ち時間</param>
    private IEnumerator fadeIn(AudioSource bgm, float timeToFade, float fromVolume, float toVolume, float delay)
    {
        if (delay > 0)
        {
            yield return new WaitForSeconds(delay);
        }


        float startTime = Time.time;
        bgm.Play();
        while (true)
        {
            float spentTime = Time.time - startTime;
            if (spentTime > timeToFade)
            {
                bgm.volume = toVolume;
                this.fadeInCoroutine = null;
                break;
            }

            float rate = spentTime / timeToFade;
            //float vol = Mathf.Lerp(fromVolume, toVolume, rate);
            float vol = Mathf.Pow(Mathf.Sin(rate * (Mathf.PI / 2)), 2);
            bgm.volume = vol;
            yield return null;
        }
    }
    /// <summary>
    /// BGMをフェードアウトし、その後停止します。
    /// </summary>
    /// <param name="bgm">フェードアウトさせるAudioSource</param>
    /// <param name="timeToFade">フェードアウトにかかる時間</param>
    /// <param name="fromVolume">フェードアウト開始前の音量</param>
    /// <param name="toVolume">フェードアウト完了時の音量</param>
    private IEnumerator fadeOut(AudioSource bgm, float timeToFade, float fromVolume, float toVolume)
    {
        float startTime = Time.time;
        while (true)
        {
            float spentTime = Time.time - startTime;
            if (spentTime > timeToFade)
            {
                bgm.volume = toVolume;
                bgm.Stop();
                this.fadeOutCoroutine = null;
                break;
            }

            float rate = spentTime / timeToFade;
            // float vol = Mathf.Lerp(fromVolume, toVolume, rate);
            float vol = Mathf.Pow(Mathf.Cos(rate * (Mathf.PI / 2)), 2);
            bgm.volume = vol;
            bgm.clip = null;
            yield return null;
        }
    }
    /// <summary>
    /// フェードイン処理を中断します。
    /// </summary>
    private void stopFadeIn()
    {
        if (this.fadeInCoroutine != null)
            StopCoroutine(this.fadeInCoroutine);
        this.fadeInCoroutine = null;

    }
    /// <summary>
    /// フェードアウト処理を中断します。
    /// </summary>
    private void stopFadeOut()
    {
        if (this.fadeOutCoroutine != null)
            StopCoroutine(this.fadeOutCoroutine);
        this.fadeOutCoroutine = null;
    }
    #endregion
    #region SE
    public void PlaySE(string seName)
    {
        if (!this.SeAudioClipDict.ContainsKey(seName)) throw new ArgumentException(seName + " not found", "seName");

        AudioSource source = this.seSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.seSources.Count >= this.MaxSE)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = seMixerGroup;
            this.seSources.Add(source);
        }

        source.clip = this.SeAudioClipDict[seName];
        source.Play();
    }
    public void StopSE()
    {
        this.seSources.ForEach(s => s.Stop());
    }
    #endregion
    #region Voice
    public void PlayVoice(string seName)
    {
        if (!this.VoiceAudioClipDict.ContainsKey(seName)) throw new ArgumentException(seName + " not found", "seName");

        AudioSource source = this.voiceSources.FirstOrDefault(s => !s.isPlaying);
        if (source == null)
        {
            if (this.voiceSources.Count >= this.MAxVoice)
            {
                Debug.Log("SE AudioSource is full");
                return;
            }

            source = this.gameObject.AddComponent<AudioSource>();
            source.outputAudioMixerGroup = voiceMixerGroup;
            this.voiceSources.Add(source);
        }

        source.clip = this.VoiceAudioClipDict[seName];
        source.Play();
    }
    public void StopVoice()
    {
        this.voiceSources.ForEach(s => s.Stop());
    }
    #endregion
}
