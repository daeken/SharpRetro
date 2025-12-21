using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Socket;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct BsdBufferConfig {
	public uint Version;
	public uint Tcp_tx_buf_size;
	public uint Tcp_rx_buf_size;
	public uint Tcp_tx_buf_max_size;
	public uint Tcp_rx_buf_max_size;
	public uint Udp_tx_buf_size;
	public uint Udp_rx_buf_size;
	public uint Sb_efficiency;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Timeout {
	public ulong Sec;
	public ulong Usec;
	public ulong Off;
}
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Sockaddr {
	public byte Sa_len;
	public byte Sa_family;
	public byte _Addr;
}
