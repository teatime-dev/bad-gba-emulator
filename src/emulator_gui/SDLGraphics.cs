using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static SDL2.SDL;
namespace emulator_gui
{
	class SDLGraphics
	{
		static int SCREEN_WIDTH = 640;
		static int SCREEN_HEIGHT = 480;

		bool Running;
		IntPtr screenSurface;
		IntPtr window;
		public SDLGraphics()
		{
			
		}
		public void Run()
		{
			Init();

			SDL_Surface sur;
			sur = (SDL_Surface)Marshal.PtrToStructure(screenSurface, typeof(SDL_Surface));
			SDL_FillRect(screenSurface, IntPtr.Zero, SDL_MapRGB(sur.format, 0xFF, 0x12, 0xFF));

			SDL_UpdateWindowSurface(window);

//			SDL_Delay(2000);

//			Close();

			return;
		}

		private void Init()
		{
			if (SDL_Init(SDL_INIT_VIDEO) < 0)
			{
				throw new Exception("SDL Could not initialise!" + SDL_GetError());
			}
			window = SDL_CreateWindow("SDL Tutorial", SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, SCREEN_WIDTH, SCREEN_HEIGHT, SDL_WindowFlags.SDL_WINDOW_SHOWN);
			if (window == null)
			{
				throw new Exception("Window could not be created! SDL_Error: " + SDL_GetError());
			}
			screenSurface = SDL_GetWindowSurface(window);
		}

		public void setColor(byte r, byte g, byte b)
		{
			SDL_Surface sur;
			sur = (SDL_Surface)Marshal.PtrToStructure(screenSurface, typeof(SDL_Surface));
			SDL_FillRect(screenSurface, IntPtr.Zero, SDL_MapRGB(sur.format, r, g, b));

			SDL_UpdateWindowSurface(window);
		}

		public void Close()
		{
			SDL_DestroyWindow(window);

			SDL_Quit();
		}

		public void handleEvent(SDL_Event Event)
		{
			//bruh
		}
	}
}
