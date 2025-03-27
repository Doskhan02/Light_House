using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    void LoadData(GamePersistantData data);

    void SaveData(ref GamePersistantData data);
}
