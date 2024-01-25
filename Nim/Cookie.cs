using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Nim
{
	public class Cookie
	{
		public Cookie(Cookie cookie) { this._texture = cookie._texture; this._drawLocation = cookie._drawLocation; }
		public Cookie(Texture2D texture) { _texture = texture; }
		public Texture2D _texture;
		public Vector2 _drawLocation;
	}
	public abstract class NotACookie : Cookie
	{
		public NotACookie(Cookie cookie) : base(cookie) {  }
		public NotACookie(Texture2D texture) : base(texture) { }
	}

	public class Snickerdoodle : Cookie {
		public Snickerdoodle(Cookie cookie) : base(cookie) { }
		public Snickerdoodle(Texture2D texture) : base(texture) { } }
	public class ChocolateChip : Cookie {
		public ChocolateChip(Cookie cookie) : base(cookie) { }
		public ChocolateChip(Texture2D texture) : base(texture) { } }
	public class Oatmeal : Cookie {
		public Oatmeal(Cookie cookie) : base(cookie) { }
		public Oatmeal(Texture2D texture) : base(texture) { } }
	public class Macarons : Cookie {
		public Macarons(Cookie cookie) : base(cookie) { }
		public Macarons(Texture2D texture) : base(texture) { } }
	public class Lofthouse : Cookie {
		public Lofthouse(Cookie cookie) : base(cookie) { }
		public Lofthouse(Texture2D texture) : base(texture) { } }
	public class Thumbprint : Cookie {
		public Thumbprint(Cookie cookie) : base(cookie) { }
		public Thumbprint(Texture2D texture) : base(texture) { } }
	public class Checkerboard : Cookie {
		public Checkerboard(Cookie cookie) : base(cookie) { }
		public Checkerboard(Texture2D texture) : base(texture) { } }
	public class Pretzel : NotACookie {
		public Pretzel(Cookie cookie) : base(cookie) { }
		public Pretzel(Texture2D texture) : base(texture) { } }

}
