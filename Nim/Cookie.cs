using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nim
{
	public abstract class Cookie
	{
		Texture2D _texture;
	}
	public abstract class NotACookie : Cookie { }

	public class Snickerdoodle : Cookie { }
	public class ChocolateChip : Cookie { }
	public class Oatmeal : Cookie { }
	public class Macarons : Cookie { }
	public class Lofthouse : Cookie { }
	public class Thumbprint : Cookie { }
	public class Checkerboard : Cookie { }
	public class Pretzel : NotACookie { }

}
