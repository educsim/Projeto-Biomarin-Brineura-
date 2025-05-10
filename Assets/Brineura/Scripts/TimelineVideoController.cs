using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class TimelineVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    private PlayableDirector playableDirector;

    public List<VideoClip> clipList = new List<VideoClip>();

    private double startTime;
    private bool isPlaying = false;
    private bool waitingToSeekFrame = false;
    private bool seekReady = false;

    private int frameRate;

    private void Start()
    {
        playableDirector = GetComponentInParent<PlayableDirector>();

        videoPlayer.seekCompleted += SeekCompleted;
    }

    private void SeekCompleted(VideoPlayer source)
    {
        seekReady = true;
    }

    public void PlayVideoClip(int index)
    {
        if (index < 0 || index >= clipList.Count) return;

        videoPlayer.clip = clipList[index];
        videoPlayer.Play();
        isPlaying = true;
        startTime = playableDirector.time;

        // Obter a taxa de quadros do vídeo
        frameRate = Mathf.RoundToInt((float)videoPlayer.frameRate);
    }

    public void ChangeTime(double time)
    {
        if (!isPlaying) return;
        if (waitingToSeekFrame) return;
        StartCoroutine(SetTimeCoroutine(time));
    }

    public void PauseVideo()
    {
        if (!isPlaying) return;
        videoPlayer.Pause();
    }

    public void PlayVideo()
    {
        if (!isPlaying) return;
        videoPlayer.Play();
    }

    public IEnumerator SetTimeCoroutine(double time)
    {
        waitingToSeekFrame = true;
        seekReady = false;
        videoPlayer.time = time - startTime;
        while (waitingToSeekFrame)
        {
            if (seekReady)
            {
                waitingToSeekFrame = false;
                StopAllCoroutines();
            }
            yield return null;
        }
        yield return null;
    }

}