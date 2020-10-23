using HutongGames.PlayMaker.Actions;
using System.Linq;
using UnityEngine;

namespace JailTV2
{
    public class TelevisionBehaviour : MonoBehaviour
    {
        GameObject TVHome;
        GameObject TVPrograms;
        PlayMakerFSM TVsimulation;
        public Collider OnOffSwitchTrigger;
        public float PlayerDistanceToTV;
        public float MinDistanceToTV = 10f;
        public GameObject TVscreen;

        public Mesh RemoteMesh;
        GameObject Remote;

        bool IsTVactive = false;

        Transform RallyTVsound;
        Transform OGrallyTVsoundParent;
        Vector3 OGrallyTVsoundPOS = new Vector3(-4.698f, 0.1f, 5.741f);

        public RaycastHit hitInfo;
        public int layerMask;

        GameObject PLAYER;

        void Start()
        {
            TVHome = GameObject.Find("YARD").transform.Find("Building/Dynamics/HouseElectricity/ElectricAppliances").gameObject;
            TVPrograms = TVHome.transform.Find("TV_Programs").gameObject;

            PLAYER = GameObject.Find("PLAYER");

            hitInfo = new RaycastHit();

            layerMask = LayerMask.GetMask("Dashboard");

            GameObject Simu = GameObject.Instantiate(GameObject.Find("YARD").transform.Find("Building/LIVINGROOM/TV/Switch").gameObject);
            Simu.name = "TVsimulation";
            TVsimulation = Simu.GetComponent<PlayMakerFSM>();
            Simu.transform.SetParent(transform, false);
            Simu.GetComponent<SphereCollider>().enabled = false;

            (TVsimulation.FsmStates.FirstOrDefault(state => state.Name == "Close TV 2").Actions[11] as BoolTest).boolVariable = true;

            OnOffSwitchTrigger.gameObject.layer = layerMask;
            TVsimulation.FsmVariables.GetFsmGameObject("Screen").Value = TVscreen;

            Remote = GameObject.Instantiate(GameObject.Find("tv remote control(itemx)"));
            Remote.name = "tv remote control(itemx)";
            Remote.transform.position = new Vector3(-656.2231f, 4.854005f, -1152.323f);
            Remote.transform.eulerAngles = new Vector3(0f, 160f, 0f);
            Remote.transform.GetChild(0).GetComponent<MeshFilter>().mesh = RemoteMesh;
            Remote.transform.GetChild(0).GetComponent<MeshRenderer>().material = transform.parent.Find("mesh/TV_HOUSING").GetComponent<MeshRenderer>().material;
            Remote.transform.SetParent(transform.root);
            PlayMakerFSM RemoteFSM = Remote.GetComponent<PlayMakerFSM>();
            (RemoteFSM.FsmStates.FirstOrDefault(State => State.Name == "Close").Actions[2] as SetMaterial).gameObject.GameObject = TVscreen;
            (RemoteFSM.FsmStates.FirstOrDefault(State => State.Name == "Open").Actions[2] as SetMaterial).gameObject.GameObject = TVscreen;

            TVsimulation.FsmVariables.GetFsmGameObject("Remote").Value = Remote;

            OGrallyTVsoundParent = GameObject.Find("RALLY").transform.Find("RallyTV");
            RallyTVsound = OGrallyTVsoundParent.Find("TVsound");

            EnableDisable(false);
        }

        void Update()
        {
            PlayerDistanceToTV = Vector3.Distance(PLAYER.transform.position, transform.position);

            if (Camera.main != null) Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, 1.35f, layerMask);

            if (hitInfo.collider == OnOffSwitchTrigger)
            {
                PlayMakerGlobals.Instance.Variables.FindFsmBool("GUIuse").Value = true;

                if (Input.GetMouseButtonDown(0))
                {
                    IsTVactive = !IsTVactive;
                }
            }

            if (IsTVactive)
            {
                if (PlayerDistanceToTV >= MinDistanceToTV)
                {
                    if (TVsimulation.FsmVariables.GetFsmBool("Open").Value)
                    {
                        IsTVactive = !IsTVactive;
                        EnableDisable(false);
                    }
                }
                else
                {
                    if (!TVsimulation.FsmVariables.GetFsmBool("Open").Value)
                    {
                        EnableDisable(true);
                    }
                }
            }
            else
            {
                if (TVsimulation.FsmVariables.GetFsmBool("Open").Value)
                {
                    EnableDisable(false);
                }
            }
        }

        void EnableDisable(bool Activation)
        {
            if (Activation)
            {
                RallyTVsound.SetParent(transform, false);
                RallyTVsound.transform.localPosition = Vector3.zero;
                TVPrograms.transform.SetParent(transform, false);
                TVPrograms.transform.localPosition = new Vector3(0.7f, 0f, 0f);
                TVPrograms.transform.localEulerAngles = Vector3.zero;
                TVsimulation.FsmVariables.GetFsmBool("Open").Value = Activation;
            }
            else
            {
                TVsimulation.SendEvent("ELEC_CUTOFF");
                TVPrograms.transform.SetParent(TVHome.transform, false);
                RallyTVsound.SetParent(OGrallyTVsoundParent, false);
                RallyTVsound.localPosition = OGrallyTVsoundPOS;
            }
        }
    }
}