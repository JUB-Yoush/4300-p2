using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class BGM
{
    public static AudioStream MenuMusic = GD.Load<AudioStream>(
        "res://assets/audio/Music/Waiting For Stream To Start.mp3"
    );
    public static AudioStream FightMusic = GD.Load<AudioStream>(
        "res://assets/audio/Music/Fighting Monsters.mp3"
    );
    public static AudioStream Static = GD.Load<AudioStream>(
        "res://assets/audio/UI/Static_Final.mp3"
    );
}

public static class SFX
{
    public static AudioStream KaijuDamage = GD.Load<AudioStream>(
        "res://assets/audio/Kaiju Sounds/k_damage.mp3"
    );
    public static AudioStream KaijuLow = GD.Load<AudioStream>(
        "res://assets/audio/Kaiju Sounds/Kaiju_Low.mp3"
    );
    public static AudioStream KaijuMid = GD.Load<AudioStream>(
        "res://assets/audio/Kaiju Sounds/Kaiju_Mid.mp3"
    );
    public static AudioStream KaijuHigh = GD.Load<AudioStream>(
        "res://assets/audio/Kaiju Sounds/Kaiju_High.mp3"
    );

    public static AudioStream MechJump = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/Mech_Jump_FINAL.mp3"
    );
    public static AudioStream MechPunch = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/Mech_Punch_FINAL.mp3"
    );
    public static AudioStream MechSpinKick = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/Mech_SpinKick_FINAL.mp3"
    );
    public static AudioStream MechDamage = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/PlayerDamage_Final.mp3"
    );
    public static AudioStream MechGunKick = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/PlayerGunKick_FINAL.mp3"
    );
    public static AudioStream MechMidSetup = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/PlayerMidSetup_FINAL.mp3"
    );
    public static AudioStream MechShield = GD.Load<AudioStream>(
        "res://assets/audio/Mech Sounds/PlayerShieldUp_FINAL.mp3"
    );
    public static AudioStream MechLaser = GD.Load<AudioStream>(
        "res://assets/audio/Weapon Noises/PlayerLaser_FINAL.mp3"
    );
    public static AudioStream MechRocket = GD.Load<AudioStream>(
        "res://assets/audio/Weapon Noises/PlayerRocket_Final.mp3"
    );

    public static AudioStream UIClick = GD.Load<AudioStream>(
        "res://assets/audio/UI/UI_Click_Final.mp3"
    );
    public static AudioStream UIHover = GD.Load<AudioStream>(
        "res://assets/audio/UI/UI_Select_Final.mp3"
    );
}

public partial class AudioManager : Node
{
    const int SFX_PLAYER_COUNT = 5;
    public int bus = AudioServer.GetBusIndex("Master");
    AudioStreamPlayer BgmPlayer = new();
    List<AudioStreamPlayer> SfxPlayers = [];

    public static AudioManager Ref = null!; // static singleton instance, global nodes are loaded before the scene tree.

    public override void _Ready()
    {
        Ref = this;
        AddChild(BgmPlayer);
        for (int i = 0; i < SFX_PLAYER_COUNT; i++)
        {
            var sfxPlayer = new AudioStreamPlayer();
            AddChild(sfxPlayer);
            SfxPlayers.Add(sfxPlayer);
        }
        BgmPlayer.ProcessMode = ProcessModeEnum.Always;
    }

    public static void PlayMusic(AudioStream music)
    {
        if (Ref.BgmPlayer.Stream == music)
        {
            return;
        }
        Ref.BgmPlayer.Stream = music;
        Ref.BgmPlayer.Play();
    }

    public static void PlaySfx(AudioStream sfx, bool singleStreamOnly = false)
    {
        // only one stream playing at a time
        if (singleStreamOnly)
        {
            var alreadyPlayingStream = Ref.SfxPlayers.FirstOrDefault(sfxPlayer =>
                sfxPlayer.Stream == sfx && sfxPlayer.Playing
            );
            if (alreadyPlayingStream != null)
            {
                return;
            }
        }

        var sfxPlayer = Ref.SfxPlayers.FirstOrDefault(sfxPlayer => !sfxPlayer.IsPlaying());

        if (sfxPlayer == null)
        {
            return;
        }

        sfxPlayer.Stream = sfx;
        sfxPlayer.Play();
    }

    public static void StopSfx(AudioStream sfx)
    {
        var sfxStream = Ref.SfxPlayers.FirstOrDefault(sfxPlayer =>
            sfxPlayer.Stream == sfx && sfxPlayer.Playing
        );
        sfxStream?.Stop();
    }

    public static void SetVolume(float value)
    {
        AudioServer.SetBusVolumeDb(Ref.bus, value);
    }

    public static void StopAll()
    {
        Ref.SfxPlayers.ForEach(SfxPlayer => SfxPlayer.Stop());
    }

    public static void PauseMusic()
    {
        Ref.BgmPlayer.Stop();
    }
}
