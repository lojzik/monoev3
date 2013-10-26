using System;
using MonoBrickFirmware;
using MonoBrickFirmware.IO;
namespace NXTColorSensorExample
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			NXTColorMode[] modes = {NXTColorMode.Color, NXTColorMode.Reflection, NXTColorMode.Ambient, NXTColorMode.Blue, NXTColorMode.Green};
			int modeIdx = 0;
			bool run = true;
			var colorSensor = new NXTColorSensor(SensorPort.In1);
			ButtonEvents buts = new ButtonEvents ();
			
			buts.EnterPressed += () => { 
				run  = false;
			};
			buts.UpPressed += () => { 
				Console.WriteLine("Sensor value: " + colorSensor.ReadAsString());
			};
			buts.LeftPressed += () => { 
				Console.WriteLine("Raw sensor value: " + colorSensor.ReadRaw());
			};
			buts.RightPressed += () => { 
				RGBColor RGB = colorSensor.ReadRGBColor();
				Console.WriteLine("Red value  : " + RGB.Red);
				Console.WriteLine("Green value: " + RGB.Green);
				Console.WriteLine("Blue value : " + RGB.Blue);
			};
			buts.DownPressed += () => { 
				modeIdx = (modeIdx+1)%modes.Length;
				colorSensor.Mode = modes[modeIdx];
				Console.WriteLine("Sensor mode is now: " + colorSensor.Mode);
			};  
			while (run) {
				System.Threading.Thread.Sleep(50);
			}
		}
	}
}
