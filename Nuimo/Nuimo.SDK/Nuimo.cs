using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Nuimo.SDK
{
	public class Nuimo
	{
		private BluetoothLEDevice _nuimo = null;

		public async Task<bool> Connect(bool pairIfUnpaired = true)
		{
			//	Search for device paired first
			DeviceInformation deviceInformation = await GetDeviceInformation(true);

			//	Search for unpaird devices
			if (deviceInformation == null)
			{
				deviceInformation = await GetDeviceInformation(false);

				//	If we found the device then pair it if requested.
				//	Pairing makes initial detection so much faster.
				if (deviceInformation != null && pairIfUnpaired)
					await deviceInformation.Pairing.PairAsync();
			}

			if (deviceInformation != null)
				_nuimo = await BluetoothLEDevice.FromIdAsync(deviceInformation.Id);

			return _nuimo != null;
		}

		private static async Task<DeviceInformation> GetDeviceInformation(bool paired)
		{
			var deviceSelector = BluetoothLEDevice.GetDeviceSelectorFromPairingState(paired);

			//	Get a list of all devices
			var deviceInfoCollection = await DeviceInformation.FindAllAsync(deviceSelector);

			//	See if the devices found are a Nuimo and return that if not return null
			var deviceInfo = deviceInfoCollection.FirstOrDefault(di => di.Name.Equals("Nuimo", StringComparison.CurrentCultureIgnoreCase));
			return deviceInfo;
		}

		public bool Connected { get { return _nuimo != null; } }
	}
}
