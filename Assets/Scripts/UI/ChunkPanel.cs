using Lyrics.Songs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPanel : MonoBehaviour
{
    public UnitPanel[] units;
    public UnitPanel preservedUnit;
    private VirtualChunk chunkToUpdate;

    /// <summary>
    /// Checks whether a channel has been occupied.
    /// </summary>
    /// <returns>Return true if the channel is occupied</returns>
    public bool CheckChannelOccupancy(int chn)
    {
        foreach(UnitPanel up in units)
        {
            if (up.unit != null && up.isOn)
            {
                if (up.unit.channel == chn) return true;
            }
        }
        return false;
    }

    public bool IsPreserved(UnitPanel unit)
    {
        return unit.Equals(preservedUnit);
    }

    public void SetChunk(VirtualChunk chunk)
    {
        chunkToUpdate = chunk;
        gameObject.SetActive(true);
        foreach(UnitPanel u in units)
        {
            u.SetUnit(null);
        }
        SetUnit(chunk.chunk.unit, true);
        int pre = chunk.chunk.unit.channel;
        if(chunk.chunk.alterUnits != null) {
            foreach (Unit u in chunk.chunk.alterUnits)
            {
                Debug.Log(u.channel != pre);
                if (u.channel != pre)
                {
                    SetUnit(u, false);
                }
            }
        }
    }

    public void ExitPanel()
    {
        UpdateChunk();
        gameObject.SetActive(false);
    }

    public void UpdateChunk()
    {
        if(chunkToUpdate != null)
        {
            chunkToUpdate.chunk.unit = preservedUnit.unit;
            List<Unit> newAlter = new List<Unit>();
            foreach(UnitPanel u in units)
            {
                if(u.unit != null && u.isOn)
                {
                    if (IsPreserved(u)) continue;

                    newAlter.Add(u.unit);
                }
            }
            chunkToUpdate.chunk.alterUnits = newAlter.ToArray();
            chunkToUpdate.UpdateDisplay();
        }
    }

    private void SetUnit(Unit unit, bool preserve)
    {
        int channel = unit.channel;
        if(channel >= 0 && channel <= 4 && unit != null)
        {
            units[channel].SetUnit(unit);
            units[channel].Highlight(false);
            if (preserve)
            {
                preservedUnit = units[channel];
                units[channel].Highlight(true);
            }
        }
    }

    public void AddUnit()
    {
        foreach (UnitPanel up in units)
        {
            if (!up.isOn && !IsPreserved(up))
            {
                int index = GetFreeNumber();
                if (index < 0) break;
                up.SetUnit(new Unit(index));
                up.Highlight(false);
                return;
            }
        }
        GameManager.instance.Alert("All 5 units of this chunk have been occupied.");
    }

    private int GetFreeNumber()
    {
        for(int i = 0; i < 5; i++)
        {
            if (!CheckChannelOccupancy(i))
            {
                return i;
            }
        }
        return -1;
    }

}
