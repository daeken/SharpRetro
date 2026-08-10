use sharpretro_jit::aarch64_enc::Aarch64Enc;
fn main() {
    let mut e = Aarch64Enc::new();
    e.ldr_q(0, 27, 0);       // ldr q0, [x27]
    e.ldr_q(1, 27, 16);      // ldr q1, [x27, #16]
    e.str_q(2, 27, 32);      // str q2, [x27, #32]
    e.zip1_v(2, 0, 1, 0);    // zip1 v2.16b, v0.16b, v1.16b  (PUNPCKLBW)
    e.zip1_v(2, 0, 1, 1);    // zip1 v2.8h,  v0.8h,  v1.8h   (PUNPCKLWD)
    e.zip1_v(2, 0, 1, 2);    // zip1 v2.4s,  v0.4s,  v1.4s   (PUNPCKLDQ)
    e.zip1_v(2, 0, 1, 3);    // zip1 v2.2d,  v0.2d,  v1.2d   (PUNPCKLQDQ)
    e.zip2_v(2, 0, 1, 0);    // zip2 v2.16b, v0.16b, v1.16b  (PUNPCKHBW)
    e.eor_v16b(2, 0, 1);     // eor v2.16b, v0.16b, v1.16b   (PXOR)
    e.and_v16b(2, 0, 1);
    e.orr_v16b(2, 0, 1);
    let bytes: Vec<u8> = e.buf.iter().flat_map(|w| w.to_le_bytes()).collect();
    std::fs::write("/tmp/enc_neon.bin", &bytes).unwrap();
    println!("wrote {} words", e.buf.len()); _batch2(); _batch3(); _batch4();
}
// Second batch: INS/UMOV
fn _batch2() {
    let mut e = sharpretro_jit::aarch64_enc::Aarch64Enc::new();
    e.ins_vd_x(0, 0, 9);   // mov v0.d[0], x9
    e.ins_vd_x(0, 1, 11);  // mov v0.d[1], x11
    e.umov_x_vd(9, 2, 0);  // mov x9, v2.d[0]
    e.umov_x_vd(11, 2, 1); // mov x11, v2.d[1]
    let bytes: Vec<u8> = e.buf.iter().flat_map(|w| w.to_le_bytes()).collect();
    std::fs::write("/tmp/enc_neon2.bin", &bytes).unwrap();
}
fn _batch3() {
    let mut e = sharpretro_jit::aarch64_enc::Aarch64Enc::new();
    e.fmov_d_x(0, 9); e.fmov_x_d(9, 0);
    e.fmov_s_w(0, 9); e.fmov_w_s(9, 0);
    e.scvtf_d_x(0, 9); e.scvtf_s_x(0, 9); e.scvtf_d_w(0, 9); e.scvtf_s_w(0, 9);
    e.ucvtf_d_x(0, 9);
    e.fcvtzs_x_d(9, 0); e.fcvtzs_x_s(9, 0); e.fcvtzs_w_d(9, 0);
    e.fcvt_d_s(0, 1); e.fcvt_s_d(0, 1);
    let bytes: Vec<u8> = e.buf.iter().flat_map(|w| w.to_le_bytes()).collect();
    std::fs::write("/tmp/enc_neon3.bin", &bytes).unwrap();
}
fn _batch4() {
    let mut e = sharpretro_jit::aarch64_enc::Aarch64Enc::new();
    e.fadd_d(0,1,2); e.fsub_d(0,1,2); e.fmul_d(0,1,2); e.fdiv_d(0,1,2);
    e.fadd_s(0,1,2); e.fsub_s(0,1,2); e.fmul_s(0,1,2); e.fdiv_s(0,1,2);
    e.fsqrt_d(0,1); e.fneg_d(0,1); e.fabs_d(0,1); e.fsqrt_s(0,1);
    e.fcmp_d(0,1); e.fcmp_s(0,1); e.mrs_nzcv(9);
    let bytes: Vec<u8> = e.buf.iter().flat_map(|w| w.to_le_bytes()).collect();
    std::fs::write("/tmp/enc_neon4.bin", &bytes).unwrap();
}
