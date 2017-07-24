using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace ZefirVR.OVRPlatform {
	public class OVRPlatformLeaderboards : MonoBehaviour
	{
		public Text dataOutput;

		//  Leaderboards
		#region Leaderboards

		public void GetLeaderboardEntries(string leaderboardName)
		{
			Leaderboards.GetEntries(leaderboardName, 10, LeaderboardFilterType.None, LeaderboardStartAt.Top).OnComplete(LeaderboardGetCallback);
		}

		public void AddLeaderboardEntry(string leaderboardName, string value)
		{
			byte[] extraData = new byte[] { 0x54, 0x65, 0x73, 0x74 };

			Leaderboards.WriteEntry(leaderboardName, Convert.ToInt32(value), extraData, false).OnComplete(LeaderboardAddCallback);
		}

		// Callbacks

		void LeaderboardGetCallback(Message<LeaderboardEntryList> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Leaderboard entry get success.");
				var entries = msg.Data;

				foreach (var entry in entries.Data)
				{
					printOutputLine(entry.Rank + ". " + entry.User.OculusID + " " + entry.Score + " " + entry.Timestamp);
				}
			}
			else
			{
				printOutputLine("Received leaderboard get error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void LeaderboardAddCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Leaderboard entry write success.");
				var didUpdate = (Message<bool>)msg;

				if (didUpdate.Data)
				{
					printOutputLine("Score updated.");
				}
				else
				{
					printOutputLine("Score NOT updated.");
				}
			}
			else
			{
				printOutputLine("Received leaderboard write error");
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
