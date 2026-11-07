"""Native pointer carriers and observed object views execute with exact bytes."""
import test_pipeline as pipeline
import test_values as values
import test_native_memory as native_memory

class NativeTypeEmissionTests(pipeline.PipelineCase):
    recover=native_memory.NativeMemoryTests.recover
    driver=values.ValueRecoveryTests.driver
    execute=values.ValueRecoveryTests.execute

    def test_readonly_fixed_fields_use_observed_accessors(self):
        source=self.recover(bytes.fromhex('8b07 8b4f08 4801c8 c3'))
        text=source.read_text()
        self.assertIn('const u8* argument_0',text)
        self.assertIn('object_view_1000_7',text)
        self.assertIn('.field_8()',text)
        self.driver(source,r'''
int main() {
  for(std::uint32_t a : {0u,1u,0x80000000u,0xffffffffu})
    for(std::uint32_t b : {0u,1u,0x80000000u,0xffffffffu}) {
      std::array<unsigned char,24> bytes;bytes.fill(0xa5);
      std::memcpy(bytes.data(),&a,4);std::memcpy(bytes.data()+8,&b,4);
      auto before=bytes;
      if(sharp_retro::source::fn_1000(sharp_retro::source::address(bytes.data()))!=std::uint64_t(a)+b || bytes!=before)return 1;
    }
}
''')

    def test_overlapping_fields_keep_alias_views(self):
        source=self.recover(bytes.fromhex('488b07 8b4f04 4831c8 c3'))
        self.assertNotIn('object_view_1000_',source.read_text())
        self.driver(source,r'''
int main() {
  std::array<unsigned char,24> bytes;bytes.fill(0xa5);
  std::uint64_t wide=0xfedcba9876543210ULL;std::memcpy(bytes.data()+1,&wide,8);
  auto before=bytes;
  auto value=sharp_retro::source::fn_1000(sharp_retro::source::address(bytes.data()+1));
  if(value!=(wide^std::uint32_t(wide>>32)) || bytes!=before)return 1;
}
''')

    def test_loaded_pointer_is_native_and_parent_const_does_not_freeze_bytes(self):
        source=self.recover(bytes.fromhex('488b07 c6007b 0fb64708 c3'))
        text=source.read_text()
        self.assertIn('const u8* argument_0',text)
        self.assertRegex(text,r'u8\s*\*\s+(?:value_|variable_)')
        self.assertIn('.field_8()',text)
        self.driver(source,r'''
int main() {
  for(unsigned offset : {0u,1u,7u}) {
    std::array<unsigned char,48> bytes;bytes.fill(0xa5);
    auto base=bytes.data()+offset;
    std::uint64_t target=sharp_retro::source::address(base+8);
    std::memcpy(base,&target,8);auto expected=bytes;expected[offset+8]=0x7b;
    auto result=sharp_retro::source::fn_1000(sharp_retro::source::address(base));
    if(result!=0x7b || bytes!=expected)return 1;
  }
}
''')

    def test_const_source_rereads_mutable_alias_at_o3(self):
        source=self.recover(bytes.fromhex('0fb607 c6067b 0fb60f 31c8 c3'))
        self.assertIn('const u8* argument_0,u8* argument_1',source.read_text())
        driver=self.root/'alias-o3.cpp'
        driver.write_text(f'#define main recovered_cli_main\n#include "{source.name}"\n#undef main\n#include <initializer_list>\n'+r'''
int main() {
  for(bool same : {false,true}) {
    std::array<unsigned char,16> bytes;bytes.fill(0xa5);bytes[1]=5;bytes[2]=9;
    auto expected=bytes;expected[same?1:2]=0x7b;
    auto value=sharp_retro::source::fn_1000(sharp_retro::source::address(bytes.data()+1),sharp_retro::source::address(bytes.data()+(same?1:2)));
    if(value!=(same ? (5^0x7b) : 0) || bytes!=expected)return 1;
  }
}
''')
        binary=self.root/'alias-o3'
        built=pipeline.run('clang++','-std=c++20','-O3','-fstrict-aliasing','-Wall','-Wextra','-Werror','-Wno-unused-parameter','-fsanitize=address,undefined',driver,'-o',binary)
        self.assertEqual(built.returncode,0,built.stderr)
        ran=pipeline.run(binary)
        self.assertEqual(ran.returncode,0,ran.stderr)
