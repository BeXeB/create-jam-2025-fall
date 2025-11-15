using Godot;
using System;
using System.Collections.Generic;

public partial class UiScene : CanvasLayer
{
	[Signal]
	public delegate void RepuUpDoneEventHandler(string upgradeName);
	[Signal]
	public delegate void StartGameEventHandler();
	private (string displayText, string upgradeName)[] _currentOptions = [];
	
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	public void ShowGameOver()
	{
		var message = GetNode<Label>("Message");
		var startButton = GetNode<Button>("StartButton");
		message.Text = "Game Over";
		message.Show();
		startButton.Show();   
	}

	private void OnStartButtonPressed()
	{
		GetNode<Label>("Message").Hide();
		GetNode<Button>("StartButton").Hide();
		EmitSignal(SignalName.StartGame);
	}

	private void OnPlayerHealthChanged(float newHealth, float maxHealth)
	{
		var healthBar = GetNode<ProgressBar>("HealthBar");
		healthBar.MaxValue = maxHealth;
		healthBar.Value = newHealth;
		var healthLabel = GetNode<Label>("HealthBar/HealthText");
		healthLabel.Text = $"{newHealth}/{maxHealth}";
	}

	private void OnReputationExpChanged(float reputationExp, float reputationExpToNextLevel)
	{
		var repExpBar = GetNode<ProgressBar>("RepuBar");
		repExpBar.MaxValue = reputationExpToNextLevel;
		repExpBar.Value = reputationExp;
		var repExpLabel = GetNode<Label>("RepuBar/RepuText");
		repExpLabel.Text = $"{reputationExp:0.0}/{reputationExpToNextLevel:0.0}";
	}

	private void OnReputationUp(float newReputationLevel)
	{
		var repLabel = GetNode<Label>("RepuLevelText");
		repLabel.Text = $"Reputation: {newReputationLevel:0}";
	}

	public void ShowRepuUp((string displayText, string upgradeName)[] upgradeOptions)
	{
		var levelUpMessage = GetNode<Control>("RepuUpPanel");
		var Upgrade1Label = GetNode<Label>("RepuUpPanel/Option1/UpgradeName");
		var Upgrade2Label = GetNode<Label>("RepuUpPanel/Option2/UpgradeName");
		var Upgrade3Label = GetNode<Label>("RepuUpPanel/Option3/UpgradeName");
		Upgrade1Label.Text = upgradeOptions[0].displayText;
		Upgrade2Label.Text = upgradeOptions[1].displayText;
		Upgrade3Label.Text = upgradeOptions[2].displayText;
		_currentOptions = upgradeOptions;
		levelUpMessage.Show();
	}

	public void HideRepuUp()
	{
		var levelUpMessage = GetNode<Control>("RepuUpPanel");
		levelUpMessage.Hide();
	}

	public void OnRepuUpDoneButtonPressed(string upgradeName)
	{
		EmitSignal(SignalName.RepuUpDone, upgradeName);
	}

	public void OnOption1Selected()
	{
		OnRepuUpDoneButtonPressed(_currentOptions[0].upgradeName);
	}

	public void OnOption2Selected()
	{
		OnRepuUpDoneButtonPressed(_currentOptions[1].upgradeName);
	}

	public void OnOption3Selected()
	{
		OnRepuUpDoneButtonPressed(_currentOptions[2].upgradeName);
	}
}
