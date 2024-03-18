using Games.TicTacToe;
using Newtonsoft.Json;

namespace WebApp.Utilities;
public class BoardConverter : JsonConverter
{
	public override bool CanConvert(Type objectType)
		=> objectType == typeof(TicTacToeBoardModel);

	public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
	{
		var board = new TicTacToeBoardModel();
		int flatIndex = 0;

		while (reader.Read())
		{
			if (reader.Value != null && reader.Value.ToString() != "Board" && reader.Path.Contains("Board["))
			{
				board.Mark(reader.Value.ToString()!, flatIndex / 3, flatIndex % 3);
				flatIndex += 1;
			}
		}

		return board;
	}

	public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
	{
		if (value == null)
		{
			serializer.Serialize(writer, null);
			return;
		}

		var board = (TicTacToeBoardModel)value;
		writer.WriteStartObject();
		writer.WritePropertyName("Board");
		writer.WriteStartArray();
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				serializer.Serialize(writer, board[i, j]);
			}
		}
		writer.WriteEndArray();
	}
}
