using System;
using System.Collections.Generic;
using Godot;

public partial class Healthbar : Control
{
	public Player player = null!;
	public Enemy enemy = null!;

	public TextureProgressBar PlayerHB = null!;
	public TextureProgressBar EnemyHB = null!;

	public int Phealth = 100;

	public override void _Ready()
	{
		player = GetParent().GetParent().GetNode<Player>("Player");
		enemy = GetParent().GetParent().GetNode<Enemy>("Enemy");

		PlayerHB = GetNode<TextureProgressBar>("CanvasLayer/HealthBar_Player/PlayerHealth");
		EnemyHB = GetNode<TextureProgressBar>("CanvasLayer/HealthBar_Enemy/EnemyHealth");
		//GD.Print(PlayerHB.Value.ToString());
		PlayerHB.Value = PlayerHB.MaxValue;
		EnemyHB.Value = EnemyHB.MaxValue;
	}

	public void PlayerHurt(int NewHP)
	{
		PlayerHB.Value = NewHP;
		GD.Print("OH NO");
	}

	public void EnemyHurt(int NewHP)
	{
		EnemyHB.Value = NewHP;
	}

	public override void _Process(double delta)
	{
		PlayerHB.Value = player.Hp;
		EnemyHB.Value = enemy.Hp;
	}
}
