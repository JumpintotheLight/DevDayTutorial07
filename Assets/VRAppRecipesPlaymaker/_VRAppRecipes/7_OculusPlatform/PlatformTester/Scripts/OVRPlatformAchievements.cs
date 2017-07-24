using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using Oculus.Platform.Models;

namespace ZefirVR.OVRPlatform {
	public class OVRPlatformAchievements : MonoBehaviour
	{
		public Text dataOutput;

		#region Achievements

		public void AddFieldsAchievement(string achievementName, string fields)
		{
			Achievements.AddFields(achievementName, fields).OnComplete(AchievementFieldsCallback);
		}

		public void AddCountAchievement(string achievementName, string count)
		{
			Achievements.AddCount(achievementName, Convert.ToUInt64(count)).OnComplete(AchievementCountCallback);
		}

		public void UnlockAchievement(string achievementName)
		{
			Achievements.Unlock(achievementName).OnComplete(AchievementUnlockCallback);
		}

		public void GetAchievementProgress(string achievementName)
		{
			string[] Names = new string[1];
			Names[0] = achievementName;

			Achievements.GetProgressByName(Names).OnComplete(AchievementProgressCallback);
		}

		public void GetAchievementDefinition(string achievementName)
		{
			string[] Names = new string[1];
			Names[0] = achievementName;

			Achievements.GetDefinitionsByName(Names).OnComplete(AchievementDefinitionCallback);
		}

		void AchievementFieldsCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Achievement fields added.");
			}
			else
			{
				printOutputLine("Received achievement fields add error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void AchievementCountCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Achievement count added.");
			}
			else
			{
				printOutputLine("Received achievement count add error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void AchievementUnlockCallback(Message msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Achievement unlocked");
			}
			else
			{
				printOutputLine("Received achievement unlock error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void AchievementProgressCallback(Message<AchievementProgressList> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received achievement progress success");
				AchievementProgressList progressList = msg.GetAchievementProgressList();

				foreach (var progress in progressList.Data)
				{
					if (progress.IsUnlocked)
					{
						printOutputLine("Achievement Unlocked");
					}
					else
					{
						printOutputLine("Achievement Locked");
					}
					printOutputLine("Current Bitfield: " + progress.Bitfield.ToString());
					printOutputLine("Current Count: " + progress.Count.ToString());
				}
			}
			else
			{
				printOutputLine("Received achievement progress error");
				Error error = msg.GetError();
				printOutputLine("Error: " + error.Message);
			}
		}

		void AchievementDefinitionCallback(Message<AchievementDefinitionList> msg)
		{
			if (!msg.IsError)
			{
				printOutputLine("Received achievement definitions success");
				AchievementDefinitionList definitionList = msg.GetAchievementDefinitions();

				foreach (var definition in definitionList.Data)
				{
					switch (definition.Type)
					{
					case AchievementType.Simple:
						printOutputLine("Achievement Type: Simple");
						break;
					case AchievementType.Bitfield:
						printOutputLine("Achievement Type: Bitfield");
						printOutputLine("Bitfield Length: " + definition.BitfieldLength.ToString());
						printOutputLine("Target: " + definition.Target.ToString());
						break;
					case AchievementType.Count:
						printOutputLine("Achievement Type: Count");
						printOutputLine("Target: " + definition.Target.ToString());
						break;
					case AchievementType.Unknown:
					default:
						printOutputLine("Achievement Type: Unknown");
						break;
					}
				}
			}
			else
			{
				printOutputLine("Received achievement definitions error");
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