using System.Diagnostics;
using System.Runtime.InteropServices;
using LibSharpRetro;

namespace UmbraCore.Core;

public unsafe class IncomingMessage {
	public readonly byte* Buffer;
	public readonly bool IsDomainObject;
	public readonly ushort Type;
	public readonly uint CommandId, ACount, BCount, XCount, MoveCount, CopyCount;
	public readonly ulong Pid;
	public readonly bool HasC, HasPid;
	public readonly uint DomainHandle, DomainCommand;
	readonly uint WLen, RawOffset, SfciOffset, DescOffset, CopyOffset, MoveOffset;
	public IncomingMessage(byte* buffer, bool isDomainObject = false) {
		IsDomainObject = isDomainObject;
		Buffer = buffer;
		var buf = (uint*) buffer;
		Type = (ushort) (buf[0] & 0xFFFF);
		XCount = (buf[0] >> 16) & 0xF;
		ACount = (buf[0] >> 20) & 0xF;
		BCount = (buf[0] >> 24) & 0xF;
		WLen = buf[1] & 0x3FF;
		HasC = ((buf[1] >> 10) & 0x3) != 0;
		DomainHandle = 0;
		DomainCommand = 0;
		var pos = 2U;
		if(buf[1].HasBit(31)) {
			var hd = buf[pos++];
			HasPid = hd.HasBit(0);
			CopyCount = (hd >> 1) & 0xF;
			MoveCount = hd >> 5;
			if(HasPid) {
				Pid = *(ulong*) &buf[pos];
				pos += 2;
			}
			CopyOffset = pos * 4;
			pos += CopyCount;
			MoveOffset = pos * 4;
			pos += MoveCount;
		}

		DescOffset = pos * 4;

		pos += XCount * 2;
		pos += ACount * 3;
		pos += BCount * 3;
		RawOffset = pos * 4;
		if((pos & 3) != 0)
			pos += 4 - (pos & 3);
		if(isDomainObject && Type == 4) {
			DomainHandle = buf[pos + 1];
			DomainCommand = buf[pos] & 0xFF;
			pos += 4;
		}
		
		Debug.Assert(Type == 2 || isDomainObject && DomainCommand == 2 || buf[pos] == 0x49434653); // SFCI
		SfciOffset = pos * 4;

		CommandId = GetData<uint>(0);
	}

	public T GetData<T>(uint offset) => new Span<T>(Buffer + SfciOffset + 8 + offset, Marshal.SizeOf<T>())[0];
	public byte[] GetBytes(uint offset, uint size) =>
		new Span<byte>(Buffer + SfciOffset + 8 + offset, (int) size).ToArray();
	public void* GetDataPointer(uint offset) => Buffer + SfciOffset + 8 + offset;

	public static Func<IncomingMessage, object> BytesGetter(uint offset, uint size) => im =>
		new Span<byte>(im.Buffer + im.SfciOffset + 8 + offset, (int) size).ToArray();

	public Buffer<T> GetBuffer<T>(uint type, int num) where T : struct {
		if((type & 0x20) != 0)
			return GetBuffer<T>((type & ~0x20U) | 4U, num) ?? GetBuffer<T>((type & ~0x20U) | 8U, num);

		var ax = (type & 3) == 1 ? 1 : 0;
		var flags_ = type & 0xC0U;
		var flags = flags_ == 0x80 ? 3 : flags_ == 0x40 ? 1UL : 0UL;
		var cx = (type & 0xC) == 8 ? 1 : 0;
		
		switch((ax << 1) | cx) {
			case 0: { // B
				var t = (uint*) (Buffer + DescOffset + XCount * 8 + ACount * 12 + num * 12);
				ulong a = t[0], b = t[1], c = t[2];
				Debug.Assert((c & 0x3U) == flags);
				var buffer = new Buffer<T>(b | (((((c >> 2) << 4) & 0x70) | ((c >> 28) & 0xFU)) << 32),
					a | (((c >> 24) & 0xFU) << 32));
				if(BCount <= num || buffer.Size == 0)
					goto case 1; //  C buffer
				return buffer;
			}
			case 1: { // C
				var t = (uint*) (Buffer + RawOffset + WLen * 4);
				ulong a = t[0], b = t[1];
				return new Buffer<T>(a | ((b & 0xFFFFU) << 32), b >> 16);
			}
			case 2: { // A
				var t = (uint*) (Buffer + DescOffset + XCount * 8 + num * 12);
				ulong a = t[0], b = t[1], c = t[2];
				Debug.Assert((c & 0x3) == flags);
				var buffer = new Buffer<T>(b | (((((c >> 2) << 4) & 0x70) | ((c >> 28) & 0xFU)) << 32),
					a | (((c >> 24) & 0xFU) << 32));
				if(ACount <= num || buffer.Size == 0)
					goto case 3; // X buffer
				return buffer;
			}
			case 3: { // X
				var t = (uint*) (Buffer + DescOffset + num * 8);
				ulong a = t[0], b = t[1];
				return new Buffer<T>(b | ((((a >> 12) & 0xFU) | ((a >> 2) & 0x70U)) << 32), a >> 16);
			}
		}
		return null;
	}
	
