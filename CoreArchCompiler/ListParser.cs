using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DoubleSharp.Linq;
using PrettyPrinter;

namespace CoreArchCompiler; 

public abstract class EType {
	public static readonly EUndef Undef = new();
	public static readonly EUnit Unit = (EUnit) EUnit.CompiletimeType;
	public static readonly EString String = (EString) EString.CompiletimeType;
	public static readonly EVector Vector = (EVector) EVector.CompiletimeType;

	public bool Runtime;
	public abstract EType AsRuntime();
	public abstract EType AsCompiletime();
	public EType AsRuntime(bool cond) => cond ? AsRuntime() : this;
}

public class EInt : EType {
	public readonly bool Signed;
	public readonly int Width;
	public EInt(bool signed, int width) {
		Signed = signed;
		Width = width;
	}

	public override EType AsCompiletime() => Runtime ? new(Signed, Width) { Runtime = false } : this;
	public override EType AsRuntime() => Runtime ? this : new(Signed, Width) { Runtime = true };
	public override string ToString() => $"EInt({(Signed ? "i" : "u")}{Width})";

	public void Deconstruct(out bool signed, out int width) {
		signed = Signed;
		width = Width;
	}
}
public class EFloat : EType {
	public readonly int Width;
	public EFloat(int width) => Width = width;

	public override EType AsRuntime() => Runtime ? this : new(Width) { Runtime = true };
	public override EType AsCompiletime() => Runtime ? new(Width) { Runtime = false } : this;
	public override string ToString() => $"EFloat({Width})";

	public void Deconstruct(out int width) => width = Width;
}
public class EUndef : EType {
	public override string  ToString() => "EUndef";
	public override EType AsRuntime() => Runtime ? this : new() { Runtime = true };
	public override EType AsCompiletime() => Runtime ? Undef : this;
}
public class EString : EType {
	public override string  ToString() => "EString";
	public static readonly EType RuntimeType = new EString { Runtime = true };
	public static readonly EType CompiletimeType = new EString { Runtime = false };
	public override EType AsRuntime() => RuntimeType;
	public override EType AsCompiletime() => CompiletimeType;
}
public class EUnit : EType {
	public override string  ToString() => "EUnit";
	public static readonly EType RuntimeType = new EUnit { Runtime = true };
	public static readonly EType CompiletimeType = new EUnit { Runtime = false };
	public override EType AsRuntime() => RuntimeType;
	public override EType AsCompiletime() => CompiletimeType;
}
public class EBool : EType {
	public static readonly EType RuntimeType = new EBool { Runtime = true };
	public static readonly EType CompiletimeType = new EBool { Runtime = false };
	public override EType AsRuntime() => RuntimeType;
	public override EType AsCompiletime() => CompiletimeType;
}
public class EVector : EType {
	public static readonly EType RuntimeType = new EVector { Runtime = true };
	public static readonly EType CompiletimeType = new EVector { Runtime = false };
	public override EType AsRuntime() => RuntimeType;
	public override EType AsCompiletime() => CompiletimeType;
}
	
public abstract class PTree {
	public EType Type = EType.Undef;

	public override bool Equals(object obj) => ToString() == obj?.ToString();
	protected bool Equals(PTree other) => Equals(Type, other.Type) && ToString() == other.ToString();
	public override int GetHashCode() => HashCode.Combine(
		Type.GetHashCode(), 
		ToString()?.GetHashCode()
	);

