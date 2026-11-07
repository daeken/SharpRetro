"""PE metadata, explicit Win64 entry state, memory imports and XMM copies."""
import struct

import test_pipeline as pipeline
import test_values as values
from test_functions import Code

BASE=0x180000000


def image(code):
    code=code.finish() if isinstance(code,Code) else bytes.fromhex(code)
    assert len(code)<=512
    data=bytearray(0x1000)
    data[:2]=b"MZ";struct.pack_into("<I",data,60,0x80);data[0x80:0x84]=b"PE\0\0"
    struct.pack_into("<HHIIIHH",data,0x84,0x8664,6,0,0,0,240,0x2022)
    opt=0x98;struct.pack_into("<H",data,opt,0x20b)
    struct.pack_into("<I",data,opt+16,0x1000);struct.pack_into("<Q",data,opt+24,BASE)
    struct.pack_into("<II",data,opt+32,0x1000,0x200)
    struct.pack_into("<II",data,opt+56,0x7000,0x400);struct.pack_into("<I",data,opt+108,16)
    for directory,rva,size in ((0,0x3000,0x100),(1,0x4000,40),(3,0x5000,12),(5,0x6000,12)):
        struct.pack_into("<II",data,opt+112+directory*8,rva,size)
    for i,name in enumerate((b".text",b".rdata",b".edata",b".idata",b".pdata",b".reloc")):
        at=opt+240+i*40;data[at:at+len(name)]=name
        struct.pack_into("<IIII",data,at+8,len(code) if i==0 else 512,(i+1)*0x1000,512,0x400+i*512)
        struct.pack_into("<I",data,at+36,0x60000020 if i==0 else 0xc0000040 if i==3 else 0x40000040)
    data[0x400:0x400+len(code)]=code
    data[0x600]=1;struct.pack_into("<Q",data,0x700,BASE+0x2010)
    struct.pack_into("<IIIIII",data,0x810,1,1,1,0x3030,0x3040,0x3050)
    struct.pack_into("<I",data,0x830,0x1000);struct.pack_into("<I",data,0x840,0x3060)
    data[0x860:0x867]=b"target\0"
    struct.pack_into("<IIIII",data,0xa00,0x4040,0,0,0x4070,0x4060)
    struct.pack_into("<Q",data,0xa40,0x4090);struct.pack_into("<Q",data,0xa60,0x4090)
    data[0xa70:0xa7b]=b"msvcrt.dll\0";data[0xa92:0xa99]=b"memcpy\0"
    struct.pack_into("<III",data,0xc00,0x1000,0x1000+len(code),0x2000)
    struct.pack_into("<IIHH",data,0xe00,0x2000,12,0xa100,0)
    return data


def with_memcpy(after=""):
    code=(Code().emit("4883ec28").call("thunk").emit("4883c428 "+after+" c3").label("thunk"))
    displacement=0x4060-(0x1000+len(code.bytes)+6)
    return code.emit("ff25").emit(struct.pack("<i",displacement).hex())


