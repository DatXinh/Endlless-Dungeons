using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TalkOnlyNPC : MonoBehaviour, IInteractable
{
    public TMP_Text text;
    public List<String> chat;
    public string GetInteractionPrompt()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        if (chat.Count <= 1)
            return;
        int chatIndex = UnityEngine.Random.Range(1, chat.Count);
        text.text = chat[chatIndex];
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            text.enabled = true;
            text.text = chat[0];
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            text.enabled = false;
        }
    }
}
