using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using VRC;
using VRC.Core;

using PiShockVRC.Config;

namespace PiShockVRC.Core
{
    public static class AvatarManager
    {
        private static readonly float FixedUpdateRate = 0.1f;
        private static readonly string PiShockApi = "https://do.pishock.com/api/apioperate";
        private static readonly List<PiShockDevice> Devices = new List<PiShockDevice>();
        private static readonly HttpClient WebHandler = new HttpClient();

        private static float elapsedTime = 0f;

        public static void Init()
        {
            NetworkEvents.OnAvatarInstantiated += OnAvatarReady;
        }

        private static void SendAPIRequest(PiShockDevice device, PiShockPoint point, string user)
        {
            string request = "{\"Username\":\"" + Configuration.Username.Value
                    + "\",\"Apikey\":\"" + Configuration.ApiKey.Value
                    + "\",\"Name\":\"" + "[VRChat] " + user
                    + "\",\"Code\":\"" + device.Link.ShareCode
                    + "\",\"Intensity\":\"" + point.Strength
                    + "\",\"Duration\":\"" + point.Duration
                    + "\",\"Op\":\"" + (int)point.Type + "\"}";

            if (Configuration.LogApiRequests.Value)
                PiShockVRCMod.Logger.Msg("[PiShock API] Sending => " + request);

            Task.Run(async () =>
            {
                await WebHandler.PostAsync(PiShockApi, new StringContent(request, System.Text.Encoding.UTF8, "application/json"));
            }).ContinueWith(t =>
            {
                if (t.IsFaulted)
                    PiShockVRCMod.Logger.Error("Task raised exception: " + t.Exception);
            });
        }

        private static void OnAvatarReady(VRCAvatarManager avatarManager, ApiAvatar apiAvatar, GameObject gameObject)
        {
            if (avatarManager.field_Private_VRCPlayer_0.prop_String_3 == APIUser.CurrentUser.id)
            {
                PiShockVRCMod.Logger.Msg("Local avatar instantiated. Scanning for PiShock objects...");

                Devices.Clear();

                FindPoints(VRCPlayer.field_Internal_Static_VRCPlayer_0.field_Private_VRCAvatarManager_0.field_Private_GameObject_0.transform);
                if (Devices.Count == 0)
                    PiShockVRCMod.Logger.Msg("No valid PiShockPoints were found on this avatar.");
            }
        }

        public static void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime < FixedUpdateRate)
                return;
            else
                elapsedTime = 0;

            if (!Configuration.Enabled.Value || Devices.Count == 0)
                return;

            foreach (Player player in PlayerManager.field_Private_Static_PlayerManager_0.field_Private_List_1_Player_0)
            {
                if (player == null || player.field_Private_APIUser_0 == null
                    || (!Configuration.SelfInteraction.Value && player.field_Private_APIUser_0.id.Equals(APIUser.CurrentUser.id))
                    || (!player.field_Private_APIUser_0.id.Equals(APIUser.CurrentUser.id) && Configuration.FriendsOnly.Value && !player.field_Private_APIUser_0.isFriend))
                    continue;

                Animator animator = player.Method_Internal_VRCPlayer_0()?.field_Internal_Animator_0;

                if (animator == null || !animator.isHuman)
                    continue;

                Vector3 leftHand = animator.GetBoneTransform(HumanBodyBones.LeftHand)?.position ?? Vector3.positiveInfinity;
                Vector3 rightHand = animator.GetBoneTransform(HumanBodyBones.RightHand)?.position ?? Vector3.positiveInfinity;
                Vector3 leftFoot = animator.GetBoneTransform(HumanBodyBones.LeftFoot)?.position ?? Vector3.positiveInfinity;
                Vector3 rightFoot = animator.GetBoneTransform(HumanBodyBones.RightFoot)?.position ?? Vector3.positiveInfinity;

                foreach (PiShockDevice device in Devices)
                {
                    if (device.NextValidShock >= Time.realtimeSinceStartup)
                        continue;

                    foreach (PiShockPoint point in device.Points)
                    {
                        if (point.Object == null || !point.Object.activeSelf)
                            continue;

                        List<float> distances = new List<float>();

                        distances.Add(Vector3.Distance(leftHand, point.Object?.transform.position ?? Vector3.positiveInfinity));
                        distances.Add(Vector3.Distance(rightHand, point.Object?.transform.position ?? Vector3.positiveInfinity));

                        if (Configuration.FeetInteraction.Value)
                        {
                            distances.Add(Vector3.Distance(leftFoot, point.Object?.transform.position ?? Vector3.positiveInfinity));
                            distances.Add(Vector3.Distance(rightFoot, point.Object?.transform.position ?? Vector3.positiveInfinity));
                        }

                        float activationDistance = point.Radius <= 0 ? Configuration.DefaultRadius.Value : point.Radius;

                        if (distances.Any(distance => distance <= activationDistance))
                        {
                            device.NextValidShock = Time.realtimeSinceStartup + point.Duration;
                            SendAPIRequest(device, point, player?.field_Private_APIUser_0?.displayName ?? "Invalid Name");
                            break;
                        }
                    }
                }
            }
        }

        private static void FindPoints(Transform transform)
        {
            if (transform.name.StartsWith("PiShockPoint"))
            {
                transform = transform.Find("Settings");

                string identifier = transform.GetChild(0).name.Split(':')[1].Trim();

                PiShockDevice device = Devices.Where(d => d.Name.Equals(identifier)).DefaultIfEmpty(null).First();
                if (device == null)
                {
                    if (Configuration.DeviceLinks.TryGetValue(identifier, out PiShockDevice.LinkData linkData))
                    {
                        if (string.IsNullOrEmpty(linkData.ShareCode))
                        {
                            PiShockVRCMod.Logger.Warning("Found PiShock device without an assigned share code [Identifier=" + identifier + "]. The device wont work.");
                        }
                        else
                        {
                            device = new PiShockDevice(identifier, linkData);
                            Devices.Add(device);
                            PiShockVRCMod.Logger.Msg("Found PiShock device [Identifier=" + identifier + "]");
                        }
                    }
                    else
                    {
                        Configuration.DeviceLinks.Add(identifier, new PiShockDevice.LinkData() { ShareCode = "", DeviceId = -1 });
                        Configuration.Save();
                        PiShockVRCMod.Logger.Msg("Found new PiShock device [Identifier=" + identifier + "]. Please assign a share code via the configuration file.");
                    }
                }

                if (device != null)
                {
                    PiShockPoint.PointType type = (PiShockPoint.PointType)Enum.Parse(typeof(PiShockPoint.PointType), transform.GetChild(1).name.Split(':')[1].Trim());
                    int strength = int.Parse(transform.GetChild(2).name.Split(':')[1].Trim());
                    int duration = int.Parse(transform.GetChild(3).name.Split(':')[1].Trim());
                    float radius = -1;

                    if (transform.childCount >= 5)
                        radius = float.Parse(transform.GetChild(4).name.Split(':')[1].Trim());

                    device.Points.Add(new PiShockPoint() { Object = transform.parent.gameObject, Type = type, Strength = strength, Duration = duration, Radius = radius });

                    PiShockVRCMod.Logger.Msg("Found PiShockPoint [Device=" + identifier + "/Type=" + type.ToString() + "/Strength=" + strength + "/Duration=" + duration + "/Radius=" + radius + "]");
                }
            }
            else
            {
                foreach (var child in transform)
                    FindPoints(child.Cast<Transform>());
            }
        }
    }
}
