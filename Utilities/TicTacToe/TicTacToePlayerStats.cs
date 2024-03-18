using LearningBlazor.Utilities.Base.Player;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace LearningBlazor.Utilities.TicTacToe;
public class TicTacToePlayerStats : PlayerStats
{
	private readonly List<string> _symbolHistory;
	private string _prefferedSymbol;

	public TicTacToePlayerStats() : base()
	{
		_symbolHistory = [];
		_prefferedSymbol = "None";
	}
	public TicTacToePlayerStats(int wins, int losses, int ties, List<string> symbolHistory) 
		: base(wins, losses, ties) 
	{
		_symbolHistory = symbolHistory;
		SetPrefferedSymbol();
	}

	public string PrefferedSymbol
	{
		get => _prefferedSymbol;
		set => _prefferedSymbol = value;
	}

	public void SetPrefferedSymbol()
	{
		if (_symbolHistory.Count == 0)
		{
			_prefferedSymbol = "None";
		}

		int xTimes = _symbolHistory.Count(s => s.Equals("X"));
		double xPercentage = xTimes / _symbolHistory.Count;
		if (xPercentage >= 0.5)
		{
			_prefferedSymbol = "X";
		}
		else
		{
			_prefferedSymbol = "O";
		}
	}
	public void AddSymbolToHistory(string symbol)
	{
		_symbolHistory.Add(symbol);
		SetPrefferedSymbol();
	}
}
