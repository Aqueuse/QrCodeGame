using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using ZXing.QrCode;

public class GenerateQrCode : MonoSingleton<GenerateQrCode> {
    public BeatsDataScriptableObject songData;

    [SerializeField] private RawImage rawImageReceiver;
    private Texture2D _storeEncodedTexture;
    public Color32[] convertPixelToTexture;

    [SerializeField] private RawImage ghostNextrawImageReceiver;
    private Texture2D _ghostNextstoreEncodedTexture;
    public Color32[] ghostNextconvertPixelToTexture;
    
    private List<String> _lyricsLines = new List<string>();
    private int _lyricsLine;
    private int _lyricsNextLine;

    void Start() {
        int screenHeight = Screen.height;
        _storeEncodedTexture = new Texture2D(screenHeight, screenHeight);
        _ghostNextstoreEncodedTexture = new Texture2D(screenHeight, screenHeight);

        InitNewSong();
    }

    public void InitNewSong() {
        _lyricsLine = 0;
        _lyricsNextLine = 0;
        
        SplitLyricsStringToList();
        EncodeGhostNextTextToQrCode();
    }

    private void SplitLyricsStringToList() {
        _lyricsLines = new List<string>();
        string whole = songData.lyrics;
        int partSize = GameManager.Instance.partSize;
        
        var parts = Enumerable.Range(0, (whole.Length + partSize - 1) / partSize)
            .Select(i => whole.Substring(i * partSize, Math.Min(whole.Length - i * partSize, partSize))).ToList();
        
        foreach (var lyricsPart in parts) {
            _lyricsLines.Add(lyricsPart);
        }
    }

    private Color32[] Encode(string textForEncoding, int width, int height) {
        BarcodeWriter writer = new BarcodeWriter {
            Format = BarcodeFormat.QR_CODE,
            Options = new QrCodeEncodingOptions {
                Height = height,
                Width = width
            }
        };
        return writer.Write(textForEncoding);
    }


    public void EncodeTextToQrCode() {
        _lyricsLine++;

        if (_lyricsLine < _lyricsLines.Count) {
            string textWrite = string.IsNullOrEmpty(_lyricsLines[_lyricsLine]) ? "write something please" : _lyricsLines[_lyricsLine];
        
            convertPixelToTexture = Encode(textWrite, _storeEncodedTexture.width, _storeEncodedTexture.height);
            
            _storeEncodedTexture.SetPixels32(convertPixelToTexture);
            _storeEncodedTexture.Apply();
        
            rawImageReceiver.texture = _storeEncodedTexture;
        }
        
        else {
            _lyricsLine = 0;
        }
    }
    
    public void EncodeGhostNextTextToQrCode() {
        _lyricsNextLine++;

        if (_lyricsNextLine < _lyricsLines.Count) {
            string ghostNextTextWrite = string.IsNullOrEmpty(_lyricsLines[_lyricsNextLine]) ? "write something please" : _lyricsLines[_lyricsNextLine];
        
            ghostNextconvertPixelToTexture = Encode(ghostNextTextWrite, _ghostNextstoreEncodedTexture.width, _ghostNextstoreEncodedTexture.height);
            _ghostNextstoreEncodedTexture.SetPixels32(ghostNextconvertPixelToTexture);
            _ghostNextstoreEncodedTexture.Apply();
        
            ghostNextrawImageReceiver.texture = _ghostNextstoreEncodedTexture;
        }

        else {
            _lyricsNextLine = 0;
        }
    }
}
