using System;
using PrettyPrinter;

namespace CoreArchCompiler {
	public class BuiltinTypes {
		static EType TypeFromName(PTree expr) {
			if(expr is not PName name) throw new NotSupportedException($"Attempted to make type from expr {expr.ToPrettyString()}");

			var ns = name.Name;
			if(ns[0] == 'f') return new EFloat(int.Parse(ns[1..]));
			if(ns == "vec") return EType.Vector;
			return ns[0] == 'i' ? new EInt(true, int.Parse(ns[1..])) : new EInt(false, int.Parse(ns[1..]));
		}

		static EType LogicalType(EType a, EType b) {
			if(a is EInt || b is EInt) {
				if(a is not EInt ai) throw new NotSupportedException("Logical expression contains lhs that is non-int");
				if(b is not EInt bi) throw new NotSupportedException("Logical expression contains rhs that is non-int");
				return new EInt(
					ai.Signed == bi.Signed && ai.Signed,
					Math.Max(ai.Width, bi.Width)
				) { Runtime = ai.Runtime || bi.Runtime };
			}
			if(a is EFloat || b is EFloat) {
				if(a is not EFloat af) throw new NotSupportedException("Logical expression contains lhs that is non-float");
				if(b is not EFloat bf) throw new NotSupportedException("Logical expression contains rhs that is non-float");
				return new EFloat(Math.Max(af.Width, bf.Width)) { Runtime = af.Runtime || bf.Runtime };
			}
			throw new NotImplementedException("Logical expression has non-int/non-float type");
		}
	}
}