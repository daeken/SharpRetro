using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;
using LibSharpRetro;
using static SharpStationCore.Globals;

namespace SharpStationCore;

interface IMemory {
	int Size { get; }
	byte Load8(uint addr);
	ushort Load16(uint addr);
	uint Load32(uint addr);
	void Store8(uint addr, byte value);
	void Store16(uint addr, ushort value);
	void Store32(uint addr, uint value);
}

public abstract unsafe class BackedMemory : IMemory {
	protected readonly byte* Backing;

	protected BackedMemory(byte* backing) => Backing = backing;
	protected BackedMemory() => Backing = (byte*) Marshal.AllocHGlobal(Size);
	public abstract int Size { get; }

	public byte Load8(uint addr) => Backing[addr];
	public void Store8(uint addr, byte value) {
		Backing[addr] = value;
		Cpu.Invalidate(addr);
	}

	public ushort Load16(uint addr) => *(ushort*) (Backing + addr);
	public void Store16(uint addr, ushort value) {
		*(ushort*) (Backing + addr) = value;
		Cpu.Invalidate(addr);
	}

	public uint Load32(uint addr) => *(uint*) (Backing + addr);
	public void Store32(uint addr, uint value) {
		*(uint*) (Backing + addr) = value;
		Cpu.Invalidate(addr);
	}
}

public class Ram : BackedMemory {
	public override int Size => 2 * 1024 * 1024;
}

public class Scratchpad : BackedMemory {
	public override int Size => 1024;
}

public unsafe class Bios : BackedMemory {
	public Bios() {
		using(var fp = File.OpenRead("SCPH1001.bin")) {
			var bytes = new byte[Size];
			fp.Read(bytes, 0, Size);
			Marshal.Copy(bytes, 0, (IntPtr) Backing, Size);
		}
	}
	public override int Size => 512 * 1024;
}

public class Port<T> : IEnumerable where T : struct {
	public Func<T> _Load;
	public Action<T> _Store;
	public bool Debug;

	public Port(uint addr, string name, bool debug) {
		Addr = addr;
		Name = name;
		Debug = debug;
	}
	public uint Addr { get; }
	public string Name { get; }

	int BitSize => typeof(T).Name switch {
		"Byte" => 8,
		"UInt16" => 16,
		"UInt32" => 32,
		_ => throw new NotImplementedException($"Unknown type for Port bitsize: {typeof(T).Name}"),
	};

	public void Add(Func<T> load) => _Load = load;

	public void Add(Action<T> store) => _Store = store;

	public unsafe T Load() {
		var v = _Load?.Invoke() ?? throw new NotImplementedException($"No load{BitSize} for 0x{Addr:X8} ({Name})");
		if(Debug) $"[{Cpu.State->PC:X8}] Load{BitSize} from 0x{Addr:X8} ({Name}) -- {ToHex(v)}   @ {Timestamp}".Debug();
		return v;
	}

	static string ToHex(T value) {
		if(typeof(T) == typeof(byte)) return $"{(byte) (object) value:X2}";
		if(typeof(T) == typeof(ushort)) return $"{(ushort) (object) value:X4}";
		if(typeof(T) == typeof(uint)) return $"{(uint) (object) value:X8}";
		throw new NotImplementedException($"Unknown ToHex for {typeof(T).Name}");
	}

	public unsafe void Store(T value) {
		if(Debug) $"[{Cpu.State->PC:X8}] Store{BitSize} to 0x{Addr:X8} ({Name}) -- {ToHex(value)}   @ {Timestamp}".Debug();
		if(_Store == null) throw new NotImplementedException($"No store{BitSize} for 0x{Addr:X8} ({Name})");
		_Store(value);
	}

	public IEnumerator GetEnumerator() => throw new NotImplementedException();
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true)]
public class PortAttribute : Attribute {
	public readonly uint Addr, Stride;
	public readonly int Count;
	public readonly bool Debug;
	public PortAttribute(uint addr, bool debug = false) {
		Addr = addr;
		Debug = debug;
	}
	public PortAttribute(uint addr, int count, uint stride, bool debug = false) {
		Addr = addr;
		Count = count;
		Stride = stride;
		Debug = debug;
	}
}

public class IoPorts : IMemory {
	readonly Dictionary<uint, Port<ushort>> Ports16 = new();
	readonly Dictionary<uint, Port<uint>> Ports32 = new();

	readonly Dictionary<uint, Port<byte>> Ports8 = new();

