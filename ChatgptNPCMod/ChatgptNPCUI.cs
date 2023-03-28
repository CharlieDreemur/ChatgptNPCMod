using BepInEx;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using static UnityEngine.TouchScreenKeyboard;

namespace ChatgptNPCMod
{
    public class ChatgptNPCUI:MonoBehaviour
    {
        public static Text status;

        private static GameObject _instance;
        //The instanitated UI gameObject instance
        public static GameObject instance
        {
            get
            {
                ChatgptNPC.MyLogger.LogInfo("Try to get UI");
                if (_instance == null)
                {
                    _instance = ChatgptNPCUI.LoadUI();
                }
                
                return _instance;
            }

        }
        public static AssetBundle LoadAssetBundle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string dllFolderPath = Path.GetDirectoryName(assembly.Location);
            ChatgptNPC.MyLogger.LogInfo("Current DLL Path: " + dllFolderPath);
            var ab = AssetBundle.LoadFromFile($"{dllFolderPath}/chatgptnpc");
            if(ab == null) { Debug.LogError($"没有加载AssetBudle:chatgptnpc"); }
            return ab;
        }
        public static GameObject LoadUI()
        {
 
            GameObject uiPrefab = null;
            GameObject uiObj = null;
            AssetBundle ab = LoadAssetBundle();
            if (ab != null)
            {
                uiPrefab = ab.LoadAsset<GameObject>("ChatgptNPCInputField");
                if (uiPrefab != null)
                {
                    uiObj = InstantiateUI(uiPrefab);
                }
                else
                {
                    ChatgptNPC.MyLogger.LogError("uiPrefab is null");
                }
            }
            BindUI(uiObj);
            return uiObj;
        }

        public static GameObject InstantiateUI(GameObject prefab)
        {
            GameObject obj = GameObject.Instantiate(prefab, UnityEngine.Object.FindObjectOfType<NewUICanvas>().transform);
            obj.transform.localPosition = new Vector3(-15, 305, 0); //这个位置比较好，可以随便改
            return obj;
        }
        public static void BindUI(GameObject ui)
        {
            //Make UI draggable
            ui.AddComponent<DraggablePanel>();
            Button submitBtn = ui.transform.Find("SubmitBtn").GetComponent<Button>();
            InputField inputField = ui.transform.Find("InputField").GetComponent<InputField>();
            status = ui.transform.Find("Status").GetComponent<Text>();
            submitBtn.onClick.AddListener(async () =>
            {
                submitBtn.gameObject.SetActive(false);
                status.gameObject.SetActive(true);
                ChatgptNPC.messages.Add(new ChatgptNPC.Message("user", inputField.text));
                string message = ChatgptNPC.MessagesToString(ChatgptNPC.messages);
                status.StartCoroutine(UpdateStatusText());

                string result = await ChatgptNPCAPI.GetParseMessage(message);
                result = ChatgptNPC.CleanString(result);
                ChatgptNPC.messages.Add(new ChatgptNPC.Message("assitant", result));
                ChatgptNPC.Say(result);
                submitBtn.gameObject.SetActive(true);
                status.gameObject.SetActive(false);
            });
        }

        private static IEnumerator UpdateStatusText()
        {
            while (true)
            {
                status.text = "思考中.";
                yield return new WaitForSeconds(0.5f);
                status.text = "思考中..";
                yield return new WaitForSeconds(0.5f);
                status.text = "思考中...";
                yield return new WaitForSeconds(0.5f);
            }
        }

    }
}
