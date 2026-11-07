"""Exact carrier checks for typed expression cleanup, including nonstandard widths."""
import html
import os

import test_pipeline as pipeline


HARNESS=r'''
using SharpRetro.Decompiler;
using LiftIl;
class Probe : SourceEmitter {
    protected override string SlotName(ValueSlot slot) => slot.Kind==SlotKind.Vector ? "wide" : slot.Index==0 ? "a" : "b";
    public string Function(string name,RecoveredValue value) {
        declared.Clear(); uses.Clear(); output.Clear();
        string expression=Expression(value,destinationTyped:true);
        return $"{Type(value.Width)} {name}(std::uint64_t a,std::uint64_t b,bits wide) {{\n"+
            output.ToString()+$"{Type(value.Width)} result={expression};\nreturn result;\n}}\n";
    }
}
class Program {
    static int id;
    static IlType.I Ty(int n) => new(false,n);
    static RecoveredValue Arg(int slot,int width=64) => new(id++,width,null,[],Slot:new(width==128 ? SlotKind.Vector : SlotKind.Local,slot));
    static RecoveredValue C(int width,UInt128 bits,bool relative=false) => new(id++,width,null,[],Constant:bits,PcRelative:relative);
    static RecoveredValue Cast(RecoveredValue value,int width,CastKind kind) => new(id++,width,new IlCast(Ty(width),kind,new IlConst(Ty(value.Width),0)),[value]);
    static RecoveredValue Add(RecoveredValue a,RecoveredValue b,BinOp op=BinOp.Add) => new(id++,a.Width,new IlBin(Ty(a.Width),op,new IlConst(Ty(a.Width),0),new IlConst(Ty(b.Width),0)),[a,b]);
    static void Main() {
        var p=new Probe(); var a=Arg(0);var b=Arg(1);var wide=Arg(0,128);
        Console.WriteLine(p.Function("sub64",Add(a,C(64,ulong.MaxValue-31))));
        Console.WriteLine(p.Function("sub128",Add(wide,C(128,UInt128.MaxValue-31))));
        Console.WriteLine(p.Function("sub128half",Add(wide,C(128,UInt128.One<<127))));
        Console.WriteLine(p.Function("relative_literal",Add(a,C(64,ulong.MaxValue-31,relative:true))));
        Console.WriteLine(p.Function("mask64",Add(a,C(64,ulong.MaxValue^15),BinOp.And)));
        Console.WriteLine(p.Function("mask128",Add(wide,C(128,UInt128.MaxValue^15),BinOp.And)));
        Console.WriteLine(p.Function("low_bit",Cast(a,1,CastKind.Trunc)));
        Console.WriteLine(p.Function("low12",Cast(a,12,CastKind.Trunc)));
        var za=Cast(Cast(a,16,CastKind.Trunc),64,CastKind.Zext);
        var zb=Cast(Cast(b,16,CastKind.Trunc),64,CastKind.Zext);
        Console.WriteLine(p.Function("product16",Add(za,zb,BinOp.Mul)));
    }
}
'''


class ExpressionContextTests(pipeline.PipelineCase):
    def test_typed_assignments_keep_nested_widths_and_128bit_wrap(self):
        binaries=pipeline.DLL.parent
        references=''.join(f'<Reference Include="{name}"><HintPath>{html.escape(str(binaries/(name+".dll")))}</HintPath></Reference>'
                           for name in ('Decompiler','LiftIl'))
        (self.root/'Harness.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk">
<PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
<ItemGroup>{references}</ItemGroup></Project>''')
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        (self.root/'Program.cs').write_text(HARNESS)
        env={**os.environ,'DOTNET_CLI_TELEMETRY_OPTOUT':'1','DOTNET_NOLOGO':'1'}
        built=pipeline.run('dotnet','build',self.root/'Harness.csproj','-c','Release','--verbosity','quiet',env=env)
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stderr)
        relative=emitted.stdout.split('relative_literal(',1)[1].split('bool low_bit',1)[0]
        self.assertIn('+0xffffffffffffffe0ULL',relative)
        self.assertIn('-0x20ULL',emitted.stdout)
        self.assertIn('&~0xfULL',emitted.stdout)
        self.assertIn('&~bits(0xfULL)',emitted.stdout)
        self.compile_run(f'#include "{pipeline.ROOT}/Decompiler/IntegerRuntime.hpp"\n#include <initializer_list>\nusing namespace sharp_retro;\n'+emitted.stdout+r'''
int main() {
  bits wides[]={0,1,~bits(0),bits(1)<<127,(bits(1)<<127)+17};
  for(std::uint64_t a : {0ULL,1ULL,2ULL,3ULL,65535ULL,0x80000000ULL,0xffffffffffffffffULL})
    for(std::uint64_t b : {0ULL,1ULL,65535ULL,0xffffffffffffffffULL}) for(bits wide : wides) {
      if(sub64(a,b,wide)!=a-32 || relative_literal(a,b,wide)!=a-32) return 1;
      if(sub128(a,b,wide)!=wide-bits(32) || sub128half(a,b,wide)!=wide-(bits(1)<<127)) return 2;
      if(low_bit(a,b,wide)!=bool(a&1) || low12(a,b,wide)!=(a&4095)) return 3;
      if(product16(a,b,wide)!=std::uint64_t(std::uint16_t(a))*std::uint16_t(b)) return 4;
      if(mask64(a,b,wide)!=(a&~15ULL) || mask128(a,b,wide)!=(wide&~bits(15))) return 5;
    }
}
''')
