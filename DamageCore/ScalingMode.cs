using LibSharpRetro;

namespace DamageCore; 

public enum ScalingMode {
	[UserOption("No scaling -- use original resolution")]
	None, 
	[UserOption("Nearest neighbor scaling -- cheap but very low quality")]
	NearestNeighbor, 
	[UserOption("Bilinear interpolation scaling -- fast but soft")]
	BilinearInterpolation
}