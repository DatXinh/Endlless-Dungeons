using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteListManager : MonoBehaviour
{
    public GameObject noteItemPrefab;
    public Transform contentParent; // Content trong ScrollView
    public List<NoteInfo> allNotes;

    public void Start()
    {
        PopulateNoteList();
    }

    private void PopulateNoteList()
    {
        foreach (NoteInfo note in allNotes)
        {
            GameObject item = Instantiate(noteItemPrefab, contentParent);
            // Gán các thành phần UI
            var icon = item.transform.Find("Icon").GetComponent<Image>();
            var nameText = item.transform.Find("Name").GetComponent<TMP_Text>();
            var descText = item.transform.Find("Info").GetComponent<TMP_Text>();
            // Gán dữ liệu
            icon.sprite = note.noteIcon;
            nameText.text = note.noteTitle;
            descText.text = note.noteDescription;
        }
    }
}
