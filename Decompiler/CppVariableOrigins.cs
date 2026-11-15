using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LiftIl;

namespace SharpRetro.Decompiler;

public sealed record CppVariable(string Origin, ulong InstructionRva, int TemporaryId, int Width, string Name, string Definition);
public sealed record CppVariableAnnotation(string Name, string? Type = null);
public sealed record CppEmissionOptions(string Namespace, string RuntimeHeader,
    IReadOnlyDictionary<string,CppVariableAnnotation>? Annotations = null, Pe32ObjectPlan? Objects = null,
    bool ExternalDispatch = false);

/// Conservative origins for the machine backend's instruction-local values.
/// Display numbers and temporary IDs are not persistent identities. A changed
/// normalized instruction IL intentionally orphans its annotations.
public static class CppVariableOrigins {
    public static IReadOnlyList<CppVariable> Collect(LiftedProgram program) {
        var result = new List<CppVariable>();
        foreach(var instruction in program.Instructions.OrderBy(i=>i.Address)) {
            var ids = new Dictionary<string,int>();
            string Normalize(string text) => Regex.Replace(text,@"%([0-9]+)",m=> {
                string key=m.Groups[1].Value;
                if(!ids.ContainsKey(key)) ids[key]=ids.Count;
                return "%"+ids[key];
            });
            string normalized=Normalize(instruction.Body.ToString());
            string digest=Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(normalized)));
            var lets=Lets(instruction.Body).ToArray();
            if(lets.Select(l=>l.Id).Distinct().Count()!=lets.Length)
                throw new NotSupportedException("Annotation origins require unique instruction-local definitions");
            foreach(var let in lets) {
                if(let.Val.Ty is not IlType.I { Bits: >0 and <=128 } type)
                    throw new NotSupportedException("Annotation origins require integer values");
                ulong rva=checked(instruction.Address-program.Image.ImageBase);
                string origin=$"il-temp-v1:{rva:x8}:{digest}:{ids[let.Id.ToString()]}";
                result.Add(new(origin,rva,let.Id,type.Bits,$"variable_{result.Count}",Normalize(let.Val.ToString())));
            }
        }
        return result;
    }

    static IEnumerable<IlLet> Lets(Il node) => node switch {
        IlLet let=>[let], IlBlock block=>block.Body.SelectMany(Lets),
        IlIf conditional=>conditional.Then.Concat(conditional.Else).SelectMany(Lets), _=>[]
    };

    public static string Type(string? annotation,int width) => annotation switch {
        null or "bits"=>"bits", "bool" when width==1=>"bool",
        "u8" when width<=8=>"std::uint8_t", "u16" when width<=16=>"std::uint16_t",
        "u32" when width<=32=>"std::uint32_t", "u64" when width<=64=>"std::uint64_t",
        _=>throw new NotSupportedException($"Type {annotation} cannot represent this {width}-bit value; signed/pointer/layout overrides are not implemented")
    };
}
