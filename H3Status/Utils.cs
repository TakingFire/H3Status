using System.IO;
using System.Collections.Generic;
using UnityEngine;
using FistVR;

namespace H3Status.Utils
{

    internal static class AmmoReader
    {
        public static void GetAmmo(string path)
        {
            Plugin.Logger.LogInfo("WRITING FILE");
            StreamWriter writer = new StreamWriter(path, false);

            try {
                ManagerSingleton<AM>.Instance.GenerateFireArmRoundDictionaries();
            }
            catch {
                Plugin.Logger.LogInfo("TypeDict already generated");
            }

            foreach(var roundType in ManagerSingleton<AM>.Instance.TypeDic)
            {
                foreach(var roundClass in roundType.Value)
                {
                    string output = roundType.Key + "," + roundClass.Key + "," + roundClass.Value.Mesh.name;
                    Plugin.Logger.LogInfo(output);
                    writer.WriteLine(output);
                }
            }
            writer.Flush();
            writer.Close();
            Plugin.Logger.LogInfo("DONE");
        }

        public static void GetShells(string path)
        {
            Plugin.Logger.LogInfo("WRITING FILE");
            StreamWriter writer = new StreamWriter(path, false);

            foreach (var roundType in ManagerSingleton<AM>.Instance.TypeList)
            {
                GameObject gameObject = AM.GetRoundSelfPrefab(roundType, AM.GetDefaultRoundClass(roundType)).GetGameObject();
                FVRFireArmRound round = gameObject.GetComponent<FVRFireArmRound>();
                if (round != null)
                {
                    round.Fire();

                    if (round != null && round.FiredRenderer != null)
                    {
                        string output = roundType + "," + round.FiredRenderer.gameObject.GetComponent<MeshFilter>().sharedMesh.name;
                        Plugin.Logger.LogInfo(output);
                        writer.WriteLine(output);
                    }
                }
            }

            writer.Flush();
            writer.Close();
            Plugin.Logger.LogInfo("DONE");
        }
    }

    internal static class WeaponReader
    {
        public static void GetWeapons(string path, ItemSpawnerV2 spawner)
        {
            List<FVRObject> list = ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Firearm];
            int initialSize = list.Count;
            for (int i = 0; i < list.Count; i++)
            {
                bool isValid = false;

                FVRObject item = list[i];
                if (item.OSple)
                {
                    if (item.SpawnedFromId != null && !(item.SpawnedFromId == string.Empty))
                    {
                        if (IM.HasSpawnedID(item.SpawnedFromId))
                        {
                            if (IM.GetSpawnerID(item.SpawnedFromId).IsDisplayedInMainEntry)
                            {
                                isValid = true;
                            }
                        }
                    }
                }

                if (!isValid) {
                    list.RemoveAt(i--);
                }
            }

            Plugin.Logger.LogInfo($"Found {list.Count} guns ({initialSize - list.Count} excluded)");
            Plugin.Logger.LogInfo("WRITING FILES");

            var blacklist = new List<string>() {"Cube", "Sphere", "Capsule", "Quad"};

            foreach(var weapon in list)
            {
                string weaponName = IM.GetSpawnerID(weapon.SpawnedFromId).ItemID;
                Plugin.Logger.LogInfo(weaponName);

                GameObject go = UnityEngine.Object.Instantiate<GameObject>(weapon.GetGameObject(), Vector3.zero, Quaternion.identity);
                go.SetActive(false);

                StreamWriter writer = new StreamWriter(path + $"/{weaponName}.csv", false);

                MeshFilter[] comps = go.GetComponentsInChildren<MeshFilter>(false);

                foreach(var c in comps)
                {
                    if (c.sharedMesh != null)
                    {
                        string meshName = c.sharedMesh.name;
                        if (blacklist.Contains(meshName)) continue;

                        string output =
                            c.sharedMesh.name + "," +
                            c.transform.position.x.ToString("F6") + "," +
                            c.transform.position.y.ToString("F6") + "," +
                            c.transform.position.z.ToString("F6") + "," +
                            c.transform.rotation.x.ToString("F6") + "," +
                            c.transform.rotation.y.ToString("F6") + "," +
                            c.transform.rotation.z.ToString("F6") + "," +
                            c.transform.localScale.x.ToString("F6") + "," +
                            c.transform.localScale.y.ToString("F6") + "," +
                            c.transform.localScale.z.ToString("F6");

                        writer.WriteLine(output);
                    }
                }

                Object.Destroy(go);
                writer.Flush();
                writer.Close();
            }

            Plugin.Logger.LogInfo("DONE");
        }
    }

}
