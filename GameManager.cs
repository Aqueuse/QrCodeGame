using UnityEngine;
using UnityEngine.UI;
using ZXing.QrCode.Internal;

public class GameManager : MonoSingleton<GameManager> {
    public int partSize = 5;

    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject player;

    [SerializeField] private GameObject qrCode;
    [SerializeField] private GameObject qrCodeGrille;

    [SerializeField] private GameObject audioSource;
    private AudioSource _audioSource;

    [SerializeField] private GameObject loosePanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject chooseSongPanel;
    [SerializeField] private GameObject homePanel;
    [SerializeField] private GameObject optionsPanel;
    
    private bool _gameIsPlaying;
    
    private void Start() {
        _audioSource = audioSource.GetComponent<AudioSource>();
    }

    public bool ColorIsSafe() {
        if (qrCode.GetComponent<RawImage>().texture != null) {
            Texture2D qrCodeTexture = qrCode.GetComponent<RawImage>().texture as Texture2D;

            if (qrCodeTexture != null) {
                Color colorPicked = qrCodeTexture.GetPixel(
                    (int)player.GetComponent<RectTransform>().position.x, 
                    (int)player.GetComponent<RectTransform>().position.y
                );

                if (Color.white == colorPicked) return false;
            }
        }
        
        return true;
    }

    void Update() {
        if (_gameIsPlaying) {
            if (_audioSource.isPlaying == false) {
                GameWin();
            }

            else {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvas.GetComponent<RectTransform>(),
                    Input.mousePosition,
                    canvas.GetComponent<Canvas>().worldCamera,
                    out var localpoint);

                float gridWidth = qrCodeGrille.GetComponent<RectTransform>().rect.width * 0.75f;
                float gridHeight = qrCodeGrille.GetComponent<RectTransform>().rect.height * 0.75f;

                float cellWidth = gridWidth / 25f;

                float gridOriginX = Screen.width / 2f - gridWidth / 2f;
                float gridOriginY = Screen.height / 2f - gridHeight / 2f;

                float playerPositionX = gridOriginX + localpoint.x + gridWidth / 2f;
                float playerPositionY = gridOriginY + localpoint.y + gridHeight / 2f;

                Vector3 playerPosition = new Vector3(
                    RoundToNearestGrid(playerPositionX, cellWidth) - cellWidth / 2,
                    RoundToNearestGrid(playerPositionY, cellWidth) + cellWidth / 2,
                    0
                );

                player.GetComponent<RectTransform>().position = playerPosition;
            }
        }
    }

    public void ReturnHome() {
        loosePanel.SetActive(false);
        winPanel.SetActive(false);
        homePanel.SetActive(true);
    }

    public void ChooseSong() {
        chooseSongPanel.SetActive(true);
    }

    public void setDifficultyBias(float bias) {
        _audioSource.GetComponent<AudioSyncer>().bias = bias;
    }

    public void Play(BeatsDataScriptableObject songDataScriptableObject) {
        GenerateQrCode.Instance.songData = songDataScriptableObject;
        audioSource.GetComponent<AudioSource>().clip = songDataScriptableObject._songSound;

        chooseSongPanel.SetActive(false);
        homePanel.SetActive(false);
        
        _gameIsPlaying = true;
        
        _audioSource.time = 0;
        _audioSource.Play();
        
        GenerateQrCode.Instance.InitNewSong();
    }

    public void GameOver() {
        _gameIsPlaying = false;

        qrCode.GetComponent<RawImage>().texture = null;

        // stop the music
        _audioSource.Stop();
        
        // show the loose panel
        loosePanel.SetActive(true);
        winPanel.SetActive(false);
    }

    public void GameWin() {
        // stop the music
        _audioSource.Stop();

        // show the win panel
        winPanel.SetActive(true);
        loosePanel.SetActive(false);
    }

    public void Options() {
        Debug.Log("coin");
        optionsPanel.SetActive(true);
    }

    public void setAudioVolume(float volume) {
        _audioSource.volume = volume;
    }

    public void Quit() {
        Application.Quit();
    }

    float RoundToNearestGrid(float position, float gridCellWidth) {
        float difference = position % gridCellWidth;

        position -= difference;
        
        if (difference > gridCellWidth / 2) {
            position += gridCellWidth;
        }
        
        return position;
    }
}
