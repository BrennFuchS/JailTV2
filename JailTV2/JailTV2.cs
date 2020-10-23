using MSCLoader;
using UnityEngine;

namespace JailTV2
{
    public class JailTV2 : Mod
    {
        public override string ID => "JailTV2";
        public override string Name => "JailTV2";
        public override string Author => "BrennFuchS";
        public override string Version => "1.0";

        public override bool UseAssetsFolder => true;

        public override void OnLoad()
        {
            AssetBundle ab = LoadAssets.LoadBundle(this, "jailtv");
            GameObject TV = GameObject.Instantiate(ab.LoadAsset<GameObject>("TV.prefab"));
            TV.transform.position = new Vector3(-656.575f, 4.8975f, -1152.41f);
            TV.transform.eulerAngles = new Vector3(270f, 0f, 0f);
            TV.transform.localScale = Vector3.one;

            PlayMakerFSM FSM = GameObject.Find("Systems").transform.Find("PlayerWanted").GetComponent<PlayMakerFSM>();
            TV.transform.SetParent(FSM.FsmVariables.GetFsmGameObject("Spawn").Value.transform.root);
        }
    }
}
