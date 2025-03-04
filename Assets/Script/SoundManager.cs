using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }
    [Header("Music Settings")]
    public AudioSource musicSource;
    public List<AudioClip> musicTracks;
    [Header("SFX Settings")]
    private List<int> playedIndexes = new List<int>(); // Danh sách bài đã chơi
    private int lastTrackIndex = -1; // Để tránh phát lại bài trước đó

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        musicSource.loop = false;
        PlayRandomMusic();
        StartCoroutine(CheckMusicEnd());
    }

    private void PlayRandomMusic()
    {
        if (musicTracks.Count == 0) return;

        // Reset danh sách nếu tất cả bài đã chơi
        if (playedIndexes.Count >= musicTracks.Count)
            playedIndexes.Clear();

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, musicTracks.Count);
        }
        while (randomIndex == lastTrackIndex || playedIndexes.Contains(randomIndex)); // Tránh trùng bài trước hoặc bài đã chơi hết lượt

        lastTrackIndex = randomIndex;
        playedIndexes.Add(randomIndex);

        musicSource.clip = musicTracks[randomIndex];
        musicSource.Play();
    }

    private System.Collections.IEnumerator CheckMusicEnd()
    {
        while (true)
        {
            if (!musicSource.isPlaying)
            {
                yield return new WaitForSeconds(1f);
                PlayRandomMusic();
            }
            yield return null;
        }
    }
}