	public IoPorts() {
		Port<T> MapProperty<T>(object instance, uint addr, string name, bool debug, PropertyInfo pi) where T : struct {
			var port = new Port<T>(addr, name, debug);
			if(pi.GetMethod != null) port.Add(() => (T) pi.GetValue(instance));
			if(pi.SetMethod != null) port.Add(v => pi.SetValue(instance, v));
			return port;
		}

		AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
			.SelectMany(x => x.GetMembers(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
			.Where(x => x.GetCustomAttributes(typeof(PortAttribute)).Count() != 0)
			.ForEach(x => {
				var attr = x.GetCustomAttribute<PortAttribute>();
				var addr = attr.Addr;
				var debug = attr.Debug;
				var name = $"{x.DeclaringType.Name}.{x.Name}";
				var instance = x.DeclaringType.GetField("Instance",
					BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)?.GetValue(null);
				instance ??=
					typeof(Globals)
						.GetFields(BindingFlags.Public | BindingFlags.Static)
						.FirstOrDefault(y => y.FieldType == x.DeclaringType)?.GetValue(null);
				if(attr.Count == 0 && x is FieldInfo fi) {
					if(!fi.IsStatic && instance == null)
						throw new($"Port {name} is not static and no instance available");
					if(fi.FieldType == typeof(byte))
						Add(new Port<byte>(addr, name, debug)
							{ () => (byte) fi.GetValue(instance), v => fi.SetValue(instance, v) });
					else if(fi.FieldType == typeof(ushort))
						Add(new Port<ushort>(addr, name, debug)
							{ () => (ushort) fi.GetValue(instance), v => fi.SetValue(instance, v) });
					else if(fi.FieldType == typeof(uint))
						Add(new Port<uint>(addr, name, debug)
							{ () => (uint) fi.GetValue(instance), v => fi.SetValue(instance, v) });
					else
						throw new NotImplementedException($"Field {x.DeclaringType.Name}.{x} not a supported type");
				} else if(x is FieldInfo f) {
					if(!f.IsStatic && instance == null)
						throw new($"Port {name} is not static and no instance available");
					if(f.FieldType == typeof(byte[])) {
						var arr = new byte[attr.Count];
						f.SetValue(instance, arr);
						Enumerable.Range(0, attr.Count).ForEach(i => Add(
							new Port<byte>(addr + (uint) (attr.Stride * i), name, debug) { () => arr[i], v => arr[i] = v }));
					} else if(f.FieldType == typeof(ushort[])) {
						var arr = new ushort[attr.Count];
						f.SetValue(instance, arr);
						Enumerable.Range(0, attr.Count).ForEach(i => Add(
							new Port<ushort>(addr + (uint) (attr.Stride * i), name, debug) { () => arr[i], v => arr[i] = v }));
					} else if(f.FieldType == typeof(uint[])) {
						var arr = new uint[attr.Count];
						f.SetValue(instance, arr);
						Enumerable.Range(0, attr.Count).ForEach(i => Add(
							new Port<uint>(addr + (uint) (attr.Stride * i), name, debug) { () => arr[i], v => arr[i] = v }));
					} else
						throw new NotImplementedException($"Field {x.DeclaringType.Name}.{x} not a supported type");
				} else if(x is PropertyInfo pi) {
					if((pi.GetMethod != null && !pi.GetMethod.IsStatic || pi.SetMethod != null && !pi.SetMethod.IsStatic) && instance == null)
						throw new($"Port {name} is not static and no instance available");
					if(attr.Count != 0)
						throw new($"Port {name} is multi-port but not a field");
					if(pi.PropertyType == typeof(byte)) Add(MapProperty<byte>(instance, addr, name, debug, pi));
					else if(pi.PropertyType == typeof(ushort)) Add(MapProperty<ushort>(instance, addr, name, debug, pi));
					else if(pi.PropertyType == typeof(uint)) Add(MapProperty<uint>(instance, addr, name, debug, pi));
					else throw new NotImplementedException($"Property {x.DeclaringType.Name}.{x} not a supported type");
				} else if(attr.Count != 0 && x is MethodInfo mi) {
					if(!mi.IsStatic && instance == null)
						throw new($"Port {name} is not static and no instance available");
					if(mi.ReturnType == typeof(void)) {
						var t = mi.GetParameters()[1].ParameterType;
						if(t == typeof(byte))
							Enumerable.Range(0, attr.Count).ForEach(
								i => Add(new Port<byte>(addr + (uint) (attr.Stride * i), name, debug)
									{ v => mi.Invoke(instance, new object[] { i, v }) }));
						else if(t == typeof(ushort))
							Enumerable.Range(0, attr.Count).ForEach(i =>
								Add(new Port<ushort>(addr + (uint) (attr.Stride * i), name, debug)
									{ v => mi.Invoke(instance, new object[] { i, v }) }));
						else if(t == typeof(uint))
							Enumerable.Range(0, attr.Count).ForEach(i =>
								Add(new Port<uint>(addr + (uint) (attr.Stride * i), name, debug)
									{ v => mi.Invoke(instance, new object[] { i, v }) }));
						else throw new NotImplementedException($"Method {x.DeclaringType.Name}.{x} not a supported type");
					} else {
						var t = mi.ReturnType;
						if(t == typeof(byte))
							Enumerable.Range(0, attr.Count).ForEach(i =>
								Add(new Port<byte>(addr + (uint) (attr.Stride * i), name, debug)
									{ () => (byte) mi.Invoke(instance, new object[] { i }) }));
						else if(t == typeof(ushort))
							Enumerable.Range(0, attr.Count).ForEach(i =>
								Add(new Port<ushort>(addr + (uint) (attr.Stride * i), name, debug)
									{ () => (ushort) mi.Invoke(instance, new object[] { i }) }));
						else if(t == typeof(uint))
							Enumerable.Range(0, attr.Count).ForEach(i =>
								Add(new Port<uint>(addr + (uint) (attr.Stride * i), name, debug)
									{ () => (uint) mi.Invoke(instance, new object[] { i }) }));
						else throw new NotImplementedException($"Method {x.DeclaringType.Name}.{x} not a supported type");
					}
				} else if(x is MethodInfo m) {
					if(!m.IsStatic && instance == null)
						throw new($"Port {name} is not static and no instance available");
					if(m.ReturnType == typeof(void)) {
						var t = m.GetParameters()[0].ParameterType;
						if(t == typeof(byte)) Add(new Port<byte>(addr, name, debug) { v => m.Invoke(instance, new[] { (object) v }) });
						else if(t == typeof(ushort)) Add(new Port<ushort>(addr, name, debug) { v => m.Invoke(instance, new[] { (object) v }) });
						else if(t == typeof(uint)) Add(new Port<uint>(addr, name, debug) { v => m.Invoke(instance, new[] { (object) v }) });
						else throw new NotImplementedException($"Method {x.DeclaringType.Name}.{x} not a supported type");
					} else {
						var t = m.ReturnType;
						if(t == typeof(byte)) Add(new Port<byte>(addr, name, debug) { () => (byte) m.Invoke(instance, null) });
						else if(t == typeof(ushort)) Add(new Port<ushort>(addr, name, debug) { () => (ushort) m.Invoke(instance, null) });
						else if(t == typeof(uint)) Add(new Port<uint>(addr, name, debug) { () => (uint) m.Invoke(instance, null) });
						else throw new NotImplementedException($"Method {x.DeclaringType.Name}.{x} not a supported type");
					}
				}
			});

		Ports32.Values.Where(port => !Ports16.ContainsKey(port.Addr)).ForEach(port =>
			Add(new Port<ushort>(port.Addr, port.Name, port.Debug) {
				port._Load != null ? () => (ushort) port.Load() : (Func<ushort>) null,
				port._Store != null ? value => port.Store(value) : (Action<ushort>) null,
			}));
	}
	public int Size => 8 * 1024;

	public byte Load8(uint addr) => Ports8.ContainsKey(addr) ? Ports8[addr].Load()
		: throw new NotImplementedException($"Unknown port for load8: {addr:X8}");
	public void Store8(uint addr, byte value) {
		if(!Ports8.ContainsKey(addr)) throw new NotImplementedException($"Unknown port for store8: {addr:X8} (0x{value:X2})");
		Ports8[addr].Store(value);
	}

	public ushort Load16(uint addr) => Ports16.ContainsKey(addr) ? Ports16[addr].Load()
		: throw new NotImplementedException($"Unknown port for load16: {addr:X8}");
	public void Store16(uint addr, ushort value) {
		if(!Ports16.ContainsKey(addr)) throw new NotImplementedException($"Unknown port for store16: {addr:X8} (0x{value:X4})");
		Ports16[addr].Store(value);
	}

	public uint Load32(uint addr) => Ports32.ContainsKey(addr) ? Ports32[addr].Load()
		: throw new NotImplementedException($"Unknown port for load32: {addr:X8}");
	public void Store32(uint addr, uint value) {
		if(!Ports32.ContainsKey(addr)) throw new NotImplementedException($"Unknown port for store32: {addr:X8} (0x{value:X8})");
		Ports32[addr].Store(value);
	}

	[Port(0x1F802041)] static void POST(byte value) => Console.WriteLine($"BIOS boot status: {value:X2}");

	void Add(Port<byte> port) {
		if(Ports8.ContainsKey(port.Addr)) {
			var cport = Ports8[port.Addr];
			if(cport._Load == null && port._Load != null)
				cport._Load = port._Load;
			else if(cport._Store == null && port._Store != null)
				cport._Store = port._Store;
			else
				throw new($"Port {port.Name} assigned to already-occupied address 0x{port.Addr:X8}");
			return;
		}
		Ports8[port.Addr] = port;
	}
	void Add(Port<ushort> port) {
		if(Ports16.ContainsKey(port.Addr)) {
			var cport = Ports16[port.Addr];
			if(cport._Load == null && port._Load != null)
				cport._Load = port._Load;
			else if(cport._Store == null && port._Store != null)
				cport._Store = port._Store;
			else
				throw new($"Port {port.Name} assigned to already-occupied address 0x{port.Addr:X8}");
			return;
		}
		Ports16[port.Addr] = port;
	}
	void Add(Port<uint> port) {
		if(Ports32.ContainsKey(port.Addr)) {
			var cport = Ports32[port.Addr];
			if(cport._Load == null && port._Load != null)
				cport._Load = port._Load;
			else if(cport._Store == null && port._Store != null)
				cport._Store = port._Store;
			else
				throw new($"Port {port.Name} assigned to already-occupied address 0x{port.Addr:X8}");
			return;
		}
		Ports32[port.Addr] = port;
	}
}

public class Blackhole : IMemory {
	public int Size => 0x7FFFFFFF;

	public byte Load8(uint addr) => 0;
	public ushort Load16(uint addr) => 0;
	public uint Load32(uint addr) => 0;

	public void Store8(uint addr, byte value) {}
	public void Store16(uint addr, ushort value) {}
	public void Store32(uint addr, uint value) {}
}

public class CoreMemory {
	readonly Bios Bios = new();
	readonly Blackhole Blackhole = new();
	readonly IoPorts IoPorts = new();
	readonly Ram Ram = new();
	readonly Scratchpad Scratchpad = new();

	T FindMemory<T>(uint vaddr, Func<IMemory, uint, T> func) {
		var rvaddr = vaddr;
		if(rvaddr >= 0x80000000U && rvaddr < 0xA0000000U)
			rvaddr -= 0x80000000U;
		else if(rvaddr >= 0xA0000000U && rvaddr < 0xFFFE0000U)
			rvaddr -= 0xA0000000U;

		if(rvaddr < Ram.Size) return func(Ram, rvaddr);
		if(rvaddr >= 0x1F000000 && rvaddr < 0x1F800000) return func(Blackhole, rvaddr);
		if(rvaddr >= 0x1F800000 && rvaddr < 0x1F800400) return func(Scratchpad, rvaddr - 0x1F800000);
		if(rvaddr >= 0x1F801000 && rvaddr < 0x1F803000) return func(IoPorts, rvaddr);
		if(rvaddr >= 0x1FC00000U && rvaddr < 0x1FC80000U) return func(Bios, rvaddr - 0x1FC00000);
		if(rvaddr >= 0xFFFE0000 && rvaddr < 0xFFFE0200) return func(IoPorts, rvaddr);
		throw new NotImplementedException($"Unknown memory region for addr {rvaddr:X08}h");
	}

	void FindMemory(uint vaddr, Action<IMemory, uint> func) {
		FindMemory<uint>(vaddr, (i, a) => {
			func(i, a);
			return 0;
		});
	}

	CoreMemory LogLoad(uint addr, int size) {
		Console.WriteLine($"Load {size} bytes from {addr:X}");
		return this;
	}
	CoreMemory LogStore(uint addr, uint value, int size) {
		Console.WriteLine($"Store {size} bytes ({value:X}) to {addr:X}");
		return this;
	}

	public byte Load8(uint addr) => LogLoad(addr, 1).FindMemory(addr, (i, a) => i.Load8(a));
	public void Store8(uint addr, byte value) => LogStore(addr, value, 1).FindMemory(addr, (i, a) => i.Store8(a, value));
	public ushort Load16(uint addr) => LogLoad(addr, 2).FindMemory(addr, (i, a) => i.Load16(a));
	public void Store16(uint addr, ushort value) => LogStore(addr, value, 2).FindMemory(addr, (i, a) => i.Store16(a, value));
	public uint Load32(uint addr) => LogLoad(addr, 4).FindMemory(addr, (i, a) => i.Load32(a));
	public void Store32(uint addr, uint value) => LogStore(addr, value, 4).FindMemory(addr, (i, a) => i.Store32(a, value));
}