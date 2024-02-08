using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pente
{
	internal class Button
	{
		public Texture2D texture;
		public Rectangle rect;

		public delegate void OnClick();
		public OnClick onClick;

		public delegate void OnHover();
		public OnHover onHover;

		public Button(Rectangle rect, Texture2D texture) 
		{ 
			this.rect = rect;
			this.texture = texture;
		}

		~Button()
		{
			onClick = null;
			onHover = null;
		}

		public void Draw(ref SpriteBatch spriteBatch)
		{
			if (texture == null) return;

			spriteBatch.Draw(texture, rect, Color.White);

		}

		public void Update(MouseState mouse, bool mouseUp)
		{ 
			if (rect.Contains(mouse.Position))
			{

				if (mouse.LeftButton == ButtonState.Pressed && mouseUp)
				{
					onClick?.Invoke();
				}
				else
				{
					onHover?.Invoke();

				}
			}
		}

	}
}