	public PTree Cast<T>() {
		var et = (EType) (default(T) switch {
			bool => new EBool(), 
			byte when Type is not EBool => new EInt(false, 8),
			byte when Type is EBool => new EInt(false, 1),
			ushort => new EInt(false, 16), 
			uint => new EInt(false, 32), 
			ulong => new EInt(false, 64), 
			sbyte => new EInt(true, 8), 
			short => new EInt(true, 16), 
			int => new EInt(true, 32), 
			long => new EInt(true, 64), 
			_ => throw new NotImplementedException()
		});
		et = et.AsRuntime(Type.Runtime);

		if(typeof(T) == typeof(bool) && Type is not EBool) {
			if(Type is not EInt)
				throw new NotSupportedException($"Attempted to cast {Type} to bool");
			var itree = new PList { new PName("!="), this, new PInt(0) { Type = Type } };
			itree.Type = et;
			return itree;
		}
		
		if(Type.ToString() == et.ToString() && et.Runtime == Type.Runtime) return this;
		var tn = et switch {
			EInt(false, var width) => $"u{width}", 
			EInt(true, var width) => $"i{width}", 
			_ => throw new NotImplementedException()
		};
		var tree = new PList { new PName("cast"), this, new PName(tn) };
		tree.Type = et;
		return tree;
	}

	public PTree Cast(EType et) {
		if(Type.ToString() == et.ToString() && et.Runtime == Type.Runtime) return this;
		var tn = et switch {
			EInt(false, var width) => $"u{width}", 
			EInt(true, var width) => $"i{width}", 
			_ => throw new NotImplementedException()
		};
		var tree = new PList { new PName("cast"), this, new PName(tn) };
		tree.Type = et;
		return tree;
	}
}

public class PList : PTree, IEnumerable<PTree> {
	public readonly List<PTree> Children = [];
	public PTree Head => Children.First();
	public int Count => Children.Count;
		
	public PList() {}
	public PList(IEnumerable<PTree> elems) => elems.ForEach(Add);

	public void Add(PTree child) => Children.Add(child);

	public PTree this[int index] => Children[index];

	public IEnumerator<PTree> GetEnumerator() => Children.GetEnumerator();
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public override string ToString() => $"[{Type}] ({(Type.Runtime ? "~runtime~ " : "")}{string.Join(' ', Children.Select(x => x.ToString()))})";

	public bool AnyRuntime => Type.Runtime || Children.Any(x => x.Type.Runtime);

	public PList AsCompiletime()
		=>
			!AnyRuntime
				? this
				: new(this.Select(x => x is PList sl ? sl.AsCompiletime() : x)) {Type = Type.AsCompiletime()};

	public PList HomogeneousRuntime() {
		if(!AnyRuntime) return this;
		return new(this.Take(1).Concat(this.Skip(1).Select(x => new PList(new[] { new PName("ensure-runtime"), x }) { Type = x.Type.AsRuntime() }))) { Type = Type.AsRuntime() };
	}

	public PList MapLeaves(Func<PTree, PTree> mapper) {
		var c = new List<PTree>();
		var modified = false;

		foreach(var child in this) {
			if(child is PList list) {
				var sub = list.MapLeaves(mapper);
				c.Add(sub);
				if(!ReferenceEquals(list, sub))
					modified = true;
			} else {
				var sub = mapper(child);
				c.Add(sub);
				if(!ReferenceEquals(child, sub))
					modified = true;
			}
		}
			
		return modified ? new(c) { Type = Type } : this;
	}

	public T WalkLeaves<T>(Func<PTree, T> mapper) where T : class {
		foreach(var child in this) {
			var ret = mapper(child);
			if(ret != null) return ret;
			if(child is PList list) {
				ret = list.WalkLeaves(mapper);
				if(ret != null) return ret;
			}
		}
		return null;
	}
	
	public void WalkLeaves(Action<PTree> mapper) {
		foreach(var child in this) {
			mapper(child);
			if(child is PList list)
				list.WalkLeaves(mapper);
		}
	}
}

public class PName : PTree {
	public readonly string Name;
	public PName(string name) => Name = name;
	public override string ToString() => Name;
	public void Deconstruct(out string name) => name = Name;
	public static implicit operator string(PName name) => name.Name;
}

public class PString : PTree {
	public readonly string String;
	public PString(string @string) => String = @string;
	public override string ToString() => String.ToPrettyString();
	public static implicit operator string(PString str) => str.String;
}

