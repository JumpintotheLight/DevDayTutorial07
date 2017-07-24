using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class SubtitleItem
{
	public float startTime = 0;
	public float endTime = 0;
	ArrayList text;

	public ArrayList Lines {
		get {
			if(text == null){
				text = new ArrayList();
			}
			return text;
		}
		set {
			text = value;
		}
	}
}

public class SubtitlesParser 
{
	private readonly string[] _delimiters = new string[] { "-->" , "- >", "->" };
	public SubtitlesParser(){}  
	
	public List<SubtitleItem> ParseStream(Stream srtStream, Encoding encoding)
	{
		// test stream
		if (!srtStream.CanRead || !srtStream.CanSeek)
		{
			var message = string.Format("Operation interrupted, stream is not readable");
			throw new ArgumentException(message);
		}
		
		srtStream.Position = 0;
		
		var reader = new StreamReader(srtStream, encoding, true);
		
		var items = new List<SubtitleItem>();
		var srtSubParts = GetSubtitleParts(reader).ToList();
		if (srtSubParts.Any())
		{
			foreach (var srtSubPart in srtSubParts)
			{
				var lines =
					srtSubPart.Split(new string[] {Environment.NewLine}, StringSplitOptions.None)
						.Select(s => s.Trim())
						.Where(l => !string.IsNullOrEmpty(l))
						.ToList();
				
				var item = new SubtitleItem();
				foreach (var l in lines)
				{
					string line = l;
					if (item.startTime == 0 && item.endTime == 0)
					{
						// find timecode
						int startTc;
						int endTc;
						var success = TryParseTimecodeLine(line, out startTc, out endTc);
						if (success)
						{
							item.startTime = ((float)startTc)/1000.0f;
							item.endTime = ((float)endTc)/1000.0f;
						}
					}
					else
					{
						// found the timecode
						item.Lines.Add(line);
					}
				}
				
				if ((item.startTime != 0 || item.endTime != 0) && item.Lines.Count>0)
				{
					// parsing completed
					items.Add(item);
				}
			}
			
			if (items.Any())
			{
				return items;
			}
			else
			{
				throw new ArgumentException("Not a valid Srt format");
			}
		}
		else
		{
			throw new FormatException("No srt format found");
		}
	}
	
	private IEnumerable<string> GetSubtitleParts(TextReader reader)
	{
		string line;
		var newString = new StringBuilder();
		
		while ((line = reader.ReadLine()) != null)
		{
			if (string.IsNullOrEmpty(line.Trim()))
			{
				// return only if not empty
				var res = newString.ToString().TrimEnd();
				if (!string.IsNullOrEmpty(res))
				{
					yield return res;
				}
				newString = new StringBuilder();
			}
			else
			{
				newString.AppendLine(line);
			}
		}
		
		if (newString.Length > 0)
		{
			yield return newString.ToString();
		}
	}
	
	private bool TryParseTimecodeLine(string line, out int startTc, out int endTc)
	{
		var parts = line.Split(_delimiters, StringSplitOptions.None);
		if (parts.Length != 2)
		{
			// not a timecode line
			startTc = -1;
			endTc = -1;
			return false;
		}
		else
		{
			startTc = ParseTimecode(parts[0]);
			endTc = ParseTimecode(parts[1]);
			return true;
		}
	}
	
	private int ParseTimecode(string timecodeSting)
	{
		var match = Regex.Match(timecodeSting, "[0-9]+:[0-9]+:[0-9]+[,\\.][0-9]+");
		if (match.Success)
		{
			timecodeSting = match.Value;
			TimeSpan result;
			if (TimeSpan.TryParse(timecodeSting.Replace(',', '.'), out result))
			{
				var mlSecs = (int)result.TotalMilliseconds;
				return mlSecs;
			}
		}
		return -1;
	}
}

