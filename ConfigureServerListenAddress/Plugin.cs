using System.Reflection;
using System.Runtime.CompilerServices;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using SpaceCraft;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports;
using Unity.Netcode.Transports.UTP;
using System.ComponentModel;
using Netcode.Transports;
using System.Text;
using System.IO;
using System.Collections;
using System;
using System.Diagnostics.CodeAnalysis;

namespace ConfigureServerListenAddress
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource logger;
        private static ConfigEntry<string> serverListenAddress;

        private void Awake()
        {
            Plugin.logger = Logger;
            serverListenAddress = Config.Bind("General", "ServerListenAddress", "::", "The server listen address. Default is '::' to bind to all IPv6 addresses.)");

            Harmony harmony = new($"{PluginInfo.PLUGIN_GUID}.{PluginInfo.PLUGIN_NAME}.{PluginInfo.PLUGIN_VERSION}");
            harmony.Patch(
                original: AccessTools.Method("Unity.Netcode.NetworkManager:StartHost"),
                prefix: new HarmonyMethod(typeof(Plugin), nameof(StartHost_Prefix))
            );

            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public static void StartHost_Prefix(Unity.Netcode.NetworkManager __instance)
        {
            __instance.GetComponent<UnityTransport>().ConnectionData.ServerListenAddress = serverListenAddress.Value;
        }
    }
}
