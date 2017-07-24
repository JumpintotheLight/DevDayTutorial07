using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace ZefirVR.OVRPlatform {
	public class OVRPlatformUsers : MonoBehaviour
	{
		public Text dataOutput;

		#region Users and Friends

		public void GetLoggedInFriends()
		{
			printOutputLine("Trying to get friends of logged in user");
			Users.GetLoggedInUserFriends().OnComplete(GetFriendsCallback);
		}

		public void CheckEntitlement()
		{
			Entitlements.IsUserEntitledToApplication().OnComplete(getEntitlementCallback);
		}

		public void getUserNonce()
		{
			printOutputLine("Trying to get user nonce");
			Users.GetUserProof().OnComplete(userProofCallback);
		}

		public void GetLoggedInUser()
		{
			printOutputLine("Trying to get currently logged in user");
			Users.GetLoggedInUser().OnComplete(GetUserCallback);
		}

		void GetUserInfo(string userID)
		{
			printOutputLine("Trying to get user " + userID);
			Users.Get(Convert.ToUInt64(userID)).OnComplete(GetUserCallback);
		}

		public void GetInvitableUsers()
		{
			printOutputLine("Trying to get invitable users");
			Rooms.GetInvitableUsers().OnComplete(GetInvitableUsersCallback);
		}

		// Callbacks
		void userProofCallback(Message<UserProof> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received user nonce generation success");
				UserProof userNonce = msg.Data;
				printOutputLine("Nonce: " + userNonce.Value);
			}
			else
			{
				printOutputLine("Received user nonce generation error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}

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

		void GetUserCallback(Message<User> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received get user success");
				User user = msg.Data;
				printOutputLine("User: " + user.ID + " " + user.OculusID + " " + user.Presence + " " + user.InviteToken);
			}
			else
			{
				printOutputLine("Received get user error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void GetFriendsCallback(Message<UserList> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received get friends success");
				UserList users = msg.Data;
				outputUserArray(users);
			}
			else
			{
				printOutputLine("Received get friends error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void GetInvitableUsersCallback(Message<UserList> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received get invitable users success");
				UserList users = msg.Data;
				outputUserArray(users);
			}
			else
			{
				printOutputLine("Received get invitable users error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		#endregion

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
