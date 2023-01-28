using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence 
{
    void LoadData(ObjectData data);
    void SaveData(ref AppData data);
}
