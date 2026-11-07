"""Portable libc ABI helpers and instruction-internal division fault guards."""
import test_pipeline as pipeline
import test_values as values


class ImportAndFaultTests(pipeline.PipelineCase):
    recover=values.ValueRecoveryTests.recover
    execute=values.ValueRecoveryTests.execute
    driver=values.ValueRecoveryTests.driver

    def runtime(self,checked=False):
        root=pipeline.ROOT/"Decompiler"
        result=f'#include "{root}/IntegerRuntime.hpp"\n'
        result+=f'#include "{root}/{"NativeRuntime.hpp" if checked else "NativeObjectsRuntime.hpp"}"\n'
        if checked: result+="#define SHARP_RETRO_CHECKED_MEMORY\n"
        return result+f'#include "{root}/HostImportsRuntime.hpp"\n'

    def test_formatted_io_uses_typed_conversions_and_refuses_unsupported_varargs(self):
        output=self.compile_run(self.runtime()+r'''
int main() {
  using namespace sharp_retro::source;
  if(format_output(stdout,address("%s %% %c"),{address("hello"),65},0)!=9) return 1;
  bool bad_format=false,bad_vector=false;
  try { format_output(stdout,address("%d"),{1},0); } catch(const std::runtime_error&) { bad_format=true; }
  try { format_output(stdout,address(""),{},1); } catch(const std::runtime_error&) { bad_vector=true; }
  return bad_format && bad_vector ? 0 : 2;
}
''')
        self.assertEqual(output,"hello % A")

    def test_stdio_four_arguments_transfer_binary_data_and_preserve_counts(self):
        self.compile_run(self.runtime()+r'''
int main() {
  using namespace sharp_retro::source;
  auto* stream=std::tmpfile(); if(!stream) return 1;
  unsigned char input[]={0,127,128,255,10,13},output[6]{};
  if(host_fwrite(address(input),2,3,address(stream))!=3) return 2;
  std::rewind(stream);
  if(host_fread(address(output),2,3,address(stream))!=3) return 3;
  auto same=std::memcmp(input,output,6)==0;
  std::fclose(stream); return same ? 0 : 4;
}
''')

    def test_checked_heap_lifetime_is_removed_on_free(self):
        self.compile_run(self.runtime(checked=True)+r'''
int main() {
  using namespace sharp_retro::source;
  auto allocation=host_allocate(17); if(!allocation) return 1;
  write_memory<std::uint8_t>(allocation+16,42);
  if(read_memory<std::uint8_t>(allocation+16)!=42) return 2;
  host_release(allocation);
  try { check_memory(allocation,1,false); return 3; } catch(const std::runtime_error&) {}
  host_release(0); return 0;
}
''')

    def test_unsigned_division_preserves_results(self):
        source=self.recover("89f8 31d2 b914000000 f7f1 c3",memory="native")
        self.driver(source,"""
int main() {
  for(unsigned n=0;n<2000;++n)
    if(sharp_retro::source::recovered_program(n,nullptr)!=int((n/20)&255)) return 1;
}
""")

    def test_division_faults_are_not_removed_with_dead_results(self):
        for code in ("89f8 31d2 31c9 f7f1 c3",  # zero divisor
                     "31c0 ba01000000 b901000000 f7f1 c3",  # quotient overflow
                     "31c0 ba01000000 b901000000 f7f1 b807000000 c3"):  # dead quotient
            with self.subTest(code=code):
                source=self.recover(code,memory="native")
                result=self.execute(source)
                self.assertEqual(result.returncode,125,result.stderr)
                self.assertIn("x86 fault vector 0",result.stderr)
