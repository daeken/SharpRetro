using System.Runtime.InteropServices;
using UmbraCore.Core;
// ReSharper disable once CheckNamespace
namespace UmbraCore.Services.Nn.Aocsrv.Detail;
public partial class IAddOnContentManager : _IAddOnContentManager_Base {
	public readonly string ServiceName;
	public IAddOnContentManager(string serviceName) => ServiceName = serviceName;
}
public abstract class _IAddOnContentManager_Base : IpcInterface {
	protected virtual uint CountAddOnContentByApplicationId(ulong _0) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.CountAddOnContentByApplicationId not implemented");
	protected virtual void ListAddOnContentByApplicationId(uint _0, uint _1, ulong _2, out uint _3, Span<uint> _4) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.ListAddOnContentByApplicationId not implemented");
	protected virtual uint CountAddOnContent(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.CountAddOnContent not implemented");
	protected virtual void ListAddOnContent(uint _0, uint _1, ulong _2, ulong _3, out uint _4, Span<uint> _5) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.ListAddOnContent not implemented");
	protected virtual ulong GetAddOnContentBaseIdByApplicationId(ulong _0) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.GetAddOnContentBaseIdByApplicationId not implemented");
	protected virtual ulong GetAddOnContentBaseId(ulong _0, ulong _1) =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.GetAddOnContentBaseId not implemented");
	protected virtual void PrepareAddOnContentByApplicationId(uint _0, ulong _1) =>
		Console.WriteLine("Stub hit for Nn.Aocsrv.Detail.IAddOnContentManager.PrepareAddOnContentByApplicationId");
	protected virtual void PrepareAddOnContent(uint _0, ulong _1, ulong _2) =>
		Console.WriteLine("Stub hit for Nn.Aocsrv.Detail.IAddOnContentManager.PrepareAddOnContent");
	protected virtual KObject GetAddOnContentListChangedEvent() =>
		throw new NotImplementedException("Nn.Aocsrv.Detail.IAddOnContentManager.GetAddOnContentListChangedEvent not implemented");
	protected override unsafe void _Dispatch(IncomingMessage im, OutgoingMessage om) {
		switch(im.CommandId) {
			case 0x0: { // CountAddOnContentByApplicationId
				var _return = CountAddOnContentByApplicationId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x1: { // ListAddOnContentByApplicationId
				ListAddOnContentByApplicationId(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), out var _0, im.GetSpan<uint>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x2: { // CountAddOnContent
				var _return = CountAddOnContent(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 4);
				om.SetData(8, _return);
				break;
			}
			case 0x3: { // ListAddOnContent
				ListAddOnContent(im.GetData<uint>(8), im.GetData<uint>(12), im.GetData<ulong>(16), im.Pid, out var _0, im.GetSpan<uint>(0x6, 0));
				om.Initialize(0, 0, 4);
				om.SetData(8, _0);
				break;
			}
			case 0x4: { // GetAddOnContentBaseIdByApplicationId
				var _return = GetAddOnContentBaseIdByApplicationId(im.GetData<ulong>(8));
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x5: { // GetAddOnContentBaseId
				var _return = GetAddOnContentBaseId(im.GetData<ulong>(8), im.Pid);
				om.Initialize(0, 0, 8);
				om.SetData(8, _return);
				break;
			}
			case 0x6: { // PrepareAddOnContentByApplicationId
				PrepareAddOnContentByApplicationId(im.GetData<uint>(8), im.GetData<ulong>(16));
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x7: { // PrepareAddOnContent
				PrepareAddOnContent(im.GetData<uint>(8), im.GetData<ulong>(16), im.Pid);
				om.Initialize(0, 0, 0);
				break;
			}
			case 0x8: { // GetAddOnContentListChangedEvent
				var _return = GetAddOnContentListChangedEvent();
				om.Initialize(0, 1, 0);
				om.Copy(0, CreateHandle(_return, copy: true));
				break;
			}
			default:
				throw new NotImplementedException($"Got unhandled command 0x{im.CommandId:X} in Nn.Aocsrv.Detail.IAddOnContentManager");
		}
	}
}

