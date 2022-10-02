using UnityEngine;

/// <summary>
/// Parent class responsible for extracting beats from..
/// ..spectrum value given by AudioSpectrum.cs
/// </summary>
public class AudioSyncer : MonoBehaviour {
    /// <summary>
    /// Inherit this to cause some behavior on each beat
    /// </summary>
    [SerializeField] private GameObject QRCodeGenerator;
    
    public virtual void OnBeat() {
        QRCodeGenerator.GetComponent<GenerateQrCode>().EncodeTextToQrCode();
        QRCodeGenerator.GetComponent<GenerateQrCode>().EncodeGhostNextTextToQrCode();
        if (!GameManager.Instance.ColorIsSafe()) GameManager.Instance.GameOver();
        
        _mTimer = 0;
        MIsBeat = true;
    }

    /// <summary>
    /// Inherit this to do whatever you want in Unity's update function
    /// Typically, this is used to arrive at some rest state..
    /// ..defined by the child class
    /// </summary>
    public virtual void OnUpdate() {
        // update audio value
        _mPreviousAudioValue = _mAudioValue;
        _mAudioValue = AudioSpectrum.spectrumValue;

        // if audio value went below the bias during this frame
        if (_mPreviousAudioValue > bias &&
            _mAudioValue <= bias) {
            // if minimum beat interval is reached
            if (_mTimer > timeStep)
                OnBeat();
        }

        // if audio value went above the bias during this frame
        if (_mPreviousAudioValue <= bias &&
            _mAudioValue > bias) {
            // if minimum beat interval is reached
            if (_mTimer > timeStep)
                OnBeat();
        }

        _mTimer += Time.deltaTime;
    }

    private void Update() {
        OnUpdate();
    }

    public float bias;
    public float timeStep;
    public float timeToBeat;
    public float restSmoothTime;

    private float _mPreviousAudioValue;
    private float _mAudioValue;
    private float _mTimer;

    protected bool MIsBeat;
}