using System.Diagnostics;
using LibSharpRetro;

namespace DamageCore;

enum Mode {
	HBlank,
	VBlank, 
	SearchingOAM, 
	Drawing
}

public class Ppu {
	readonly Core Core;
	readonly IFramebufferBackend FramebufferBackend;

	byte Lcdc;
	Mode Mode;
	byte SCY, SCX, LY, LYC, WY, WX;
	byte BGP = 0b11100100, OBP0 = 0b11100100, OBP1 = 0b11100100;
	
	public Ppu(Core core) {
		Core = core;
		FramebufferBackend = core.FramebufferBackend;
		FramebufferBackend.Resolution = (160 * 2, 160);
		core.Timing.TimeSync(Run());
	}

	IEnumerable<ulong> Run() {
		var pbuf = FramebufferBackend.Framebuffer;
		while(true) {
			while(!Lcdc.HasBit(7)) {
				yield return 70224;
			}
			for(LY = 0; LY < 144; ++LY) {
				Mode = Mode.SearchingOAM;
				var sprites = new List<(int X, int YOffset, int TileIndex, byte Flags)>();
				var objEnabled = Lcdc.HasBit(1);
				var tallMode = Lcdc.HasBit(2);
				if(objEnabled) {
					var oam = Core.Memory.ReadBlock(0xFE00, 40 * 4);
					var oi = 0;
					for(var i = 0; i < 40 && sprites.Count < 10; ++i, oi += 4) {
						var y = oam[oi];
						if(LY + 16 < y || LY + 16 >= y + (tallMode ? 16 : 8)) continue;
						var flags = oam[oi + 3];
						var sy = (LY + 16) - y;
						if(flags.HasBit(6))
							sy = (tallMode ? 15 : 7) - sy;
						Debug.Assert(sy >= 0 && sy < (tallMode ? 16 : 8));
						var tin = oam[oi + 2];
						sprites.Add((oam[oi + 1], sy, tallMode ? (sy < 8 ? tin & 0xFE : tin | 1) : tin, flags));
					}
				}
				yield return 80;

				var upperIndexing = !Lcdc.HasBit(4);
				var tileDataAddr = upperIndexing ? 0x9000 : 0x8000;
				var bgMapAddr = Lcdc.HasBit(3) ? 0x9C00 : 0x9800;
				var bgMap = Core.Memory.ReadBlock((ushort) bgMapAddr, 0x400);
				var bgy = LY + SCY;
				for(var x = 0; x < 160; ++x) {
					var bgx = x + SCX;
					var tmIndex = ((bgy / 8) % 32) * 32 + ((bgx / 8) % 32);
					var tileIndex = (int) bgMap[tmIndex];
					if(upperIndexing)
						tileIndex = (sbyte) tileIndex;
					var tile = Core.Memory.ReadBlock((ushort) (tileDataAddr + tileIndex * 16 + (bgy % 8) * 2), 2);
					var offset = 7 - (bgx % 8);
					var bgColor = ((tile[0] >> offset) & 1) | (((tile[1] >> offset) & 1) << 1);
					var mappedBgColor = (BGP >> (2 * bgColor)) & 0b11;

					int color;
					if(objEnabled) {
						var objColor = (int?) null;
						var objX = 1000;
						foreach(var (sx, sy, sti, flags) in sprites) {
							if(x + 8 < sx || x + 8 >= sx + 8) continue;
							if(sx >= objX) continue;
							var xOff = (x + 8) - sx;
							if(!flags.HasBit(5))
								xOff = 7 - xOff;
							Debug.Assert(xOff is >= 0 and < 8);
							tile = Core.Memory.ReadBlock((ushort) (tileDataAddr + sti * 16 + sy * 2), 2);
							var sColor = ((tile[0] >> xOff) & 1) | (((tile[1] >> xOff) & 1) << 1);
							if(sColor == 0b00) continue;
							if(flags.HasBit(7) && bgColor != 0b00)
								objColor = mappedBgColor;
							else
								objColor = ((flags.HasBit(4) ? OBP1 : OBP0) >> (2 * sColor)) & 0b11;
							objX = sx;
						}
						color = objColor ?? mappedBgColor;
					} else
						color = mappedBgColor;

					var cval = color switch {
						0b00 => 255, 0b01 => 186, 
						0b10 => 100,    _ => 0
					};
					var poff = x * 3 + LY * 160 * 3 * 2;
					pbuf[poff] = pbuf[poff + 1] = pbuf[poff + 2] = (byte) cval;
				}
				
				Mode = Mode.Drawing;
				yield return 172;
				Mode = Mode.HBlank;
				yield return 204;
			}

			Mode = Mode.VBlank;
			Core.InterruptFlag |= 1;
			for(LY = 144; LY < 154; ++LY)
				yield return 456;
			var ti = 0;
			for(var ty = 0; ty < 20; ++ty) {
				for(var tx = 0; tx < 20; ++tx, ++ti) {
					if(ti == 384) break;
					for(var y = 0; y < 8; ++y) {
						var pi = 160 * 3 + (ty * 8 + y) * 160 * 2 * 3 + tx * 8 * 3;
						var tile = Core.Memory.ReadBlock((ushort) (0x8000 + ti * 16 + y * 2), 2);
						for(var x = 0; x < 8; ++x, pi += 3) {
							var offset = 7 - x;
							var color = ((tile[0] >> offset) & 1) | (((tile[1] >> offset) & 1) << 1);
							color = (BGP >> (2 * color)) & 0b11;
							var cval = (byte) (color switch {
								0b00 => 255, 0b01 => 186,
								0b10 => 100, _ => 0
							});
							
							pbuf[pi] = pbuf[pi + 1] = pbuf[pi + 2] = cval;
						}
					}
				}
			}
			Core.Stop();
		}
	}

	public void IoWrite(ushort addr, byte value) {
		switch(addr) {
			case 0xFF40:
				Lcdc = value;
				break;
			case 0xFF41: break;
			case 0xFF42: SCY = value; break;
			case 0xFF43: SCX = value; break;
			case 0xFF44: break;
			case 0xFF45: LYC = value; break;
			case 0xFF46: // TODO: Timing
				Core.Memory.WriteBlock(0xFE00, Core.Memory.ReadBlock((ushort) (value << 8), 40 * 4));
				break;
			case 0xFF47: BGP = value; break;
			case 0xFF48: OBP0 = value; break;
			case 0xFF49: OBP1 = value; break;
			default:
				Console.WriteLine($"Unhandled PPU IO Write to 0x{addr:X04}: 0x{value:X02}");
				break;
		}
	}

	public byte IoRead(ushort addr) {
		switch(addr) {
			case 0xFF40: return Lcdc;
			case 0xFF41: return (byte) Mode;
			case 0xFF42: return SCY;
			case 0xFF43: return SCX;
			case 0xFF44: return LY;
			case 0xFF45: return LYC;
			case 0xFF47: return BGP;
			case 0xFF48: return OBP0;
			case 0xFF49: return OBP1;
			default:
				Console.WriteLine($"Unhandled PPU IO Read from 0x{addr:X04}");
				return 0;
		}
	}
}