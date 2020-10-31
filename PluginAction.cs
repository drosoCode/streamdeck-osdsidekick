using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace OSDSidekick
{
    [PluginActionId("com.drosocode.osdsidekick")]
    public class PluginAction : PluginBase
    {
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Gen_init();

        // Token: 0x06000186 RID: 390
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Gen_SetOSD(int iHubIndex, IntPtr bytes, int size);

        // Token: 0x06000187 RID: 391
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Gen_GetOSD(int iHubIndex, IntPtr bytes, int size, IntPtr return_value, int rtn_length);

        // Token: 0x06000188 RID: 392
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Gen_Close();

        private int nbMonitors = 0;

        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings();
                instance.MonitorID = String.Empty;
                instance.Setting = String.Empty;
                instance.Value = String.Empty;
                return instance;
            }

            [JsonProperty(PropertyName = "monitorID")]
            public string MonitorID { get; set; }

            [JsonProperty(PropertyName = "setting")]
            public string Setting { get; set; }

            [JsonProperty(PropertyName = "value")]
            public string Value { get; set; }
        }

        #region Private Members

        private PluginSettings settings;

        #endregion
        public PluginAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }

            Logger.Instance.LogMessage(TracingLevel.INFO, "Initializing Monitor Connection");
            nbMonitors = Gen_init();
            Logger.Instance.LogMessage(TracingLevel.INFO, nbMonitors.ToString() + " monitors detected");

            Connection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
        }

        private void Connection_OnTitleParametersDidChange(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.TitleParametersDidChange> e)
        {
        }

        private void Connection_OnSendToPlugin(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
        }

        private void Connection_OnPropertyInspectorDidDisappear(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidDisappear> e)
        {
        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidAppear> e)
        {
        }

        private void Connection_OnDeviceDidDisconnect(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidDisconnect> e)
        {
        }

        private void Connection_OnDeviceDidConnect(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidConnect> e)
        {
        }

        private void Connection_OnApplicationDidTerminate(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidTerminate> e)
        {
        }

        private void Connection_OnApplicationDidLaunch(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidLaunch> e)
        {
        }

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Closing Monitor Connection");
            Gen_Close();
            Connection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public async override void KeyPressed(KeyPayload payload)
        {
        }

        public async override void KeyReleased(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Released");
            double value = double.Parse(settings.Value);
            int id = Int32.Parse(settings.MonitorID);
            if (id < 0)
                for(int i = 0; i < nbMonitors; i++)
                    SetValue(i, value);
            else
                SetValue(id, value);
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }


        #region Private Methods

        private void SetValue(int display, double value)
        {
            if (settings.Setting == "brightness")
                SetOSD(display, 3, new byte[] { 16, 0, (byte)value });
            else if (settings.Setting == "contrast")
                SetOSD(display, 3, new byte[] { 18, 0, (byte)value });
        }

        private void SetOSD(int display, int count, byte[] ary)
        {
            byte[] array = new byte[32];
            byte b = 130;
            b += Convert.ToByte(count - 1);
            Array.Clear(array, 0, 32);
            array[0] = 110;
            array[1] = 81;
            array[2] = (byte)(128 + count + 1);
            array[3] = 3;
            for (int j = 0; j < count; j++)
            {
                array[4 + j] = ary[j];
            }
            int cb = Marshal.SizeOf<byte>(array[0]) * array.Length;
            IntPtr intPtr2 = Marshal.AllocHGlobal(cb);
            Marshal.Copy(array, 0, intPtr2, count + 4);
            try
            {
                Thread.Sleep(200);
                Gen_SetOSD(display, intPtr2, count + 4);
                Thread.Sleep(200);
            }
            finally
            {
                Marshal.FreeHGlobal(intPtr2);
            }
            Thread.Sleep(150);
        }

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}