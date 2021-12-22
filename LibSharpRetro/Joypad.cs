using System.Diagnostics;

namespace LibSharpRetro; 

public class Joypad {
	public int A, B, Select, Start, Up, Down, Left, Right;
	public bool ActionSelected;

	public void KeyDown(byte scancode) {
		switch(scancode) {
			case 0x04: A = 1; break;
			case 0x16: B = 1; break;
			case 0x2C: Select = 1; break;
			case 0x28: Start = 1; break;
			case 0x52: Up = 1; break;
			case 0x51: Down = 1; break;
			case 0x50: Left = 1; break;
			case 0x4F: Right = 1; break;
		}
	}
	
	public void KeyUp(byte scancode) {
		switch(scancode) {
			case 0x04: A = A == 1 ? -1 : 0; break;
			case 0x16: B = B == 1 ? -1 : 0; break;
			case 0x2C: Select = Select == 1 ? -1 : 0; break;
			case 0x28: Start = Start == 1 ? -1 : 0; break;
			case 0x52: Up = Up == 1 ? -1 : 0; break;
			case 0x51: Down = Down == 1 ? -1 : 0; break;
			case 0x50: Left = Left == 1 ? -1 : 0; break;
			case 0x4F: Right = Right == 1 ? -1 : 0; break;
		}
	}

	public void IoWrite(ushort addr, byte value) {
		Debug.Assert(addr == 0xFF00);
		if(!value.HasBit(4)) ActionSelected = false;
		if(!value.HasBit(5)) ActionSelected = true;
	}

	public byte IoRead(ushort addr) {
		Debug.Assert(addr == 0xFF00);
		if(ActionSelected) {
			var v = (byte) (
				0b11010000 |
				(Start == 0 ? (1 << 3) : 0) |
				(Select == 0 ? (1 << 2) : 0) |
				(B == 0 ? (1 << 1) : 0) |
				(A == 0 ? 1 : 0));
			Start = Start == -1 ? 0 : Start;
			Select = Select == -1 ? 0 : Select;
			B = B == -1 ? 0 : B;
			A = A == -1 ? 0 : A;
			return v;
		} else {
			var v = (byte) (
				0b11100000 |
				(Down == 0 ? (1 << 3) : 0) |
				(Up == 0 ? (1 << 2) : 0) |
				(Left == 0 ? (1 << 1) : 0) |
				(Right == 0 ? 1 : 0));
			Down = Down == -1 ? 0 : Down;
			Up = Up == -1 ? 0 : Up;
			Left = Left == -1 ? 0 : Left;
			Right = Right == -1 ? 0 : Right;
			return v;
		}
	}
}