	public uint GetMove(uint offset) {
		var buf = (uint*) Buffer;
		return IsDomainObject ? buf[(SfciOffset >> 2) + 4 + offset] : buf[(MoveOffset >> 2) + offset];
	}
	public uint GetCopy(uint offset) => ((uint*) Buffer)[(CopyOffset >> 2) + offset];
}

public unsafe class OutgoingMessage {
	readonly byte* Buffer;
	public bool IsDomainObject;
	public uint ErrCode;
	uint SfcoOffset, RealDataOffset, CopyCount;
	public readonly IncomingMessage Incoming;

	public OutgoingMessage(byte* buffer, bool isDomainObject, IncomingMessage im) {
		Buffer = buffer;
		IsDomainObject = isDomainObject;
		Incoming = im;
	}

	public void Initialize(uint moveCount, uint copyCount, uint dataBytes) {
		CopyCount = copyCount;
		var buf = (uint *) Buffer;
		for(var i = 0; i < 100; ++i)
			buf[i] = 0;
		buf[0] = 0;
		buf[1] = 0;
		if(moveCount != 0 || copyCount != 0) {
			buf[1] = moveCount != 0 && !IsDomainObject || copyCount != 0 ? 1U << 31 : 0;
			buf[2] = (copyCount << 1) | ((IsDomainObject ? 0 : moveCount) << 5);
		}

		var pos = 2 + (moveCount != 0 && !IsDomainObject || copyCount != 0 ? 1 + moveCount + copyCount : 0);
		if((pos & 3) != 0)
			pos += 4 - (pos & 3);
		if(IsDomainObject) {
			buf[pos] = moveCount;
			pos += 4;
		}
		RealDataOffset = IsDomainObject ? moveCount << 2 : 0;
		var dataWords = (RealDataOffset >> 2) + (dataBytes & 3) != 0 ? (dataBytes >> 2) + 1 : dataBytes >> 2;

		buf[1] |= 4U + (IsDomainObject ? 4U : 0) + 4 + dataWords;

		SfcoOffset = pos * 4;
		buf[pos] = 0x4f434653; // SFCO
	}

	public void Move(uint offset, uint handle) {
		var buf = (uint*) Buffer;
		if(IsDomainObject) {
			Console.WriteLine($"Sending back domain object 0x{handle:X}");
			buf[(SfcoOffset >> 2) + 4 + offset] = handle;
		} else
			buf[3 + CopyCount + offset] = handle;
	}

	public void Copy(uint offset, uint handle) =>
		((uint*) Buffer)[3 + offset] = handle;

	public void Bake() {
		var buf = (uint*) Buffer;
		buf[(SfcoOffset >> 2) + 2] = ErrCode;
	}
	
	public void SetData<T>(uint offset, T value) => 
		new Span<T>(Buffer + SfcoOffset + 8 + offset + (offset < 8 ? 0 : RealDataOffset), Marshal.SizeOf<T>())[0] = value;
	public void* GetDataPointer(uint offset) => Buffer + SfcoOffset + 8 + offset + (offset < 8 ? 0 : RealDataOffset);
	public void SetBytes(uint offset, byte[] data) =>
		data.CopyTo(new Span<byte>(GetDataPointer(offset), data.Length));
}

public class IpcException : Exception {
	public uint Code;
	public IpcException(uint code) => Code = code;
}

