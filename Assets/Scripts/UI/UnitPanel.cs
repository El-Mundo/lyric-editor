using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitPanel : MonoBehaviour
{
    [HideInInspector]
    public Unit unit;
    public bool isOn;

    private bool canUpdate = true;

    public InputField lyric, notation, channel, midi;
    public Image highlight;
    public ChunkPanel parent;

    public void SetUnit(Unit unit)
    {
        if(unit == null)
        {
            DisableSelf();
        }
        else
        {
            canUpdate = false;

            this.unit = unit;
            this.isOn = true;
            gameObject.SetActive(true);

            lyric.text = unit.content;
            notation.text = unit.notation;
            channel.text = unit.channel + "";
            midi.text = NotesToMIDICode(unit.notes);

            canUpdate = true;
        }
    }

    public void Highlight(bool isOn)
    {
        highlight.enabled = isOn;
    }

    private void DisableSelf()
    {
        isOn = false;
        gameObject.SetActive(false);
    }

    public void DeleteSelf()
    {
        if (!parent.IsPreserved(this))
        {
            DisableSelf();
        }
        else
        {
            //LANGUAGE
            GameManager.instance.Alert("Cannot delete the preserved Unit of a Chunk!");
        }
    }

    public void ChangeLyric()
    {
        if (!canUpdate) return;
        unit.content = lyric.text;
    }

    public void ChangeNotation()
    {
        if (!canUpdate) return;
        unit.notation = notation.text;
    }

    public void ChangeMIDI()
    {
        if (!canUpdate) return;
        Note[] notes = MIDICodeToNotes(midi.text);
        if(notes != null)
        {
            unit.notes = notes;
        }
        else
        {
            midi.text = NotesToMIDICode(unit.notes);
        }
    }

    public void ChangeChannel()
    {
        if (!canUpdate) return;
        try
        {
            int chn = 0;
            bool parsed = int.TryParse(channel.text, out chn);

            if(!parsed) throw new System.Exception("Input is not a number.");
            if (chn < 0 || chn > 4) throw new System.Exception("Channel number should be within 0-4.");
            if(parent.CheckChannelOccupancy(chn)) throw new System.Exception("The channel has been occupied.");

            unit.channel = chn;
        }
        catch(System.Exception e)
        {
            channel.text = unit.channel + "";
            //LANGUAGE
            GameManager.instance.Alert("Cannot modyfy the channel.\n" + e.Message);
        }
    }

    private static readonly string[] SPLIT = {";" };

    /// <summary>
    /// Convert MIDI code in editor to Note objects.
    /// </summary>
    /// <param name="code">Format: Value,Vocal,Duration;Value,Vocal,Duration;...
    /// (e.g. c3,duo,1.0;d#8,lai,2.0)</param>
    public static Note[] MIDICodeToNotes(string code)
    {
        string errorCode = "";
        try
        {
            code = code.Trim();
            string[] notes = code.Split(SPLIT, System.StringSplitOptions.RemoveEmptyEntries);
            Note[] output = new Note[notes.Length];
            int i = 0;
            foreach(string n in notes)
            {
                errorCode = n;

                string[] sp = n.Split(',');
                int key = NoteToMIDIValue(sp[0]);
                string vocal = sp[1];
                float duration = float.Parse(sp[2]);

                output[i] = new Note(duration, key, vocal);
                i++;
            }

            return output;
        }
        catch
        {
            //LANGUAGE
            GameManager.instance.Alert(string.IsNullOrEmpty(errorCode) ? "Wrong format, please use ASCII characters.\n" : "Invalid MIDI code:\n" + errorCode);
            return null;
        }
    }

    public static string NotesToMIDICode(Note[] notes)
    {
        try
        {
            string output = "";
            int i = 0;
            foreach (Note n in notes)
            {
                string note = MIDIValueToNoteString(n.key) + "," + n.vocal + "," + n.duration;
                output += note + (i == notes.Length - 1 ? "" : ";");
                i++;
            }

            return output;
        }
        catch
        {
            return "";
        }
    }

    private static int NoteToMIDIValue(string note)
    {
        int prog = int.Parse(note.Substring(note.Length - 1, 1));
        int no = 0;

        note = note.Substring(0, note.Length - 1);
        note = note.ToLower();

        switch (note)
        {
            case "c":
                no = 0;
                break;
            case "c#":
                no = 1;
                break;
            case "d":
                no = 2;
                break;
            case "d#":
                no = 3;
                break;
            case "e":
                no = 4;
                break;
            case "f":
                no = 5;
                break;
            case "f#":
                no = 6;
                break;
            case "g":
                no = 7;
                break;
            case "g#":
                no = 8;
                break;
            case "a":
                no = 9;
                break;
            case "a#":
                no = 10;
                break;
            case "b":
                no = 11;
                break;

        }

        return 12 + prog * 12 + no;
    }

    private static string MIDIValueToNoteString(int value)
    {
        string note = "";
        int no = value % 12;
        int prog = value / 12 - 1;

        switch (no)
        {
            case 0:
                note = "c";
                break;
            case 1:
                note = "c#";
                break;
            case 2:
                note = "d";
                break;
            case 3:
                note = "d#";
                break;
            case 4:
                note = "e";
                break;
            case 5:
                note = "f";
                break;
            case 6:
                note = "f#";
                break;
            case 7:
                note = "g";
                break;
            case 8:
                note = "g#";
                break;
            case 9:
                note = "a";
                break;
            case 10:
                note = "a#";
                break;
            case 11:
                note = "b";
                break;

        }

        return note + prog;
    }

}