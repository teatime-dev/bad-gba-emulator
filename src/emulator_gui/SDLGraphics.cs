using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
namespace emulator_gui
{
	class SDLGraphics
	{
		bool Running;
		IntPtr Window;
		SDL_Surface surface;
		public SDLGraphics()
		{

		}
		public bool Run()
		{
			return true;
		}


		public void handleEvent(SDL_Event Event)
		{
			//bruh
		}
	}
}
