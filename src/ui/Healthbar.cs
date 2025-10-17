using Godot;
using System.Collections.Generic;
using System;

public partial class Healthbar : Control
{
	public Player player;
	public Enemy enemy;
	
	public TextureProgressBar PlayerHB;
	public TextureProgressBar EnemyHB;
	
	public int Phealth = 100;
	
	
	
	public override void _Ready()
	{
		player = GetNode<Player>("main/Player");
		enemy = GetNode<Enemy>("main/Enemy");
		
		PlayerHB = GetNode<TextureProgressBar>("CanvasLayer/HealthBar_Player/PlayerHealth");
		EnemyHB = GetNode<TextureProgressBar>("CanvasLayer/HealthBar_Enemy/EnemyHealth");
		//GD.Print(PlayerHB.Value.ToString());
		PlayerHB.Value = PlayerHB.MaxValue;
		EnemyHB.Value = EnemyHB.MaxValue;
		
		GD.Print(player.Hp.ToString());
		
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
		PlayerHB.Value = PlayerHB.Value - 1;
	
		//PlayerHB.Value = player.getHP();
		//EnemyHB.Value = enemy.HP;
		EnemyHB.Value = EnemyHB.Value - 1;
	}
	
}
