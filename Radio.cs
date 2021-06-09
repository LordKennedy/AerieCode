using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radio : MonoBehaviour {

    [SerializeField] private AudioClip[] songs;
    private AudioSource radioPlayer;

    // Start is called before the first frame update
    void Start() {
        radioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        if (!radioPlayer.isPlaying) {
            // Pick a random song to play
            int toPlay = Random.Range(1, songs.Length);
            // Move it to index 0
            AudioClip tmp = songs[0];
            songs[0] = songs[toPlay];
            songs[toPlay] = tmp;
            // Play it
            radioPlayer.PlayOneShot(songs[0]);
        }
    }
}
