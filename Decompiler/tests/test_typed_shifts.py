"""Native shift spelling against an independent bit-position oracle."""
import html
import os
import test_pipeline as pipeline

HARNESS=r'''
using SharpRetro.Decompiler;
using LiftIl;
class Probe:SourceEmitter{
 protected override string SlotName(ValueSlot slot)=>slot.Kind==SlotKind.Vector?"wide":slot.Index==0?"a":"count";
 public string Function(string name,RecoveredValue value){declared.Clear();uses.Clear();output.Clear();string expr=Expression(value,true);return $"{Type(value.Width)} {name}(std::uint64_t a,std::uint64_t count,bits wide){{\n{output}{Type(value.Width)} result={expr};return result;}}";}
}
class Program{
 static int id;
 static IlType.I Ty(int n)=>new(false,n);
 static RecoveredValue Arg(int slot,int width=64)=>new(id++,width,null,[],Slot:new(width==128?SlotKind.Vector:SlotKind.Local,slot));
 static RecoveredValue Input(int width){var a=Arg(0,width==128?128:64);return width is 64 or 128?a:new(id++,width,new IlCast(Ty(width),CastKind.Trunc,new IlConst(Ty(64),0)),[a]);}
 static RecoveredValue Shift(int input,int width,UInt128? count,bool right){var a=Input(input);var b=count is {} n?new RecoveredValue(id++,64,null,[],Constant:n):Arg(1);return new(id++,width,new IlBin(Ty(width),right?BinOp.Shr:BinOp.Shl,new IlConst(Ty(input),0),new IlConst(Ty(64),0)),[a,b]);}
 static void Main(){var p=new Probe();int index=0;
 foreach(int input in new[]{8,16,32,64,128})foreach(int width in new[]{1,8,12,16,32,48,64,80,128})foreach(int count in new[]{0,1,width-1,width,width+1,128}.Distinct())foreach(bool right in new[]{false,true}){
  var s=p.Function("case_"+index,Shift(input,width,(UInt128)count,right));
  if((s.Contains("shift_left(")||s.Contains("shift_right("))!=(count>=width))throw new Exception("Unexpected shift spelling "+index);
  Console.WriteLine(s);Console.WriteLine("// CASE "+index+" "+input+" "+width+" "+count+" "+(right?1:0));index++;
 }
 Console.WriteLine(p.Function("dynamic",Shift(64,64,null,true)));
 Console.WriteLine(p.Function("huge",Shift(128,128,(UInt128)ulong.MaxValue,true)));
 foreach(int count in new[]{0,64,127}){
  var left=new RecoveredValue(id++,128,null,[],Constant:1);
  var right=new RecoveredValue(id++,64,null,[],Constant:(UInt128)count);
  var expr=new RecoveredValue(id++,128,new IlBin(Ty(128),BinOp.Shr,new IlConst(Ty(128),1),new IlConst(Ty(64),(UInt128)count)),[left,right]);
  Console.WriteLine(p.Function("literal_"+count,expr));
 }
 }
}
'''

class TypedShiftTests(pipeline.PipelineCase):
    def test_native_shifts_preserve_cross_width_truncation_and_extreme_counts(self):
        binaries=pipeline.DLL.parent
        refs=''.join(f'<Reference Include="{n}"><HintPath>{html.escape(str(binaries/(n+".dll")))}</HintPath></Reference>' for n in ('Decompiler','LiftIl'))
        project=self.root/'Harness.csproj'
        project.write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings></PropertyGroup><ItemGroup>{refs}</ItemGroup></Project>')
        (self.root/'Program.cs').write_text(HARNESS)
        (self.root/'NuGet.Config').write_text('<configuration><packageSources><clear /></packageSources></configuration>')
        built=pipeline.run('dotnet','build',project,'-c','Release','--verbosity','quiet','-p:UseSharedCompilation=false',env={**os.environ,'DOTNET_NOLOGO':'1'})
        self.assertEqual(built.returncode,0,built.stdout+built.stderr)
        emitted=pipeline.run('dotnet',self.root/'bin/Release/net10.0/Harness.dll')
        self.assertEqual(emitted.returncode,0,emitted.stdout+emitted.stderr)
        checks=[]
        for row in emitted.stdout.splitlines():
            if row.startswith('// CASE '):
                index,input_width,width,count,right=map(int,row.split()[2:])
                checks.append(f'if(case_{index}(a,count,wide)!=oracle({"wide" if input_width==128 else "bits(a)"},{input_width},{width},{count},{right}))return {index+1};')
        self.assertGreater(len(checks),400)
        source=f'#include "{pipeline.ROOT}/Decompiler/IntegerRuntime.hpp"\n#include <initializer_list>\nusing namespace sharp_retro;\n'+emitted.stdout+r'''
bits oracle(bits input,unsigned input_width,unsigned width,unsigned count,bool right){
 bits value=0;
 for(unsigned bit=0;bit<width;++bit){
  if(count>=width)continue;
  if(!right&&bit<count)continue;
  unsigned source=right?bit+count:bit-count;
  if(source<input_width&&source<width&&((input>>source)&1))value|=bits(1)<<bit;
 }
 return value;
}
int main(){for(std::uint64_t a:{0ULL,1ULL,0x80000000ULL,0x8000000000000000ULL,0xffffffffffffffffULL})
 for(bits wide:{bits(0),bits(1)<<127,~bits(0),(bits(1)<<80)+31})for(std::uint64_t count:{0ULL,1ULL,31ULL,63ULL,64ULL,127ULL,128ULL,0xffffffffffffffffULL}){
'''+''.join(checks)+r'''
 if(dynamic(a,count,wide)!=oracle(a,64,64,count>128?128:unsigned(count),true))return 2000;
 if(huge(a,count,wide)!=0)return 2001;
 if(literal_0(a,count,wide)!=1||literal_64(a,count,wide)!=0||literal_127(a,count,wide)!=0)return 2002;
}}
'''
        self.compile_run(source)
