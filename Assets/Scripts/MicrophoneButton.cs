﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class MicrophoneButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField]
    private Sprite m_OnPressedSprite;

    private AudioClip m_RecordedAudio;

    private int m_MaxMicrophoneFrequency;

    public void OnPointerDown(PointerEventData eventData)
    {
        GetComponent<Image>().sprite = m_OnPressedSprite;
        m_RecordedAudio = Microphone.Start("", false, 60, m_MaxMicrophoneFrequency);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        transform.GetChild(0).gameObject.SetActive(true);

        // Start clip creation
        int micDuration = Microphone.GetPosition("");
        Microphone.End("");
        float[] samples = new float[m_RecordedAudio.samples];
        m_RecordedAudio.GetData(samples, 0);
        float[] clipSamples = new float[micDuration];
        System.Array.Copy(samples, clipSamples, clipSamples.Length - 1);
        var clip = AudioClip.Create("playRecordClip", clipSamples.Length, 1, m_MaxMicrophoneFrequency, false);
        clip.SetData(clipSamples, 0);
        // End clip creation

        var bytes = WavUtility.FromAudioClip(clip);

        PhotonNetwork.RaiseEvent((byte)EventCodes.SetRoles, player_Role, raiseEventOptions, SendOptions.SendReliable);
    }

    void Start()
    {
        Microphone.GetDeviceCaps("", out var minFreq, out m_MaxMicrophoneFrequency);
    }

}
