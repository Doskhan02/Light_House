using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistance
{
    void LoadData(GamePesistantData data);

    void SaveData(ref GamePesistantData data);
}
