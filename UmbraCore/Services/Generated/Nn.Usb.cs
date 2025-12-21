using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Usb;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_interface_descriptor {
	public byte BLength;
	public byte BDescriptorType;
	public byte BInterfaceNumber;
	public byte BAlternateSetting;
	public byte BNumEndpoints;
	public byte BInterfaceClass;
	public byte BInterfaceSubClass;
	public byte BInterfaceProtocol;
	public byte IInterface;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_descriptor_data {
	public ushort IdVendor;
	public ushort IdProduct;
	public ushort BcdDevice;
	public fixed byte Manufacturer[32];
	public fixed byte Product[32];
	public fixed byte SerialNumber[32];
}
public enum Usb_device_speed : uint {
	Unknown = 0x0,
	Low = 0x1,
	Full = 0x2,
	High = 0x3,
	SuperSpeed = 0x4,
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_device_descriptor {
	public byte BLength;
	public byte BDescriptorType;
	public ushort BcdUSB;
	public byte BDeviceClass;
	public byte BDeviceSubClass;
	public byte BDeviceProtocol;
	public byte BMaxPacketSize0;
	public ushort IdVendor;
	public ushort IdProduct;
	public ushort BcdDevice;
	public byte IManufacturer;
	public byte IProduct;
	public byte ISerialNumber;
	public byte BNumConfigurations;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_bos_dev_capability_descriptor {
	public byte BLength;
	public byte BDescriptorType;
	public byte BDevCapabilityType;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_bos_descriptor {
	public byte BLength;
	public byte BDescriptorType;
	public ushort WTotalLength;
	public byte BNumDeviceCaps;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_endpoint_descriptor {
	public byte BLength;
	public byte BDescriptorType;
	public byte BEndpointAddress;
	public byte BmAttributes;
	public ushort WMaxPacketSize;
	public byte BInterval;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Usb_report_entry {
	public uint UrbId;
	public uint RequestedSize;
	public uint TransferredSize;
	public uint UrbStatus;
}
