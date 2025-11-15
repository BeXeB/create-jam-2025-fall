using Godot;
using MafiaGame.Interfaces;
using System;

public partial class ThrowingKniveProjectile : Node2D, IProjectile
{
	private float _damage;
	[Export]
	private float _speed = 500f;
	[Export]
	private float _lifetime = 3f;

	public float Damage { get => _damage; set => _damage = value; }

	public float Speed => _speed;

	public float Lifetime => _lifetime;

	public void Launch(Vector2 direction, float damage)
	{
		_damage = damage;
		var velocity = direction * _speed;
		var moveTween = CreateTween();
		moveTween.TweenProperty(this, "position", Position + velocity * _lifetime, _lifetime).SetTrans(Tween.TransitionType.Linear);
		moveTween.TweenCallback(Callable.From(() =>
		{
			QueueFree();
		}));
	}

	private void OnBodyEntered(Node2D body)
	{
		if (!body.IsInGroup("Mobs"))
		{
			return;
		}

		var enemyScript = body.GetParent() as IEnemy;

		enemyScript.Health -= _damage;
		if (enemyScript.Health <= 0)
		{
			enemyScript.Die();
		}

		QueueFree();
	}
}
