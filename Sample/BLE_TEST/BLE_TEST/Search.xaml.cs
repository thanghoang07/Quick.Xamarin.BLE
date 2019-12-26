using Quick.Xamarin.BLE;
using Quick.Xamarin.BLE.Abstractions;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace BLE_TEST
{
    public partial class Search : ContentPage
    {
        public static AdapterConnectStatus BleStatus;
        public static IBle ble;
        public static IDev ConnectDevice = null;
        Device deviceSignee;
        public static List<IDev> ScanDevices = new List<IDev>();
        public Search()
        {
            InitializeComponent();
            ble = CrossBle.Createble();
            //when search devices
            ble.OnScanDevicesIn += Ble_OnScanDevicesIn;
            BleStatus = ble.AdapterConnectStatus;
        }

        private void Ble_OnScanDevicesIn(object sender, IDev e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    if (e.Name != null)
                    {
                        var n = ScanDevices.Find(x => x.Uuid == e.Uuid);
                        if (n == null)
                        {
                            if (e.Name.Equals("Signee Settings"))
                            {
                                ScanDevices.Add(e);
                                deviceSignee = new Device(e.Name, e.Uuid);
                                Debug.WriteLine($"bluetooth: uuid-->{e.Uuid}|| name-->{e.Name}");
                                nameDevice.Text = $"{deviceSignee.Name}-{ deviceSignee.Uuid}";
                            }
                        }
                    }
                }
                catch { }
            });
        }

        private async void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var n = (Device)e.Item;
            foreach (var dev in ScanDevices)
            {
                if (n.Uuid == dev.Uuid)
                {
                    var check = await DisplayAlert("", "Connecting to  [" + dev.Name + "]", "ok", "cancel");
                    if (check)
                    {
                        ConnectDevice = dev;
                        ConnectDevice.ConnectToDevice();
                        await Navigation.PushAsync(new Service(), false);
                    }
                }
            }
        }
        protected override void OnAppearing()
        {
            ScanDevices.Clear();
            ble.StartScanningForDevices();
            base.OnAppearing();
        }
        protected override void OnDisappearing()
        {
            ble.StopScanningForDevices();
            base.OnDisappearing();
        }

        private async void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            var raw = ((Label)sender).Text;
            string[] textt = raw.Split('-');
            var deviceSn = new Device(textt[0], textt[1]);
            foreach (var dev in ScanDevices)
            {
                if (deviceSn.Uuid == dev.Uuid)
                {
                    var check = await DisplayAlert("", "Connecting to  [" + dev.Name + "]", "ok", "cancel");
                    if (check)
                    {
                        ConnectDevice = dev;
                        ConnectDevice.ConnectToDevice();
                        await Navigation.PushAsync(new Service(), false);
                    }
                }
            }
        }
    }
}
