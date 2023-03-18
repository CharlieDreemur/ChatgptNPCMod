using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using Fungus;
using UnityEngine;
using HarmonyLib;
using BepInEx.Logging;
using UnityEngine.Events;
using System.IO;
using System.Reflection;
using System.Collections;
using UnityEngine.UI;

namespace ChatgptNPCMod
{
    [BepInPlugin("CharlieDreemur.plugin.ChatgptNPCMod", "ChargptNPC", "0.0.1")]
    public class ChatgptNPC : BaseUnityPlugin
    {
        public static List<MenuDialog> menuDialogs = new List<MenuDialog>();
        private ConfigEntry<KeyCode> hotkey;
        private static bool isShowUI = false;
        private Rect _windowRect = new Rect(20, 20, 250, 100);
        private string _inputText = string.Empty;
        private string _displayText = string.Empty;
        public static ManualLogSource MyLogger;
        private void Start()
        {
            hotkey = Config.Bind<KeyCode>("config", "hotkey",KeyCode.Z, "快捷键");
            Harmony.CreateAndPatchAll(typeof(ChatgptNPC));
            MyLogger = Logger;
            MyLogger.LogInfo("ChatgptNPC.testMessage BepInPlugin");
        }

        private void Update()
        {
            if (Input.GetKeyDown(hotkey.Value))
            {
                Logger.LogInfo("ChatgptNPC.testMessage BepInPlugin");
                isShowUI = !isShowUI;
                ChatgptNPCUI.instance.SetActive(isShowUI);
                
            }
        }



        [HarmonyPostfix, HarmonyPatch(typeof(UINPCJiaoHu), "ShowJiaoHuPop")]
        public static void ShowUI()
        {
            MyLogger.LogInfo("ShowUI()");
            ChatgptNPCUI.instance.SetActive(true);

        }
        [HarmonyPostfix, HarmonyPatch(typeof(UINPCJiaoHu), "HideJiaoHuPop")]
        public static void HideUI()
        {
            MyLogger.LogInfo("HideUI()");
            ChatgptNPCUI.instance.SetActive(false);
        }
        public static void Say(string text)
        {
            Tools.Say(text, UINPCJiaoHu.Inst.NowJiaoHuNPC.ID);
        }

        public static void AddOption(string str, Action action)
        {

            MenuDialog menuDialog = MenuDialog.GetMenuDialog();
            menuDialog.SetActive(true);
            menuDialogs.Add(menuDialog);
            var method = Traverse.Create(menuDialog).Method("AddOption", new object[] { str, true, false, new UnityAction(action + (() => DestroyMenu())) }) ;
            bool result = method.GetValue<bool>();

            //method.Invoke(menuDialog, new object[] { str, true, false, new UnityAction(action + (() => DestroyMenu())) });
        }

        static void DestroyMenu()
        {
            foreach (MenuDialog menuDialog in menuDialogs)
                UIDrawCall.Destroy(menuDialog.gameObject);
            menuDialogs.Clear();
        }

        
    }
}