class PeFunctionTests(pipeline.PipelineCase):
    driver=values.ValueRecoveryTests.driver
    execute=values.ValueRecoveryTests.execute

    def emit(self,data,*options,success=True):
        self.image.write_bytes(data);self.source=self.root/"values.cpp"
        result=self.cli("decompile-function",self.image,"--export","target","--output",self.source,*options)
        if success: self.assertEqual(result.returncode,0,result.stderr)
        else:
            self.assertEqual(result.returncode,1,result.stderr)
            self.assertFalse(self.source.exists())
        return result

    def test_pe_metadata_records_exports_iat_runtime_ranges_and_relocations(self):
        result=self.inspect(image("31c0c3"))
        self.assertEqual(result["Exports"]["target"]["Address"],BASE+0x1000)
        self.assertEqual(result["PeImportSlots"][str(BASE+0x4060)],"memcpy")
        self.assertEqual(result["PeRuntimeFunctions"][0]["Begin"],BASE+0x1000)
        self.assertEqual(result["PeRelocations"],[{"Address":BASE+0x2100,"Type":10}])

    def test_win64_register_arguments_are_rcx_rdx_r8_r9(self):
        self.emit(image("4889c8 4801d0 4c01c0 4c01c8 c3"))
        self.driver(self.source,"""
int main() { return sharp_retro::source::recovered_export(1,2,4,8)==15 ? 0 : 1; }
""")

    def test_preserved_entry_inputs_are_explicit_and_not_silently_zeroed(self):
        self.emit(image("89d8 c3"))
        self.assertIn("incoming_local_3=0",self.source.read_text())
        self.driver(self.source,"""
int main() { return sharp_retro::source::recovered_export(0,0,0,0,0x123456789abcdef0ULL)==0x9abcdef0ULL ? 0 : 1; }
""")

    def test_thunk_and_original_first_thunk_fallback_use_windows_memory_bridge(self):
        for fallback in (False,True):
            with self.subTest(fallback=fallback):
                data=image(with_memcpy())
                if fallback: struct.pack_into("<I",data,0xa00,0)
                self.emit(data)
                self.driver(self.source,"""
int main() {
  unsigned char source[]={1,2,3,4},dest[4]{};
  auto result=sharp_retro::source::recovered_export(reinterpret_cast<std::uintptr_t>(dest),reinterpret_cast<std::uintptr_t>(source),4);
  return result==reinterpret_cast<std::uintptr_t>(dest) && std::memcmp(source,dest,4)==0 ? 0 : 1;
}
""")

    def test_volatile_import_result_cannot_be_assumed_preserved(self):
        self.assertIn("volatile register",self.emit(image(with_memcpy("4889c8")),success=False).stderr)

    def test_unknown_import_is_refused_without_output(self):
        data=image(with_memcpy());data[0xa92:0xa99]=b"malloc\0"
        self.assertIn("Unmodeled Windows import",self.emit(data,success=False).stderr)

    def test_vector_copy_preserves_unaligned_and_overlapping_bytes(self):
        self.emit(image("f30f6f01 f30f7f02 31c0 c3"))
        self.driver(self.source,"""
int main() {
  std::array<unsigned char,40> data{},expected{};
  for(unsigned i=0;i<data.size();++i) data[i]=i*7;
  expected=data; auto old=data;
  std::memcpy(expected.data()+3,old.data()+1,16);
  auto result=sharp_retro::source::recovered_export(reinterpret_cast<std::uintptr_t>(data.data()+1),reinterpret_cast<std::uintptr_t>(data.data()+3));
  return result==0 && data==expected ? 0 : 1;
}
""")

    def test_unproved_aligned_vector_fault_and_unknown_vector_input_refuse(self):
        for code in ("660f6f01 f30f7f02 31c0 c3","f30f7f01 31c0 c3"):
            with self.subTest(code=code): self.emit(image(code),success=False)

    def test_explicit_checked_mode_and_default_dllmain_are_not_selected(self):
        self.emit(image("31c0c3"),"--memory","checked",success=False)
        self.image.write_bytes(image("31c0c3"))
        result=self.cli("decompile-function",self.image)
        self.assertEqual(result.returncode,1)
        self.assertIn("explicit",result.stderr)

    def test_forwarded_export_and_unterminated_forwarder_are_refused(self):
        data=image("31c0c3");struct.pack_into("<I",data,0x830,0x3080);data[0x880:0x88b]=b"other.func\0"
        self.assertIn("Forwarded",self.emit(data,success=False).stderr)
        data[0x880:0x900]=b"x"*128
        self.assertIn("Unterminated",self.emit(data,success=False).stderr)

    def test_malformed_image_sizes_export_ordinals_imports_and_relocs_refuse(self):
        variants=[]
        data=image("31c0c3");struct.pack_into("<I",data,0x98+56,0x2000);variants.append(data)
        data=image("31c0c3");struct.pack_into("<I",data,0x98+60,16);variants.append(data)
        data=image("31c0c3");struct.pack_into("<H",data,0x850,2);variants.append(data)
        data=image("31c0c3");data[0xa14:0xa28]=bytes([1])*20;variants.append(data)
        data=image("31c0c3");struct.pack_into("<I",data,0xe04,7);variants.append(data)
        data=image("31c0c3");struct.pack_into("<I",data,0xc04,0x1000);variants.append(data)
        for data in variants:
            with self.subTest(sha=__import__('hashlib').sha256(data).hexdigest()):
                self.image.write_bytes(data);self.assertEqual(self.cli("inspect",self.image).returncode,1)

    def test_leaf_export_does_not_require_a_pdata_entry(self):
        data=image("b82a000000c3");struct.pack_into("<II",data,0x98+112+3*8,0,0)
        self.emit(data)
        self.driver(self.source,"int main() { return sharp_retro::source::recovered_export()==42 ? 0 : 1; }")

    def test_dynamic_and_spilled_image_addresses_and_image_writes_refuse(self):
        lea="488d0d"+struct.pack("<i",0x2000-(0x1000+7)).hex()
        for suffix in ("4801d1 8b01 c3","c60100 31c0 c3","4883ec08 48890c24 31c0 4883c408 c3"):
            with self.subTest(suffix=suffix): self.emit(image(lea+suffix),success=False)

    def test_internal_call_cannot_hide_image_address_provenance(self):
        lea="488d0d"+struct.pack("<i",0x2000-(0x1000+7)).hex()
        code=Code().emit(lea).call("helper").emit("c3").label("helper").emit("c60100 31c0 c3")
        self.assertIn("internal PE call",self.emit(image(code),success=False).stderr)

    def test_import_literal_source_needs_a_range(self):
        # RCX=external destination, RDX=image literal, R8=unknown length.
        lead="488d15"+struct.pack("<i",0x2000-(0x1000+7)).hex()
        code=Code().emit(lead+"4883ec28").call("thunk").emit("4883c428 c3").label("thunk")
        code.emit("ff25"+struct.pack("<i",0x4060-(0x1000+len(code.bytes)+6)).hex())
        self.assertIn("bounded literal read",self.emit(image(code),success=False).stderr)

    def test_segment_tls_dependent_function_refuses(self):
        self.assertIn("TLS",self.emit(image("65488b042530000000 c3"),success=False).stderr)

    def test_nonimage_internal_helper_keeps_direct_windows_arguments(self):
        code=Code().call("helper").emit("c3").label("helper").emit("8d4107 c3")
        self.emit(image(code))
        self.driver(self.source,"int main() { return sharp_retro::source::recovered_export(5)==12 ? 0 : 1; }")

    def test_image_pointer_cannot_escape_through_helper_return(self):
        code=Code().call("helper").emit("488b00 c3").label("helper")
        code.emit("488d05"+struct.pack("<i",0x2000-(0x1000+len(code.bytes)+7)).hex()+" c3")
        self.assertIn("return escapes",self.emit(image(code),success=False).stderr)

    def test_known_aligned_frame_copy_is_admitted(self):
        self.emit(image("4883ec18 f30f6f01 0f290424 0f280424 f30f7f02 4883c418 31c0 c3"))
        self.driver(self.source,"""
int main() {
  std::array<unsigned char,16> a{},b{};
  for(unsigned i=0;i<a.size();++i) a[i]=i+17;
  sharp_retro::source::recovered_export(reinterpret_cast<std::uintptr_t>(a.data()),reinterpret_cast<std::uintptr_t>(b.data()));
  return a==b ? 0 : 1;
}
""")

    def test_unmodeled_relocation_refuses_function_profile(self):
        data=image("31c0c3");struct.pack_into("<H",data,0xe08,0x5100)
        self.assertIn("DIR64",self.emit(data,success=False).stderr)

    def test_non_temporal_vector_copies_are_not_silently_downgraded(self):
        for code in ("660fe701 31c0 c3","0f2b01 31c0 c3"):
            with self.subTest(code=code): self.emit(image(code),success=False)

    def test_void_internal_helper_needs_no_fabricated_accumulator(self):
        code=Code().call("helper").emit("31c0 c3").label("helper").emit("c3")
        self.emit(image(code))
        self.driver(self.source,"int main() { return sharp_retro::source::recovered_export()==0 ? 0 : 1; }")

    def test_image_pointer_cannot_escape_as_a_nonaccumulator_helper_result(self):
        code=Code().call("helper").emit("488b03 c3").label("helper")
        code.emit("488d1d"+struct.pack("<i",0x2000-(0x1000+len(code.bytes)+7)).hex()+" 31c0 c3")
        self.assertIn("return escapes",self.emit(image(code),success=False).stderr)

    def test_absolute_image_immediates_and_relocated_instructions_refuse(self):
        code="48b8"+struct.pack("<Q",BASE+0x2000).hex()+" 8b00 c3"
        self.assertIn("preferred-image",self.emit(image(code),success=False).stderr)
        data=image(code);struct.pack_into("<IIHH",data,0xe00,0x1000,12,0xa002,0)
        self.assertIn("Relocation in selected",self.emit(data,success=False).stderr)

    def test_known_import_thunk_can_lie_outside_selected_function_range(self):
        self.emit(image(with_memcpy()),"--range",f"{BASE+0x1000}:14")
        self.assertIn("std::memcpy",self.source.read_text())

    def test_literal_access_requires_read_permission(self):
        code="488d05"+struct.pack("<i",0x2000-(0x1000+7)).hex()+" 8b00 c3"
        data=image(code);struct.pack_into("<I",data,0x98+240+40+36,0x40)
        self.assertIn("admitted data",self.emit(data,success=False).stderr)
