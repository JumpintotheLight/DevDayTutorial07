using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace ZefirVR.OVRPlatform {
	public class OVRPlatformManager : MonoBehaviour
	{
		public Text dataOutput;

		public void InitPlatform()
		{
			Core.Initialize();
			CheckEntitlement();
		}

		void Update()
		{
			// Handle all messages being returned
			Request.RunCallbacks();
		}

		public void CheckEntitlement()
		{
			Entitlements.IsUserEntitledToApplication().OnComplete(getEntitlementCallback);
		}

		void getEntitlementCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("You are entitled to use this app.");
			}
			else
			{
				printOutputLine("You are NOT entitled to use this app.");
			}
		}

		#region Output helpers

		void printOutputLine(String newLine)
		{
			dataOutput.text = newLine;
			Debug.Log (newLine);
		}

		void outputRoomDetails(Room room)
		{
			printOutputLine("Room ID: " + room.ID + ", AppID: " + room.ApplicationID + ", Description: " + room.Description);
			printOutputLine("MaxUsers: " + room.MaxUsers.ToString() + " Users in room: " + room.Users.Data.Count);
			if (room.Owner != null)
			{
				printOutputLine("Room owner: " + room.Owner.ID + " " + room.Owner.OculusID);
			}
			printOutputLine("Join Policy: " + room.JoinPolicy.ToString());
			printOutputLine("Room Type: " + room.Type.ToString());

			Message.MessageType.Matchmaking_Enqueue.GetHashCode();

		}

		void outputUserArray(UserList users)
		{
			foreach (User user in users.Data)
			{
				printOutputLine("User: " + user.ID + " " + user.OculusID + " " + user.Presence + " " + user.InviteToken);
			}
		}

		#endregion
	}	
}
