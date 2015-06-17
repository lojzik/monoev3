﻿using System;
using System.Reflection;
using MonoBrickFirmware.FirmwareUpdate;
using MonoBrickFirmware.Connections;
using MonoBrickFirmware.Display.Dialogs;

namespace MonoBrickFirmware.Display.Menus
{
	public class ItemWithBrickInfo : IChildItem, IParentItem
	{
		private bool hasFocus = false;
		private GetInfoDialog dialog;
		Information information = null;

		public ItemWithBrickInfo ()
		{
			dialog = new GetInfoDialog();
			dialog.OnInformationLoaded += this.OnInformationLoaded;
		}

		public void OnEnterPressed ()
		{
			if (!hasFocus) 
			{
				if (information == null) 
				{
					dialog.SetFocus (this);
				} 
				else 
				{
					Parent.SetFocus (this);
					hasFocus = true;
				}
			} 
			else 
			{
				hasFocus = false;
				Parent.RemoveFocus (this);
			}
		}

		public void OnLeftPressed ()
		{
			
		}

		public void OnRightPressed ()
		{
			
		}

		public void OnUpPressed ()
		{
			
		}

		public void OnDownPressed ()
		{
			
		}

		public void OnEscPressed ()
		{
			if (hasFocus) 
			{
				Parent.RemoveFocus (this);
				hasFocus = false;
			}
		}

		public void OnDrawTitle (Font font, Rectangle rectangle, bool selected)
		{
			Lcd.Instance.WriteTextBox(font, rectangle, "Information", selected);	
		}

		public void OnDrawContent ()
		{
			string monoVersion = "Unknown";
			Type type = Type.GetType("Mono.Runtime");
			if (type != null)
			{                                          
				MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 
				if (displayName != null)                   
					monoVersion = (string)displayName.Invoke(null, null); 
			}	
			string monoCLR = System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
			var currentVersion = VersionHelper.CurrentVersions ();

			Point offset = new Point(0, (int)Font.MediumFont.maxHeight);
			Point startPos = new Point(0,0);
			Lcd.Instance.Clear();
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*0, "Firmware: " + currentVersion.Firmware, true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*1, "Image: " + currentVersion.Image , true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*2, "Mono version: " + monoVersion.Substring(0,7), true);
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*3, "Mono CLR: " + monoCLR, true);			
			Lcd.Instance.WriteText(Font.MediumFont, startPos+offset*4, "IP: " + WiFiDevice.GetIpAddress(), true);			

			Lcd.Instance.Update();
		}

		public void SetFocus (IChildItem item)
		{
			Parent.SetFocus (this);
		}

		public void RemoveFocus (IChildItem item)
		{
			if (dialog.Ok) 
			{
				Parent.SetFocus (this);
				hasFocus = true;
			} 
			else 
			{
				Parent.RemoveFocus (this);
			}
		}

		public void SuspendEvents (IChildItem item)
		{
			Parent.SuspendEvents (item);
		}

		public void ResumeEvents (IChildItem item)
		{
			Parent.ResumeEvents (item);
		}

		public void OnHideContent ()
		{
			
		}

		private void OnInformationLoaded(Information info)
		{
			information = info;
		}

		public IParentItem Parent { get; set; }
	}

	internal class Information
	{
		public string FirmwareVersion;
		public string ImageVersion;
		public string MonoVersion;
		public string MonoCLRVersion;
		public string IpAddress;	
	}


	internal class GetInfoDialog : ItemWithDialog<ProgressDialog>
	{
		private static Information info = null;
		public Action<Information> OnInformationLoaded = delegate {};
		public GetInfoDialog():base( new ProgressDialog("Getting Info", new StepContainer(LoadSettings, "Loading", "Failed to load info")))
		{
		
		}

		public bool Ok{get{return this.dialog.Ok;}}

		public override void OnExit (ProgressDialog dialog)
		{
			OnInformationLoaded(info);
			Parent.RemoveFocus (this);		
		}

		private static bool LoadSettings()
		{
			bool ok = true;
			try
			{
				string monoVersion = "Unknown";
				Type type = Type.GetType("Mono.Runtime");
				if (type != null)
				{                                          
					MethodInfo displayName = type.GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static); 
					if (displayName != null)                   
						monoVersion = (string)displayName.Invoke(null, null); 
				}	
				string monoCLR = System.Reflection.Assembly.GetExecutingAssembly().ImageRuntimeVersion;
				var currentVersion = VersionHelper.CurrentVersions ();
				string ip = WiFiDevice.GetIpAddress();
				info = new Information();
				info.FirmwareVersion = currentVersion.Firmware;
				info.ImageVersion = currentVersion.Image;
				info.IpAddress = ip;
				info.MonoCLRVersion = monoCLR;
				info.MonoVersion = monoVersion;
			}
			catch
			{
				ok = false;
			}
			return ok;
		}
	
	}



}

