using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Tc;
public partial class IManager : _IManager_Base {
	public readonly string ServiceName;
	public IManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IManager_Base : IpcInterface {
	protected virtual void SetOperatingMode(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.SetOperatingMode");
	protected virtual KObject GetThermalEvent(byte[] _0) =>
		throw new NotImplementedException("Nn.Tc.IManager.GetThermalEvent not implemented");
	protected virtual void Unknown2(byte[] _0, out byte[] _1) =>
		throw new NotImplementedException("Nn.Tc.IManager.Unknown2 not implemented");
	protected virtual void Unknown3(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown3");
	protected virtual void Unknown4(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown4");
	protected virtual void Unknown5(byte[] _0) =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown5");
	protected virtual void Unknown6() =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown6");
	protected virtual void Unknown7() =>
		Console.WriteLine("Stub hit for Nn.Tc.IManager.Unknown7");
	protected virtual void Unknown8(out byte[] _0) =>
		throw new NotImplementedException("Nn.Tc.IManager.Unknown8 not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // SetOperatingMode
				SetOperatingMode(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x1: { // GetThermalEvent
				var _return = GetThermalEvent(im.GetBytes(8, 0x4));
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			case 0x2: { // Unknown2
				Unknown2(im.GetBytes(8, 0x4), out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			case 0x3: { // Unknown3
				Unknown3(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x4: { // Unknown4
				Unknown4(im.GetBytes(8, 0x4));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x5: { // Unknown5
				Unknown5(im.GetBytes(8, 0x8));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x6: { // Unknown6
				Unknown6();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // Unknown7
				Unknown7();
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // Unknown8
				Unknown8(out var _0);
				om.Initialize(0, 0, 1);
				om.SetBytes(8, _0);
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Tc.IManager");
		}
	}
}