public abstract class IpcInterface : KObject {
	public bool IsDomainObject;
	IpcInterface DomainOwner;
	uint DomainHandleIter = 0xf001;
	const uint ThisHandle = 0xf000;
	readonly Dictionary<uint, KObject> DomainHandles = [];
	readonly Dictionary<uint, uint> DomainHandleMap = [];
	
	public uint CreateHandle(KObject obj, bool copy = false) {
		if(obj == null) throw new NotSupportedException();
		Console.WriteLine($"Creating handle for object with real handle 0x{obj.Handle:X}");
		if(copy) return obj.Handle;
		if(DomainOwner != null) return DomainOwner.CreateHandle(obj);
		if(!IsDomainObject) return obj.Handle;
		if(DomainHandleMap.TryGetValue(obj.Handle, out var dmap)) return dmap;
		var handle = DomainHandleMap[obj.Handle] = DomainHandleIter++;
		DomainHandles[handle] = obj;
		if(obj is IpcInterface iface)
			iface.DomainOwner = this;
		return handle;
	}

	protected abstract void _Dispatch(IncomingMessage incoming, OutgoingMessage outgoing);

	void Dispatch(IncomingMessage incoming, OutgoingMessage outgoing) {
		try {
			_Dispatch(incoming, outgoing);
		} catch(IpcException ie) {
			outgoing.Initialize(0, 0, 0);
			outgoing.ErrCode = ie.Code;
		}
	}

	public unsafe uint SyncMessage(ulong bufferAddr, uint bufferSize, out bool closeHandle) {
		var buffer = (byte*) bufferAddr;
		//new Span<byte>(buffer, (int) bufferSize).Hexdump();
		var incoming = new IncomingMessage(buffer, IsDomainObject);
		var outgoing = new OutgoingMessage(buffer, IsDomainObject, incoming);
		var ret = 0xF601U;
		closeHandle = false;
		var target = this;
		if(IsDomainObject && incoming.DomainHandle != ThisHandle && incoming.Type == 4)
			target = (IpcInterface) DomainHandles[incoming.DomainHandle];
		if(!IsDomainObject || incoming.DomainCommand == 1 || incoming.Type == 2 || incoming.Type == 5)
			switch(incoming.Type) {
				case 2:
					closeHandle = true;
					outgoing.Initialize(0, 0, 0);
					ret = 0x25a0b;
					break;
				case 4:
				case 6:
					Console.WriteLine($"IPC command {incoming.CommandId} for {target}");
					target.Dispatch(incoming, outgoing);
					ret = 0;
					break;
				case 5:
				case 7:
					switch(incoming.CommandId) {
						case 0: // ConvertSessionToDomain
							Console.WriteLine("Converting session to domain...");
							outgoing.Initialize(0, 0, 4);
							IsDomainObject = true;
							outgoing.SetData(8, ThisHandle);
							break;
						case 2: // DuplicateSession
							outgoing.IsDomainObject = false;
							outgoing.Initialize(1, 0, 0);
							outgoing.Move(0, Handle);
							break;
						case 3: // QueryPointerBufferSize
							outgoing.Initialize(0, 0, 4);
							outgoing.SetData(8, 0x500U);
							break;
						case 4: // DuplicateSessionEx
							outgoing.IsDomainObject = false;
							outgoing.Initialize(1, 0, 0);
							outgoing.Move(0, Handle);
							outgoing.ErrCode = 0;
							break;
						default:
							throw new NotImplementedException($"Unknown domain command ID: {incoming.CommandId}");
					}
					ret = 0;
					break;
				default:
					throw new NotImplementedException($"Unknown message type: {incoming.Type}");
			}
		else
			switch(incoming.DomainCommand) {
				case 2:
					DomainHandles.Remove(incoming.DomainHandle);
					outgoing.Initialize(0, 0, 0);
					outgoing.ErrCode = 0;
					ret = 0;
					break;
				default:
					throw new NotImplementedException($"Unknown domain command ID: {incoming.DomainCommand}");
			}
		if(ret == 0)
			outgoing.Bake();
		//new Span<byte>(buffer, (int) bufferSize).Hexdump();
		return ret;
	}
}

public class IpcManager {
    public void Setup(GameWrapper game) {
        game.Callbacks.ConnectToNamedPort = (name, ref handle) => {
            Console.WriteLine($"Attempting to connect to '{Marshal.PtrToStringAnsi((IntPtr) name)}'");
            handle = 0xdaedae00;
            return 0;
        };
    }
}