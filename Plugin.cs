using System;
using System.IO;
using System.Reflection;
using BepInEx;
using ModTemp;
using Photon.Voice;
using Unity.Mathematics;
using UnityEngine;
using Utilla;
using ModTemp.Tools;
using UnityEngine.UIElements;

namespace ModTemp
{
    [ModdedGamemode]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {


        public bool active;
        bool inRoom;

        void Start()
        {
            Utilla.Events.GameInitialized += OnGameInitialized;
        }

        void OnEnable()
        {
            if (inRoom)
            {
                AssetObj.SetActive(true);
            }
            active = true;

            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            AssetObj.SetActive(false);

            speedActive = false;




            active = false;

            HarmonyPatches.RemoveHarmonyPatches();
        }
        public GameObject AssetObj;
        void OnGameInitialized(object sender, EventArgs e)
        {
            var assetBundle = LoadAssetBundle("ModTemp.galaxys");
            GameObject Obj = assetBundle.LoadAsset<GameObject>("SphereObject");

            AssetObj = Instantiate(Obj);


            AssetObj.transform.position = new Vector3(-63.5543f, 3.2236f, - 63.5632f);
            AssetObj.transform.rotation = Quaternion.Euler(90f, 177.715f, 0f);
            AssetObj.SetActive(false);

        }
        public bool speedActive = false;
        void Update()
        {
            if (AssetObj != null && GorillaLocomotion.Player.Instance.bodyCollider.transform.position.Distance(AssetObj.transform.position) < 2.5f)
            {
                speedActive = true;
            }
            if (speedActive && inRoom)
            {
                GorillaLocomotion.Player.Instance.jumpMultiplier = 1.8f;
                GorillaLocomotion.Player.Instance.maxJumpSpeed = 9.8f;
            }
        } 
           
        

        AssetBundle LoadAssetBundle(string path)
        {
            try
            {
                Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
                AssetBundle bundle = AssetBundle.LoadFromStream(stream);
                stream.Close();
                Debug.Log("[" + PluginInfo.GUID + "] Success loading asset bundle");
                return bundle;
            }
            catch (Exception e)
            {
                Debug.Log("[" + PluginInfo.Name + "] Error loading asset bundle: " + e.Message + " " + path);
                throw;
            }
        }

        [ModdedGamemodeJoin]

        public void OnJoin(string gamemode)
        {
            if (active)
            {
                AssetObj.SetActive(true);
            }

            inRoom = true;
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {
            AssetObj.SetActive(false);

            speedActive = false;


            inRoom = false;
        }
    }
}