public class PInt : PTree {
	public readonly long Value;
	public PInt(long value) => Value = value;
	public override string ToString() => Value.ToString();
	public void Deconstruct(out long value) => value = Value;
	public static implicit operator long(PInt val) => val.Value;
}
	
public static class ListParser {
	public static PList Parse(string code) {
		var i = 0;
		return ParseList(code, ref i, true);
	}

	static PList ParseList(string code, ref int i, bool top = false) {
		var list = new PList();

		while(i < code.Length) {
			switch(code[i]) {
				case ' ': case '\t': case '\n': case '\r':
					i++;
					break;
				case '(':
					i++;
					list.Add(ParseList(code, ref i));
					break;
				case ')':
					i++;
					return list;
				case '-' when code[i + 1] >= '0' && code[i + 1] <= '9':
				case >= '0' and <= '9':
					list.Add(ParseInt(code, ref i));
					break;
				case '"': case '\'':
					list.Add(ParseString(code, ref i));
					break;
				default:
					list.Add(ParseName(code, ref i));
					break;
			}
		}
			
		if(!top)
			throw new("Reached end of file parsing non-top list");
			
		return list;
	}

	static PInt ParseInt(string code, ref int i) {
		var negative = false;
		if(code[i] == '-') {
			negative = true;
			i++;
		}

		var value = 0L;
		if(code[i] == '0' && code[i + 1] == 'b') {
			i += 2;
			while(code[i] == '0' || code[i] == '1' || code[i] == '_') {
				if(code[i] == '_')
					i++;
				else if(code[i++] == '0')
					value <<= 1;
				else
					value = (value << 1) | 1L;
			}
		} else if(code[i] == '0' && code[i + 1] == 'x') {
			i += 2;
			while(true) {
				if(code[i] == '_') {
				} else if(code[i] >= '0' && code[i] <= '9')
					value = (value << 4) | (uint) (code[i] - '0');
				else if(code[i] >= '0' && code[i] <= '9')
					value = (value << 4) | (uint) (code[i] - '0');
				else if(code[i] >= 'a' && code[i] <= 'f')
					value = (value << 4) | (uint) (code[i] - 'a' + 10);
				else if(code[i] >= 'A' && code[i] <= 'F')
					value = (value << 4) | (uint) (code[i] - 'A' + 10);
				else
					break;
				i++;
			}
		} else {
			while(code[i] == '_' || code[i] >= '0' && code[i] <= '9') {
				if(code[i] == '_')
					i++;
				else
					value = value * 10 + (uint) (code[i++] - '0');
			}
		}
		return new(negative ? -value : value);
	}

	static PTree ParseString(string code, ref int i) {
		var start = code[i++];
		var segs = new List<PTree>();
		var parsed = "";
		var done = false;

		void PushSeg() {
			if(parsed != "")
				segs.Add(new PString(parsed));
			parsed = "";
		}
		while(!done) {
			switch(code[i++]) {
				case '\\':
					parsed += code[i++];
					break;
				case var x when x == start:
					done = true;
					break;
				case '$':
					PushSeg();
					if(code[i] == '(') {
						i++;
						segs.Add(ParseList(code, ref i));
					} else
						segs.Add(ParseName(code, ref i, true));
					break;
				case var x:
					parsed += x;
					break;
			}
		}
		PushSeg();
		if(segs.Count == 0) return new PString("");
		if(segs.Count == 1) return segs[0];
		var ret = new PList { new PName("string-concat") };
		segs.ForEach(ret.Add);
		return ret;
	}

	static PName ParseName(string code, ref int i, bool inString = false) {
		var name = "";
		while(true) {
			switch(code[i++]) {
				case ' ': case '\t': case '\n': case '\r': case '(': case ')': case ',':
				case '.' when inString: case '[' when inString: case ']' when inString:
				case '$' when name.Length > 0:
				case '\'' when inString: case '"' when inString:
					i--;
					return new(name);
				case char x:
					name += x;
					break;
			}
		}
	}
}