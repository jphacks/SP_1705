using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace WinMessenger
{
    sealed class MessageTransfer
    {
        private static readonly Guid
            ServiceId = Guid.Parse("52E7111F-A600-434D-AA23-DFE0107E6522"),
            cServiceId = Guid.Parse("4D0C42A1-01D4-46F1-8C32-97AD0F3CBB40");

        public static MessageTransfer Instance { get; private set; }

        private readonly ConcurrentBag<byte[]> queue = new ConcurrentBag<byte[]>();

        public MessageTransfer(MessageAccount account)
        {
            Init(account);
            Instance = this;
        }

        public void Send(byte[] dat)
        {
            queue.Add(dat);
        }

        private async void Init(MessageAccount account)
        {
            foreach (var msg in account.GetMessages())
            {
                queue.Add(msg.Binary);
            }

            var result = await GattServiceProvider.CreateAsync(ServiceId);

            if (result.Error == BluetoothError.Success)
            {
                var serviceProvider = result.ServiceProvider;

                var prm = new GattLocalCharacteristicParameters()
                {
                    CharacteristicProperties = GattCharacteristicProperties.WriteWithoutResponse,
                    UserDescription = "DMsg"
                };
                var characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(cServiceId, prm);
                if (characteristicResult.Error != BluetoothError.Success)
                {
                    // An error occurred.
                    return;
                }
                var _readCharacteristic = characteristicResult.Characteristic;
                _readCharacteristic.WriteRequested += OnWriteMessage;

                var advParameters = new GattServiceProviderAdvertisingParameters
                {
                    IsDiscoverable = true,
                    //IsConnectable = true
                };
                serviceProvider.StartAdvertising(advParameters);
            }

            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
            var deviceWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                    requestedProperties,
                    DeviceInformationKind.AssociationEndpoint);

            deviceWatcher.Added += OnAddDevice;
            deviceWatcher.Start();

            var udp = new UdpClient(AddressFamily.InterNetworkV6);
            udp.EnableBroadcast = true;
            udp.Client.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
            udp.Client.Bind(new IPEndPoint(IPAddress.IPv6Any, 24680));
            MessageBroadcast(udp);

            while (true)
            {
                var ures = await udp.ReceiveAsync();
                try
                {
                    account.AddMessage(ures.Buffer);
                }
                catch (Exception) { }
            }
        }

        private async void MessageBroadcast(UdpClient client)
        {
            while (true)
            {
                foreach (var item in queue)
                {
                    await client.SendAsync(item, item.Length, new IPEndPoint(IPAddress.Broadcast, 24680));
                    Debug.WriteLine("LAN Broadcast " + item.Length);
                    await Task.Delay(1000);
                }
                await Task.Delay(1000);
            }
        }

        private async void OnAddDevice(DeviceWatcher sender, DeviceInformation args)
        {
            if (queue.IsEmpty)
                return;

            using (var bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(args.Id))
            {
                var services = await bluetoothLeDevice.GetGattServicesAsync();
                if (services.Status != GattCommunicationStatus.Success)
                    return;
                foreach (var svc in services.Services)
                {
                    Debug.WriteLine("Device Found: " + svc.Uuid);
                    if (svc.Uuid != ServiceId)
                        continue;

                    var cservice = await svc.GetCharacteristicsAsync();
                    if (cservice.Status != GattCommunicationStatus.Success)
                        continue;

                    foreach (var csvc in cservice.Characteristics)
                    {
                        if (csvc.Uuid != cServiceId)
                            continue;

                        if (!csvc.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse))
                            continue;

                        Debug.WriteLine("Blietooth Message Sending... " + args.Id);
                        foreach (var item in queue)
                        {
                            await csvc.WriteValueWithResultAsync(item.AsBuffer());
                        }
                        Debug.WriteLine("Blietooth Message Sended." + args.Id);
                    }
                }
            }
        }

        private async void OnWriteMessage(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            var deferral = args.GetDeferral();
            try
            {
                var req = await args.GetRequestAsync();
                var reader = DataReader.FromBuffer(req.Value);

                var buf = new byte[reader.UnconsumedBufferLength];
                reader.ReadBytes(buf);
                var xml = DMessenger.MessageEncoder.Decode(buf);
                var msg = new DMessenger.Message(xml);

                if (req.Option == GattWriteOption.WriteWithResponse)
                    req.Respond();
            }
            finally
            {
                deferral.Complete();
            }
        }
    }
}
