using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace ZefirVR.OVRPlatform {
	public class OVRPlatformRooms : MonoBehaviour
	{
		public Text dataOutput;

		#region Rooms

		public void CreateAndJoinPrivateRoom(string joinPolicy, string maxUsers)
		{
			printOutputLine("Trying to create and join private room");
			Rooms.CreateAndJoinPrivate((RoomJoinPolicy)Convert.ToUInt32(joinPolicy), Convert.ToUInt32(maxUsers)).OnComplete(CreateAndJoinPrivateRoomCallback);
		}

		public void GetCurrentRoom()
		{
			printOutputLine("Trying to get current room");
			Rooms.GetCurrent().OnComplete(GetCurrentRoomCallback);
		}

		public void GetRoomInfo(string roomID)
		{
			printOutputLine("Trying to get room " + roomID);
			Rooms.Get(Convert.ToUInt64(roomID)).OnComplete(GetCurrentRoomCallback);
		}

		public void JoinRoom(string roomID)
		{
			printOutputLine("Trying to join room " + roomID);
			Rooms.Join(Convert.ToUInt64(roomID), true).OnComplete(JoinRoomCallback);
		}

		public void LeaveRoom(string roomID)
		{
			printOutputLine("Trying to leave room " + roomID);
			Rooms.Leave(Convert.ToUInt64(roomID)).OnComplete(LeaveRoomCallback);
		}

		public void KickUser(string roomID, string userID)
		{
			printOutputLine("Trying to kick user " + userID + " from room " + roomID);
			Rooms.KickUser(Convert.ToUInt64(roomID), Convert.ToUInt64(userID), 10 /*kick duration */).OnComplete(GetCurrentRoomCallback);
		}

		public void InviteUser(string roomID, string inviteToken)
		{
			printOutputLine("Trying to invite token " + inviteToken + " to room " + roomID);
			Rooms.InviteUser(Convert.ToUInt64(roomID), inviteToken).OnComplete(InviteUserCallback);
		}

		public void SetRoomDescription(string roomID, string description)
		{
			printOutputLine("Trying to set description " + description + " to room " + roomID);
			Rooms.SetDescription(Convert.ToUInt64(roomID), description).OnComplete(GetCurrentRoomCallback);
		}

		public void UpdateRoomDataStore(string roomID, string key, string value)
		{
			Dictionary<string, string> kvPairs = new Dictionary<string, string>();
			kvPairs.Add(key, value);

			printOutputLine("Trying to set k=" + key + " v=" + value + " for room " + roomID);
			Rooms.UpdateDataStore(Convert.ToUInt64(roomID), kvPairs).OnComplete(GetCurrentRoomCallback);
		}

		// Callbacks

		void CreateAndJoinPrivateRoomCallback(Message<Room> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received create and join room success");
				outputRoomDetails(msg.Data);
			}
			else
			{
				printOutputLine("Received create and join room error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void GetCurrentRoomCallback(Message<Room> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received get room success");
				outputRoomDetails(msg.Data);
			}
			else
			{
				printOutputLine("Received get room error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void JoinRoomCallback(Message<Room> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received join room success");
				outputRoomDetails(msg.Data);
			}
			else
			{
				printOutputLine("Received join room error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void LeaveRoomCallback(Message<Room> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received leave room success");
				outputRoomDetails(msg.Data);
			}
			else
			{
				printOutputLine("Received leave room error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void InviteUserCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received invite user success");
			}
			else
			{
				printOutputLine("Received invite user error");
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
