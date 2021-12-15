namespace LibSharpRetro; 

[Flags]
public enum GraphicsBackend {
	Framebuffer = 1,
	OpenGL = 2,
	GLES = 4, 
	Vulkan = 8
}