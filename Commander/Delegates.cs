namespace EphemereGames.Commander
{
    delegate void NoneHandler();
    delegate bool IntegerHandler(int i);
    delegate void PanelWidgetHandler(PanelWidget p);
}


namespace EphemereGames.Commander.Simulation
{
    using EphemereGames.Core.Physics;


    delegate void PhysicalObjectHandler(IObjetPhysique o);
    delegate void PhysicalObjectPhysicalObjectHandler(IObjetPhysique o1, IObjetPhysique o2);
    delegate void SimPlayerHandler(SimPlayer p);
    delegate void PausePlayerHandler(PausePlayer p);
    delegate void CommonStashHandler(CommonStash s);
    delegate void CelestialObjectHandler(CelestialBody c);
    delegate void NewGameStateHandler(GameState s);
    delegate void TurretHandler(Turret t);
    delegate void TurretSimPlayerHandler(Turret t, SimPlayer p);
    delegate void PowerUpTypeSimPlayerHandler(PowerUpType p, SimPlayer sp);
    delegate void PowerUpTypeHandler(PowerUpType p);
    delegate void PowerUpSimPlayerHandler(PowerUp p, SimPlayer pl);
    delegate void TurretTurretHandler(Turret t1, Turret t2);
    delegate void EnemyCelestialBodyHandler(Enemy e, CelestialBody c);
    delegate void TurretPhysicalObjectHandler(Turret t, IObjetPhysique o);
    delegate void EnemyBulletHandler(Enemy e, Bullet b);
    delegate void EditorPlayerHandler(EditorPlayer e);
    delegate void EditorCommandHandler(EditorCommand e);
    delegate void NextWaveHandler(WaveDescriptor d);
}
